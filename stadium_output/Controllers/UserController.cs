using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MimeKit;
using StadiumWeb.Data;
using StadiumWeb.Models;
using StadiumWeb.Services;

namespace StadiumWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly DbHelper _db;
        private readonly EmailService _email;
        private readonly OtpService _otp;
        private readonly IConfiguration _config;

        public UserController(DbHelper db, EmailService email, OtpService otp, IConfiguration config)
        {
            _db = db;
            _email = email;
            _otp = otp;
            _config = config;
        }

        bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;

        // ── LOGIN ──────────────────────────────────────────────
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            using var con = _db.GetConnection();
            con.Open();
            // Fetch email too so we always have it in session for ticket confirmations
            var cmd = new MySqlCommand("SELECT password, user_name, email FROM user WHERE user_id=@id", con);
            cmd.Parameters.AddWithValue("@id", model.Id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read() && rdr.GetString(0) == model.Password)
            {
                HttpContext.Session.SetInt32("UserId", model.Id);
                HttpContext.Session.SetString("UserName", rdr.GetString(1));
                // Store email — may be null/DBNull if DB has no email column yet
                try { HttpContext.Session.SetString("UserEmail", rdr.IsDBNull(2) ? "" : rdr.GetString(2)); }
                catch { /* email column doesn't exist in older schemas */ }
                return RedirectToAction("Dashboard");
            }
            ViewBag.Error = "Invalid User ID or Password.";
            return View(model);
        }

        // ── REGISTER — Step 1: fill form → send OTP ────────────────
        public IActionResult Register() => View();

        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterPost(RegisterViewModel model)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(model.UserName) ||
                string.IsNullOrWhiteSpace(model.Password) ||
                string.IsNullOrWhiteSpace(model.MobileNo) ||
                string.IsNullOrWhiteSpace(model.Email))
            {
                ViewBag.Error = "All fields are required.";
                return View(model);
            }
            if (model.MobileNo.Length != 10 || !model.MobileNo.All(char.IsDigit))
            {
                ViewBag.Error = "Mobile number must be exactly 10 digits.";
                return View(model);
            }
            if (!model.Email.Contains('@') || !model.Email.Contains('.'))
            {
                ViewBag.Error = "Please enter a valid email address.";
                return View(model);
            }

            // Store registration data in session pending OTP verification
            // UserId is NOT collected from user — it will be auto-generated on DB insert
            HttpContext.Session.SetString("Reg_UserName", model.UserName);
            HttpContext.Session.SetString("Reg_Password", model.Password);
            HttpContext.Session.SetString("Reg_MobileNo", model.MobileNo);
            HttpContext.Session.SetString("Reg_Email",    model.Email);

            // Send OTP via Email
            var (sent, emailSentTo, errMsg) = await _otp.SendOtpAsync(
                model.MobileNo, model.Email, model.UserName);

            if (!sent)
            {
                // OTP is still stored in memory — redirect anyway so dev can use console OTP
                TempData["DevOtpError"] = errMsg;
            }
            else
            {
                TempData["OtpEmailSentTo"] = emailSentTo;
            }

            return RedirectToAction("VerifyOtp");
        }

        // ── REGISTER — Step 2: verify OTP ─────────────────────────────
        public IActionResult VerifyOtp()
        {
            var mobile = HttpContext.Session.GetString("Reg_MobileNo");
            if (string.IsNullOrEmpty(mobile)) return RedirectToAction("Register");
            ViewBag.MobileNo = mobile;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string otpCode)
        {
            var mobile   = HttpContext.Session.GetString("Reg_MobileNo");
            var userName = HttpContext.Session.GetString("Reg_UserName");
            var password = HttpContext.Session.GetString("Reg_Password");
            var email    = HttpContext.Session.GetString("Reg_Email");

            if (string.IsNullOrEmpty(mobile))
            {
                TempData["Error"] = "Session expired. Please register again.";
                return RedirectToAction("Register");
            }

            ViewBag.MobileNo = mobile;

            if (string.IsNullOrWhiteSpace(otpCode) || otpCode.Length != 6)
            {
                ViewBag.Error = "Please enter the 6-digit OTP.";
                return View();
            }

            if (!_otp.VerifyOtp(mobile, otpCode))
            {
                ViewBag.Error = "Invalid or expired OTP. Please try again.";
                return View();
            }

            // OTP verified — complete registration; user_id is AUTO_INCREMENT
            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO user(user_name, password, mobile_no, email) VALUES(@name,@pass,@mob,@email)", con);
            cmd.Parameters.AddWithValue("@name",  userName);
            cmd.Parameters.AddWithValue("@pass",  password);
            cmd.Parameters.AddWithValue("@mob",   mobile);
            cmd.Parameters.AddWithValue("@email", email);

            try
            {
                cmd.ExecuteNonQuery();

                // Retrieve the auto-generated user_id
                var idCmd = new MySqlCommand("SELECT LAST_INSERT_ID()", con);
                int newUserId = Convert.ToInt32(idCmd.ExecuteScalar());

                // Clear registration session data
                HttpContext.Session.Remove("Reg_UserName");
                HttpContext.Session.Remove("Reg_Password");
                HttpContext.Session.Remove("Reg_MobileNo");
                HttpContext.Session.Remove("Reg_Email");

                // Send welcome email with the auto-generated User ID
                _ = _email.SendWelcomeEmailAsync(email!, userName!, newUserId);

                TempData["Success"] = $"✅ Mobile verified! Registration successful. Your User ID has been sent to {email}. Please check your email, then login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Registration failed: " + ex.Message;
                return View();
            }
        }

        // ── RESEND OTP ─────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> ResendOtp()
        {
            var mobile   = HttpContext.Session.GetString("Reg_MobileNo");
            var email    = HttpContext.Session.GetString("Reg_Email") ?? "";
            var userName = HttpContext.Session.GetString("Reg_UserName") ?? "User";
            if (string.IsNullOrEmpty(mobile)) return RedirectToAction("Register");

            var (sent, emailSentTo, errMsg) = await _otp.SendOtpAsync(mobile, email, userName);
            ViewBag.MobileNo = mobile;
            if (sent)
                ViewBag.Info = $"A new OTP has been sent to {emailSentTo}.";
            else
                ViewBag.Error = errMsg ?? "Failed to resend OTP.";
            return View("VerifyOtp");
        }

        // ── LOGOUT ─────────────────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ── DASHBOARD ─────────────────────────────────────────
        public IActionResult Dashboard()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            using var con = _db.GetConnection();
            con.Open();
            var cmd1 = new MySqlCommand("SELECT COUNT(*) FROM ticket WHERE user_id=@id", con);
            cmd1.Parameters.AddWithValue("@id", userId);
            ViewBag.TicketCount = cmd1.ExecuteScalar();
            var cmd2 = new MySqlCommand("SELECT COALESCE(SUM(total_payments),0) FROM ticket WHERE user_id=@id", con);
            cmd2.Parameters.AddWithValue("@id", userId);
            ViewBag.TotalSpent = cmd2.ExecuteScalar();
            var cmd3 = new MySqlCommand("SELECT COUNT(*) FROM matches", con);
            ViewBag.MatchCount = cmd3.ExecuteScalar();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        // ── MATCHES ───────────────────────────────────────────
        public IActionResult Matches()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            var list = new List<Match>();
            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand("SELECT * FROM matches ORDER BY match_id", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                list.Add(new Match
                {
                    MatchId = rdr.GetInt32(0), MatchName = rdr.GetString(1),
                    SeriesTournamentName = rdr.GetString(2), MatchFormat = rdr.GetString(3),
                    MatchDate = rdr.GetString(4), MatchTime = rdr.GetString(5)
                });
            return View(list);
        }

        // ── BOOK TICKET ───────────────────────────────────────
        public IActionResult BookTicket(int matchId = 0)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            ViewBag.Matches = LoadMatches();
            ViewBag.SelectedMatchId = matchId;
            return View(new BookTicketViewModel { MatchId = matchId });
        }

        [HttpPost]
        public async Task<IActionResult> BookTicket(BookTicketViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;

            var validStands  = new[] { "A", "B", "C", "D" };
            var validMethods = new[] { "UPI", "Debit Card", "Credit Card", "Netbanking" };

            if (model.MatchId <= 0)          { ViewBag.Error = "Please select a valid match."; return ReloadBookTicketView(model); }
            if (!validStands.Contains(model.Stand))   { ViewBag.Error = "Please select a valid stand."; return ReloadBookTicketView(model); }
            if (model.NoOfTickets < 1 || model.NoOfTickets > 20) { ViewBag.Error = "Tickets must be between 1 and 20."; return ReloadBookTicketView(model); }
            if (!validMethods.Contains(model.PaymentMethod)) { ViewBag.Error = "Please select a valid payment method."; return ReloadBookTicketView(model); }

            int price = model.Stand switch { "A" => 1000, "B" => 7000, "C" => 5000, "D" => 10000, _ => 0 };
            int total = price * model.NoOfTickets;

            using var con = _db.GetConnection();
            con.Open();

            // Verify match exists and get details
            var matchCmd = new MySqlCommand("SELECT * FROM matches WHERE match_id=@mid", con);
            matchCmd.Parameters.AddWithValue("@mid", model.MatchId);
            Match? match = null;
            using (var rdr = matchCmd.ExecuteReader())
            {
                if (rdr.Read())
                    match = new Match
                    {
                        MatchId = rdr.GetInt32(0), MatchName = rdr.GetString(1),
                        SeriesTournamentName = rdr.GetString(2), MatchFormat = rdr.GetString(3),
                        MatchDate = rdr.GetString(4), MatchTime = rdr.GetString(5)
                    };
            }
            if (match == null) { ViewBag.Error = "Selected match does not exist."; return ReloadBookTicketView(model); }

            // Get user email — try DB first, fall back to session, then to config
            var userCmd = new MySqlCommand("SELECT user_name, email FROM user WHERE user_id=@uid", con);
            userCmd.Parameters.AddWithValue("@uid", userId);
            string userEmail = "", userName2 = "";
            try
            {
                using var rdr2 = userCmd.ExecuteReader();
                if (rdr2.Read())
                {
                    userName2 = rdr2.IsDBNull(0) ? "" : rdr2.GetString(0);
                    userEmail = rdr2.IsDBNull(1) ? "" : rdr2.GetString(1);
                }
            }
            catch
            {
                userName2 = HttpContext.Session.GetString("UserName") ?? "";
                userEmail = "";
            }

            // Fallback chain: session → config default
            if (string.IsNullOrEmpty(userEmail))
                userEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            if (string.IsNullOrEmpty(userEmail))
                userEmail = _config["Smtp:UserName"] ?? ""; // send to inbox owner as fallback
            if (string.IsNullOrEmpty(userName2))
                userName2 = HttpContext.Session.GetString("UserName") ?? "Customer";

            try
            {
                // Insert ticket
                var cmd = new MySqlCommand(
                    "INSERT INTO ticket(match_id,user_id,stand,ticket_price,no_of_tickets,total_payments,payment_method) VALUES(@mid,@uid,@st,@tp,@nt,@tot,@pm)", con);
                cmd.Parameters.AddWithValue("@mid", model.MatchId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@st",  model.Stand);
                cmd.Parameters.AddWithValue("@tp",  price);
                cmd.Parameters.AddWithValue("@nt",  model.NoOfTickets);
                cmd.Parameters.AddWithValue("@tot", total);
                cmd.Parameters.AddWithValue("@pm",  model.PaymentMethod);
                cmd.ExecuteNonQuery();

                // Get the new ticket ID
                var idCmd = new MySqlCommand("SELECT LAST_INSERT_ID()", con);
                int ticketId = Convert.ToInt32(idCmd.ExecuteScalar());

                // Build ticket object for email
                var ticket = new Ticket
                {
                    TicketId = ticketId, MatchId = model.MatchId, UserId = userId,
                    Stand = model.Stand, TicketPrice = price,
                    NoOfTickets = model.NoOfTickets, TotalPayments = total,
                    PaymentMethod = model.PaymentMethod
                };

                // Send confirmation email with PDF ticket attached
                string emailStatus = "";
                if (!string.IsNullOrEmpty(userEmail))
                {
                    try
                    {
                        await _email.SendTicketConfirmationAsync(userEmail, userName2, ticket, match);
                        emailStatus = $" Confirmation email sent to {userEmail}.";
                    }
                    catch (Exception ex)
                    {
                        // Log the real error so developer can see it in terminal
                        var logger = HttpContext.RequestServices
                            .GetRequiredService<ILogger<UserController>>();
                        logger.LogError(ex, "Ticket email FAILED for {Email} — {Msg}", userEmail, ex.Message);
                        emailStatus = " (Email delivery failed — check SMTP settings.)";
                    }
                }
                else
                {
                    emailStatus = " (No email on file — update your profile to receive confirmations.)";
                }

                TempData["Success"] = $"🎉 Booking confirmed! {model.NoOfTickets} ticket(s) for {match.MatchName}. Total: ₹{total:N0}.{emailStatus}";
                return RedirectToAction("MyTickets");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Booking failed: " + ex.Message;
                return ReloadBookTicketView(model);
            }
        }

        // ── MY TICKETS ────────────────────────────────────────
        public IActionResult MyTickets()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var list = new List<Ticket>();
            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand(@"
                SELECT t.*, m.match_name, m.match_date, m.match_time
                FROM ticket t
                JOIN matches m ON t.match_id = m.match_id
                WHERE t.user_id = @uid
                ORDER BY t.ticket_id DESC", con);
            cmd.Parameters.AddWithValue("@uid", userId);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                list.Add(new Ticket
                {
                    TicketId = rdr.GetInt32(0), MatchId = rdr.GetInt32(1), UserId = rdr.GetInt32(2),
                    Stand = rdr.GetString(3), TicketPrice = rdr.GetInt32(4),
                    NoOfTickets = rdr.GetInt32(5), TotalPayments = rdr.GetInt32(6),
                    PaymentMethod = rdr.GetString(7), MatchName = rdr.GetString(8),
                    MatchDate = rdr.GetString(9), MatchTime = rdr.GetString(10)
                });
            return View(list);
        }

        // ── CHANGE PASSWORD ───────────────────────────────────
        public IActionResult ChangePassword() => !IsLoggedIn() ? RedirectToAction("Login") : View();

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            using var con = _db.GetConnection();
            con.Open();
            var check = new MySqlCommand("SELECT password FROM user WHERE user_id=@id", con);
            check.Parameters.AddWithValue("@id", userId);
            if (check.ExecuteScalar()?.ToString() != oldPassword)
            {
                ViewBag.Error = "Old password is incorrect.";
                return View();
            }
            var update = new MySqlCommand("UPDATE user SET password=@p WHERE user_id=@id", con);
            update.Parameters.AddWithValue("@p", newPassword);
            update.Parameters.AddWithValue("@id", userId);
            update.ExecuteNonQuery();
            TempData["Success"] = "Password changed successfully!";
            return RedirectToAction("Dashboard");
        }

        // ── DELETE ACCOUNT ────────────────────────────────────
        public IActionResult DeleteAccount() => !IsLoggedIn() ? RedirectToAction("Login") : View();

        [HttpPost]
        public IActionResult DeleteAccountConfirm()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand("CALL delete_user(@uid)", con);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.ExecuteNonQuery();
            HttpContext.Session.Clear();
            TempData["Success"] = "Your account has been deleted.";
            return RedirectToAction("Index", "Home");
        }

        // ── DEBUG EMAIL (visit /User/DebugEmail while logged in) ──
        public IActionResult DebugEmail()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var sb = new System.Text.StringBuilder();
            sb.Append("<pre style='font-family:monospace;font-size:14px;padding:24px;'>");
            sb.AppendLine("<b>== Email Resolution Debug ==</b>\n");
            sb.AppendLine($"Session UserId   : {userId}");
            sb.AppendLine($"Session UserName : {HttpContext.Session.GetString("UserName")}");
            sb.AppendLine($"Session UserEmail: '{HttpContext.Session.GetString("UserEmail")}'");
            sb.AppendLine($"Config Smtp:User : {_config["Smtp:UserName"]}");

            try
            {
                using var con = _db.GetConnection();
                con.Open();
                var cmd = new MySqlCommand("SELECT user_name, email FROM user WHERE user_id=@uid", con);
                cmd.Parameters.AddWithValue("@uid", userId);
                using var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    sb.AppendLine($"\nDB user_name : '{(rdr.IsDBNull(0) ? "NULL" : rdr.GetString(0))}'");
                    sb.AppendLine($"DB email     : '{(rdr.IsDBNull(1) ? "NULL" : rdr.GetString(1))}'");
                }
                else sb.AppendLine("\nNo user row found for this UserId!");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"\n❌ DB query failed: {ex.Message}");
                sb.AppendLine("\nThe 'email' column is missing from your user table.");
                sb.AppendLine("Run this SQL in phpMyAdmin / MySQL Workbench:\n");
                sb.AppendLine($"  ALTER TABLE user ADD COLUMN email VARCHAR(255) NULL;");
                sb.AppendLine($"  UPDATE user SET email = 'bhanderibhavya15@gmail.com' WHERE user_id = {userId};");
            }
            sb.Append("</pre>");
            return Content(sb.ToString(), "text/html");
        }

        // ── TEST EMAIL (dev only — visit /User/TestEmail) ─────
        public async Task<IActionResult> TestEmail()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<pre style='font-family:monospace;font-size:14px;padding:24px;'>");
            sb.AppendLine("<b>== NM Stadium SMTP Diagnostics ==</b>\n");

            var host  = _config["Smtp:Host"]     ?? "(not set)";
            var port  = _config["Smtp:Port"]     ?? "(not set)";
            var user  = _config["Smtp:UserName"] ?? "(not set)";
            var pass  = _config["Smtp:Password"] ?? "";

            sb.AppendLine($"Host     : {host}");
            sb.AppendLine($"Port     : {port}");
            sb.AppendLine($"UserName : {user}");
            sb.AppendLine($"Password : {new string('*', pass.Length)} ({pass.Length} chars)");

            var rawPass = pass.Replace(" ", "");
            sb.AppendLine(rawPass.Length == 16
                ? "\n✅ Password looks like a valid Gmail App Password (16 chars)\n"
                : $"\n⚠️  Password is {rawPass.Length} chars — Gmail App Passwords must be exactly 16 chars\n");

            sb.AppendLine("Attempting SMTP connection...");
            try
            {
                var (ok, error) = await _email.TestSmtpAsync(host, port, user, rawPass);
                if (ok)
                {
                    sb.AppendLine($"  ✅ Connected &amp; authenticated OK");
                    sb.AppendLine($"  ✅ Test email sent to <b>{user}</b> — check your inbox!");
                    sb.AppendLine("\n<b style='color:green'>✅ ALL CHECKS PASSED — ticket emails will work.</b>");
                }
                else
                {
                    sb.AppendLine($"\n❌ FAILED: {System.Net.WebUtility.HtmlEncode(error)}");
                    sb.AppendLine("\n<b>Common fixes:</b>");
                    sb.AppendLine("  • Go to https://myaccount.google.com/apppasswords");
                    sb.AppendLine("  • Generate a new App Password (Mail → Windows Computer)");
                    sb.AppendLine("  • Paste the 16-char code (no spaces) into appsettings.json → Smtp:Password");
                    sb.AppendLine("  • Make sure 2-Step Verification is ON for the Gmail account");
                    sb.AppendLine("  • Restart: dotnet run");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"\n❌ Exception: {System.Net.WebUtility.HtmlEncode(ex.Message)}");
            }

            sb.Append("</pre>");
            return Content(sb.ToString(), "text/html");
        }
        private List<Match> LoadMatches()
        {
            var list = new List<Match>();
            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand("SELECT * FROM matches ORDER BY match_id", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                list.Add(new Match
                {
                    MatchId = rdr.GetInt32(0), MatchName = rdr.GetString(1),
                    SeriesTournamentName = rdr.GetString(2), MatchFormat = rdr.GetString(3),
                    MatchDate = rdr.GetString(4), MatchTime = rdr.GetString(5)
                });
            return list;
        }

        private IActionResult ReloadBookTicketView(BookTicketViewModel model)
        {
            ViewBag.Matches = LoadMatches();
            ViewBag.SelectedMatchId = model.MatchId;
            return View(model);
        }
    }
}
