using MimeKit;
using MailKit.Security;

namespace StadiumWeb.Services
{
    public class OtpService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<OtpService> _logger;

        private static readonly Dictionary<string, (string Otp, DateTime Expiry)> _store = new();
        private static readonly object _lock = new();

        public OtpService(IConfiguration config, ILogger<OtpService> logger)
        {
            _config = config;
            _logger = logger;
        }

        private static string GenerateOtp() =>
            new Random().Next(100000, 999999).ToString("D6");

        // Returns (success, emailSentTo, errorMessage)
        public async Task<(bool Success, string? EmailSentTo, string? Error)> SendOtpAsync(
            string mobileNo, string email, string userName)
        {
            var otp    = GenerateOtp();
            var expiry = DateTime.UtcNow.AddMinutes(10);
            var key    = "otp:" + mobileNo;

            lock (_lock) { _store[key] = (otp, expiry); }

            // Always log — makes local dev easy
            _logger.LogWarning(
                "============================\n" +
                " OTP for {Mobile} : {Otp}\n" +
                "============================", mobileNo, otp);

            try
            {
                var html = BuildOtpEmail(toName: userName, mobileNo: mobileNo, otp: otp);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _config["Smtp:FromName"] ?? "NM Stadium",
                    _config["Smtp:UserName"]!));
                message.To.Add(new MailboxAddress(userName, email));
                message.Subject = $"Your OTP: {otp} — NM Stadium Registration";

                var builder = new BodyBuilder { HtmlBody = html };
                message.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(
                    _config["Smtp:Host"]!,
                    int.Parse(_config["Smtp:Port"] ?? "587"),
                    SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(
                    _config["Smtp:UserName"]!,
                    _config["Smtp:Password"]!);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("OTP email sent to {Email}", email);
                return (true, email, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OTP email failed for {Email}", email);
                return (false, null,
                    $"Email delivery failed. Check the console window for your OTP.");
            }
        }

        public bool VerifyOtp(string mobileNo, string entered)
        {
            var key = "otp:" + mobileNo;
            lock (_lock)
            {
                if (!_store.TryGetValue(key, out var entry)) return false;
                if (DateTime.UtcNow > entry.Expiry) { _store.Remove(key); return false; }
                if (entry.Otp != entered.Trim()) return false;
                _store.Remove(key);
                return true;
            }
        }

        private static string BuildOtpEmail(string toName, string mobileNo, string otp) => $@"
<!DOCTYPE html><html><head><meta charset='utf-8'/>
<style>
  body{{font-family:'Segoe UI',Arial,sans-serif;background:#f5f5f5;margin:0;padding:0;}}
  .wrap{{max-width:500px;margin:30px auto;background:#fff;border-radius:14px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.10);}}
  .hdr{{background:linear-gradient(135deg,#4f0f1c,#7b1c2e);padding:28px 24px;text-align:center;}}
  .hdr h2{{color:#fff;margin:0;font-size:20px;letter-spacing:.02em;}}
  .hdr p{{color:rgba(255,255,255,.6);margin:6px 0 0;font-size:12px;}}
  .body{{padding:30px 32px;}}
  .otp-box{{background:#0f2044;border-radius:12px;padding:24px;text-align:center;margin:22px 0;}}
  .otp-code{{font-size:44px;font-weight:900;letter-spacing:12px;color:#c9a84c;font-family:monospace;}}
  .otp-label{{color:rgba(255,255,255,.5);font-size:11px;margin-top:8px;letter-spacing:.12em;}}
  .note{{background:#fff8e1;border:1px solid #ffc107;border-radius:8px;padding:10px 14px;font-size:12px;color:#856404;margin-bottom:18px;}}
  .footer{{background:#faf8f3;padding:14px 24px;text-align:center;border-top:1px solid #eee;font-size:11px;color:#aaa;}}
</style></head><body>
<div class='wrap'>
  <div class='hdr'><h2>🏏 Narendra Modi Stadium</h2><p>Account Verification</p></div>
  <div class='body'>
    <p style='font-size:16px;font-weight:700;color:#1a1a2e;margin-bottom:6px;'>Hi {toName}!</p>
    <p style='color:#555;font-size:13px;margin-bottom:4px;'>
      Use the OTP below to verify mobile <strong>+91 {mobileNo}</strong> and complete your registration.
    </p>
    <div class='otp-box'>
      <div class='otp-code'>{otp}</div>
      <div class='otp-label'>ONE-TIME PASSWORD</div>
    </div>
    <div class='note'>⏱️ Valid for <strong>10 minutes</strong>. Do not share this OTP with anyone.</div>
    <p style='color:#aaa;font-size:11px;'>If you did not request this, please ignore this email.</p>
  </div>
  <div class='footer'>Narendra Modi Stadium · Motera, Ahmedabad 380005</div>
</div></body></html>";
    }
}
