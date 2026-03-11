using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using StadiumWeb.Models;

namespace StadiumWeb.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly TicketPdfService _pdfService;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, TicketPdfService pdfService)
        {
            _config = config;
            _logger = logger;
            _pdfService = pdfService;
        }

        // ── SMTP test (used by /User/TestEmail diagnostic) ───────────────
        public async Task<(bool Ok, string Error)> TestSmtpAsync(
            string host, string port, string user, string password)
        {
            try
            {
                using var client = new SmtpClient();
                client.Timeout = 10000;
                await client.ConnectAsync(host, int.Parse(port), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(user, password);

                // Send test email to self
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress("NM Stadium", user));
                msg.To.Add(new MailboxAddress("NM Stadium", user));
                msg.Subject = "✅ NM Stadium — SMTP Test OK";
                var bb = new BodyBuilder
                {
                    HtmlBody = "<h2 style='color:#4f0f1c'>SMTP is working!</h2>" +
                               "<p>Your Narendra Modi Stadium email delivery is configured correctly. " +
                               "Ticket confirmation emails will be sent after booking.</p>"
                };
                msg.Body = bb.ToMessageBody();
                await client.SendAsync(msg);
                await client.DisconnectAsync(true);
                return (true, "");
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null) msg += " → " + ex.InnerException.Message;
                return (false, msg);
            }
        }

        // ── Send ticket confirmation ──────────────────────────────────────
        public async Task SendTicketConfirmationAsync(string toEmail, string toName, Ticket ticket, Match match)
        {
            try
            {
                var standNames = new Dictionary<string, string>
                {
                    {"A", "Stand A — General (Open Seating)"},
                    {"B", "Stand B — Premium (Covered, Reserved)"},
                    {"C", "Stand C — Club (Cushioned Seats)"},
                    {"D", "Stand D — VIP (Lounge Access)"}
                };
                var standName = standNames.TryGetValue(ticket.Stand, out var sn) ? sn : $"Stand {ticket.Stand}";

                var html = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'/>
  <style>
    body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #f5f5f5; margin: 0; padding: 0; }}
    .wrapper {{ max-width: 600px; margin: 30px auto; background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,.1); }}
    .header {{ background: linear-gradient(135deg, #4f0f1c, #7b1c2e); padding: 30px 24px; text-align: center; }}
    .header h1 {{ color: #fff; margin: 0; font-size: 22px; letter-spacing: .05em; }}
    .header p {{ color: rgba(255,255,255,.7); margin: 6px 0 0; font-size: 13px; }}
    .badge {{ display: inline-block; background: #c9a84c; color: #4f0f1c; padding: 4px 14px; border-radius: 99px; font-size: 11px; font-weight: 700; letter-spacing: .1em; text-transform: uppercase; margin-top: 10px; }}
    .body {{ padding: 28px 24px; }}
    .greeting {{ font-size: 18px; font-weight: 700; color: #1a1a2e; margin-bottom: 6px; }}
    .subtext {{ color: #666; font-size: 14px; margin-bottom: 20px; }}
    .ticket-box {{ background: #0f2044; border-radius: 10px; padding: 20px 24px; color: #fff; margin-bottom: 20px; position: relative; overflow: hidden; }}
    .ticket-box::before {{ content: ''; position: absolute; right: -30px; top: -30px; width: 120px; height: 120px; border-radius: 50%; background: rgba(255,255,255,.04); }}
    .ticket-match {{ font-size: 22px; font-weight: 800; letter-spacing: .02em; margin-bottom: 4px; }}
    .ticket-series {{ font-size: 13px; color: rgba(255,255,255,.6); margin-bottom: 16px; }}
    .ticket-row {{ display: flex; justify-content: space-between; margin-bottom: 8px; font-size: 13px; }}
    .ticket-row span {{ color: rgba(255,255,255,.55); }}
    .ticket-row strong {{ color: #fff; }}
    .ticket-divider {{ border: none; border-top: 1px dashed rgba(255,255,255,.2); margin: 12px 0; }}
    .ticket-total {{ display: flex; justify-content: space-between; font-size: 16px; }}
    .ticket-total span {{ color: rgba(255,255,255,.7); }}
    .ticket-total strong {{ color: #c9a84c; font-size: 20px; }}
    .ticket-id {{ position: absolute; bottom: 12px; right: 16px; font-size: 10px; color: rgba(255,255,255,.3); font-family: monospace; }}
    .info-row {{ display: flex; gap: 10px; margin-bottom: 14px; }}
    .info-pill {{ flex: 1; background: #f7f3ee; border-radius: 8px; padding: 12px; text-align: center; }}
    .info-pill .label {{ font-size: 11px; color: #888; text-transform: uppercase; letter-spacing: .06em; }}
    .info-pill .value {{ font-size: 15px; font-weight: 700; color: #1a1a2e; margin-top: 3px; }}
    .footer {{ background: #faf8f3; padding: 18px 24px; text-align: center; border-top: 1px solid #eee; }}
    .footer p {{ margin: 0; font-size: 12px; color: #aaa; }}
    .footer a {{ color: #7b1c2e; text-decoration: none; font-weight: 600; }}
    .instructions {{ background: #e8f5e9; border-left: 4px solid #2e7d32; padding: 12px 16px; border-radius: 0 6px 6px 0; margin-bottom: 16px; font-size: 13px; color: #1b5e20; }}
  </style>
</head>
<body>
  <div class='wrapper'>
    <div class='header'>
      <h1>🏏 Narendra Modi Stadium</h1>
      <p>Motera, Ahmedabad · Capacity 1,32,000</p>
      <span class='badge'>✅ Booking Confirmed</span>
    </div>
    <div class='body'>
      <div class='greeting'>Hi {toName}!</div>
      <div class='subtext'>Your ticket booking is confirmed. Here are your booking details:</div>

      <div class='ticket-box'>
        <div class='ticket-match'>{match.MatchName}</div>
        <div class='ticket-series'>{match.SeriesTournamentName} &nbsp;·&nbsp; {match.MatchFormat}</div>
        <div class='ticket-row'><span>📅 Date</span><strong>{match.MatchDate}</strong></div>
        <div class='ticket-row'><span>🕐 Time</span><strong>{match.MatchTime}</strong></div>
        <div class='ticket-row'><span>🪑 Stand</span><strong>{standName}</strong></div>
        <div class='ticket-row'><span>🎫 Tickets</span><strong>{ticket.NoOfTickets} ticket(s)</strong></div>
        <div class='ticket-row'><span>💳 Payment</span><strong>{ticket.PaymentMethod}</strong></div>
        <div class='ticket-divider'/>
        <div class='ticket-total'><span>Total Paid</span><strong>₹{ticket.TotalPayments:N0}</strong></div>
        <div class='ticket-id'>Booking ID: #{ticket.TicketId}</div>
      </div>

      <div class='info-row'>
        <div class='info-pill'>
          <div class='label'>Price/Ticket</div>
          <div class='value'>₹{ticket.TicketPrice:N0}</div>
        </div>
        <div class='info-pill'>
          <div class='label'>Quantity</div>
          <div class='value'>{ticket.NoOfTickets}</div>
        </div>
        <div class='info-pill'>
          <div class='label'>Stand</div>
          <div class='value'>{ticket.Stand}</div>
        </div>
      </div>

      <div class='instructions'>
        📌 <strong>Instructions:</strong> Please carry a valid photo ID and arrive 45 minutes before the match starts. Electronic tickets on mobile will be accepted at entry gates.
      </div>
    </div>
    <div class='footer'>
      <p>Narendra Modi Stadium, Motera, Ahmedabad — 380005</p>
      <p style='margin-top:6px;'>Questions? Email <a href='mailto:support@nmstadium.in'>support@nmstadium.in</a></p>
    </div>
  </div>
</body>
</html>";

                // Generate PDF ticket
                ticket.UserName = toName;
                var pdfBytes = _pdfService.GenerateTicketPdf(ticket, match, toName);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _config["Smtp:FromName"] ?? "Narendra Modi Stadium",
                    _config["Smtp:UserName"]!));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = $"🎫 Booking Confirmed — {match.MatchName} | #{ticket.TicketId}";

                var bodyBuilder = new BodyBuilder { HtmlBody = html };
                // Attach PDF ticket
                bodyBuilder.Attachments.Add(
                    $"NMStadium_Ticket_{ticket.TicketId}.pdf",
                    pdfBytes,
                    new MimeKit.ContentType("application", "pdf"));
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _config["Smtp:Host"]!,
                    int.Parse(_config["Smtp:Port"] ?? "587"),
                    SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(
                    _config["Smtp:UserName"]!,
                    _config["Smtp:Password"]!);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("✅ Ticket confirmation email sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ TICKET EMAIL FAILED → To:{Email} | SMTP:{Host}:{Port} | Error: {Msg}",
                    toEmail,
                    _config["Smtp:Host"],
                    _config["Smtp:Port"],
                    ex.Message);
                throw; // re-throw so the controller can catch and inform the user
            }
        }

        // Send welcome email on registration
        public async Task SendWelcomeEmailAsync(string toEmail, string toName, int userId)
        {
            try
            {
                var html = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'/><style>
  body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #f5f5f5; margin: 0; padding: 0; }}
  .wrapper {{ max-width: 580px; margin: 30px auto; background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,.1); }}
  .header {{ background: linear-gradient(135deg, #4f0f1c, #7b1c2e); padding: 30px 24px; text-align: center; color: #fff; }}
  .header h1 {{ margin: 0; font-size: 22px; }}
  .header p {{ color: rgba(255,255,255,.7); margin: 6px 0 0; font-size: 13px; }}
  .body {{ padding: 28px 24px; }}
  .uid-box {{ background: #0f2044; border-radius: 8px; padding: 16px 20px; text-align: center; color: #fff; margin: 16px 0; }}
  .uid-box .label {{ font-size: 12px; color: rgba(255,255,255,.6); text-transform: uppercase; letter-spacing: .08em; }}
  .uid-box .value {{ font-size: 28px; font-weight: 800; color: #c9a84c; letter-spacing: .12em; margin-top: 4px; }}
  .footer {{ background: #faf8f3; padding: 18px 24px; text-align: center; border-top: 1px solid #eee; font-size: 12px; color: #aaa; }}
</style></head>
<body>
  <div class='wrapper'>
    <div class='header'>
      <h1>🏏 Welcome to NM Stadium!</h1>
      <p>Your fan account has been created</p>
    </div>
    <div class='body'>
      <p style='font-size:16px;font-weight:700;color:#1a1a2e;'>Hi {toName}! 👋</p>
      <p style='color:#555;font-size:14px;'>You have successfully registered on the Narendra Modi Stadium ticket booking portal. Save your User ID below — you'll need it to log in.</p>
      <div class='uid-box'>
        <div class='label'>Your User ID</div>
        <div class='value'>{userId}</div>
      </div>
      <p style='color:#555;font-size:13px;'>You can now browse upcoming matches and book tickets directly from the portal. We hope to see you at the stadium!</p>
    </div>
    <div class='footer'><p>Narendra Modi Stadium, Motera, Ahmedabad</p></div>
  </div>
</body></html>";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _config["Smtp:FromName"] ?? "Narendra Modi Stadium",
                    _config["Smtp:UserName"]!));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = "Welcome to Narendra Modi Stadium — Account Created!";

                var bodyBuilder = new BodyBuilder { HtmlBody = html };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _config["Smtp:Host"]!,
                    int.Parse(_config["Smtp:Port"] ?? "587"),
                    SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(
                    _config["Smtp:UserName"]!,
                    _config["Smtp:Password"]!);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
            }
        }
    }
}
