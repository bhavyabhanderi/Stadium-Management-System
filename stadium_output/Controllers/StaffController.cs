using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using StadiumWeb.Data;
using StadiumWeb.Models;

namespace StadiumWeb.Controllers
{
    public class StaffController : Controller
    {
        private readonly DbHelper _db;

        public StaffController(DbHelper db) => _db = db;

        bool IsStaff() => HttpContext.Session.GetInt32("StaffId") != null;

        // ── AUTH ──────────────────────────────────────────────
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand("SELECT password FROM staff WHERE staff_id=@id", con);
            cmd.Parameters.AddWithValue("@id", model.Id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read() && rdr.GetString(0) == model.Password.ToString())
            {
                HttpContext.Session.SetInt32("StaffId", model.Id);
                return RedirectToAction("Dashboard");
            }
            ViewBag.Error = "Invalid Staff ID or Password.";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("StaffId");
            return RedirectToAction("Index", "Home");
        }

        // ── DASHBOARD ─────────────────────────────────────────
        public IActionResult Dashboard()
        {
            if (!IsStaff()) return RedirectToAction("Login");

            using var con = _db.GetConnection();
            con.Open();

            var cmd1 = new MySqlCommand("SELECT COUNT(*) FROM matches", con);
            ViewBag.MatchCount = cmd1.ExecuteScalar();

            var cmd2 = new MySqlCommand("SELECT COUNT(*) FROM ticket", con);
            ViewBag.TicketCount = cmd2.ExecuteScalar();

            var cmd3 = new MySqlCommand("SELECT COUNT(*) FROM user", con);
            ViewBag.UserCount = cmd3.ExecuteScalar();

            var cmd4 = new MySqlCommand("SELECT COALESCE(SUM(total_payments),0) FROM ticket", con);
            ViewBag.TotalRevenue = cmd4.ExecuteScalar();

            return View();
        }

        // ── MATCHES ───────────────────────────────────────────
        public IActionResult Matches()
        {
            if (!IsStaff()) return RedirectToAction("Login");

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

        public IActionResult AddMatch() => IsStaff() ? View() : RedirectToAction("Login");

        [HttpPost]
        public IActionResult AddMatch(AddMatchViewModel model)
        {
            if (!IsStaff()) return RedirectToAction("Login");

            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO matches(match_id,match_name,series_tournament_name,match_format,match_date,match_time) VALUES(@id,@n,@s,@f,@d,@t)", con);
            cmd.Parameters.AddWithValue("@id", model.MatchId);
            cmd.Parameters.AddWithValue("@n", model.MatchName);
            cmd.Parameters.AddWithValue("@s", model.SeriesTournamentName);
            cmd.Parameters.AddWithValue("@f", model.MatchFormat);
            cmd.Parameters.AddWithValue("@d", model.MatchDate);
            cmd.Parameters.AddWithValue("@t", model.MatchTime);
            try
            {
                cmd.ExecuteNonQuery();
                TempData["Success"] = "Match added successfully!";
                return RedirectToAction("Matches");
            }
            catch
            {
                ViewBag.Error = "Match ID already exists or invalid data.";
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult RemoveMatch(int matchId)
        {
            if (!IsStaff()) return RedirectToAction("Login");

            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand("CALL delete_match(@mid)", con);
            cmd.Parameters.AddWithValue("@mid", matchId);
            cmd.ExecuteNonQuery();
            TempData["Success"] = $"Match #{matchId} removed.";
            return RedirectToAction("Matches");
        }

        // ── TICKETS ───────────────────────────────────────────
        public IActionResult Tickets()
        {
            if (!IsStaff()) return RedirectToAction("Login");

            var list = new List<Ticket>();
            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand(@"
                SELECT t.*, m.match_name, m.match_date, m.match_time, u.user_name
                FROM ticket t
                JOIN matches m ON t.match_id = m.match_id
                JOIN user u ON t.user_id = u.user_id
                ORDER BY t.ticket_id DESC", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                list.Add(new Ticket
                {
                    TicketId = rdr.GetInt32(0), MatchId = rdr.GetInt32(1), UserId = rdr.GetInt32(2),
                    Stand = rdr.GetString(3), TicketPrice = rdr.GetInt32(4),
                    NoOfTickets = rdr.GetInt32(5), TotalPayments = rdr.GetInt32(6),
                    PaymentMethod = rdr.GetString(7), MatchName = rdr.GetString(8),
                    MatchDate = rdr.GetString(9), MatchTime = rdr.GetString(10),
                    UserName = rdr.GetString(11)
                });
            return View(list);
        }

        // ── USERS ─────────────────────────────────────────────
        public IActionResult Users()
        {
            if (!IsStaff()) return RedirectToAction("Login");

            var list = new List<User>();
            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand("SELECT user_id, user_name, mobile_no FROM user ORDER BY user_id", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                list.Add(new User
                {
                    UserId = rdr.GetInt32(0),
                    UserName = rdr.GetString(1),
                    MobileNo = rdr.GetString(2)
                });
            return View(list);
        }

        public IActionResult UserDetail(int id)
        {
            if (!IsStaff()) return RedirectToAction("Login");

            using var con = _db.GetConnection();
            con.Open();
            var cmd = new MySqlCommand("SELECT user_id, user_name, mobile_no FROM user WHERE user_id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
                return View(new User { UserId = rdr.GetInt32(0), UserName = rdr.GetString(1), MobileNo = rdr.GetString(2) });
            return RedirectToAction("Users");
        }
    }
}
