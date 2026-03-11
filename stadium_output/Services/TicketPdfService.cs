using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Geom;
using StadiumWeb.Models;

namespace StadiumWeb.Services
{
    public class TicketPdfService
    {
        // Brand colours
        private static readonly DeviceRgb Maroon    = new(0x4f, 0x0f, 0x1c);
        private static readonly DeviceRgb DarkBlue  = new(0x0f, 0x20, 0x44);
        private static readonly DeviceRgb Gold      = new(0xc9, 0xa8, 0x4c);
        private static readonly DeviceRgb LightGrey = new(0xf5, 0xf5, 0xf5);
        private static readonly DeviceRgb MidGrey   = new(0x88, 0x88, 0x88);
        private static readonly DeviceRgb GreenBg   = new(0xe8, 0xf5, 0xe9);
        private static readonly DeviceRgb GreenDark = new(0x1b, 0x5e, 0x20);
        private static readonly DeviceRgb GreenBdr  = new(0x2e, 0x7d, 0x32);
        private static readonly DeviceRgb White     = new(0xff, 0xff, 0xff);
        private static readonly DeviceRgb Grey77    = new(0x77, 0x77, 0x77);
        private static readonly DeviceRgb GreyAA    = new(0xaa, 0xaa, 0xaa);
        private static readonly DeviceRgb GreyCC    = new(0xcc, 0xcc, 0xcc);
        private static readonly DeviceRgb GreyEE    = new(0xee, 0xee, 0xee);
        private static readonly DeviceRgb GreyF8    = new(0xf8, 0xf8, 0xf8);

        private static readonly Border NoBorder = Border.NO_BORDER;
        private static readonly BorderRadius R8  = new(8);
        private static readonly BorderRadius R10 = new(10);
        private static readonly BorderRadius R20 = new(20);

        public byte[] GenerateTicketPdf(Ticket ticket, Match match, string userName)
        {
            var standNames = new Dictionary<string, string>
            {
                { "A", "Stand A - General (Open Seating)" },
                { "B", "Stand B - Premium (Covered, Reserved)" },
                { "C", "Stand C - Club (Cushioned Seats)" },
                { "D", "Stand D - VIP (Lounge Access)" }
            };
            var standName = standNames.TryGetValue(ticket.Stand, out var sn)
                ? sn : "Stand " + ticket.Stand;

            using var ms  = new MemoryStream();
            var writer    = new PdfWriter(ms);
            var pdf       = new PdfDocument(writer);
            var doc       = new Document(pdf, PageSize.A4);
            doc.SetMargins(0, 0, 0, 0);

            var bold    = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var oblique = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

            // ── HEADER ────────────────────────────────────────────────────
            var headerTbl = new Table(1).UseAllAvailableWidth();
            var hCell = new Cell()
                .SetBackgroundColor(Maroon).SetBorder(NoBorder)
                .SetPaddingTop(28).SetPaddingBottom(22)
                .SetPaddingLeft(40).SetPaddingRight(40);

            hCell.Add(new Paragraph("Narendra Modi Stadium")
                .SetFont(bold).SetFontSize(24).SetFontColor(White)
                .SetTextAlignment(TextAlignment.CENTER).SetMarginBottom(5));

            hCell.Add(new Paragraph("Motera, Ahmedabad  |  Capacity 1,32,000")
                .SetFont(regular).SetFontSize(11).SetFontColor(GreyCC)
                .SetTextAlignment(TextAlignment.CENTER).SetMarginBottom(16));

            // Badge as a 1-cell table (Paragraph doesn't support BorderRadius in iText7)
            var badgeTbl = new Table(1)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetBackgroundColor(Gold)
                .SetBorderRadius(R20);
            badgeTbl.AddCell(new Cell().SetBorder(NoBorder)
                .SetBackgroundColor(Gold).SetBorderRadius(R20)
                .SetPaddingTop(5).SetPaddingBottom(5)
                .SetPaddingLeft(20).SetPaddingRight(20)
                .Add(new Paragraph("BOOKING CONFIRMED")
                    .SetFont(bold).SetFontSize(10).SetFontColor(Maroon)
                    .SetTextAlignment(TextAlignment.CENTER).SetMarginBottom(0)));
            hCell.Add(badgeTbl);

            headerTbl.AddCell(hCell);
            doc.Add(headerTbl);

            // Gold accent bar
            var accentTbl = new Table(1).UseAllAvailableWidth();
            accentTbl.AddCell(new Cell()
                .SetBackgroundColor(Gold).SetBorder(NoBorder).SetHeight(5));
            doc.Add(accentTbl);

            // ── BODY ──────────────────────────────────────────────────────
            var body = new Div()
                .SetPaddingLeft(40).SetPaddingRight(40)
                .SetPaddingTop(26).SetPaddingBottom(10);

            body.Add(new Paragraph(match.MatchName)
                .SetFont(bold).SetFontSize(22).SetFontColor(DarkBlue).SetMarginBottom(4));
            body.Add(new Paragraph(match.SeriesTournamentName + "  |  " + match.MatchFormat)
                .SetFont(regular).SetFontSize(11).SetFontColor(MidGrey).SetMarginBottom(18));

            // ── Info pills ────────────────────────────────────────────────
            var pillTbl = new Table(new float[] { 1, 1, 1 })
                .UseAllAvailableWidth().SetMarginBottom(18);

            void AddPill(string label, string value)
            {
                var pc = new Cell()
                    .SetBackgroundColor(LightGrey).SetBorder(NoBorder)
                    .SetBorderRadius(R8)
                    .SetPaddingTop(12).SetPaddingBottom(12)
                    .SetPaddingLeft(10).SetPaddingRight(10)
                    .SetTextAlignment(TextAlignment.CENTER);
                pc.Add(new Paragraph(label.ToUpper())
                    .SetFont(regular).SetFontSize(8).SetFontColor(MidGrey)
                    .SetMarginBottom(3).SetCharacterSpacing(0.6f));
                pc.Add(new Paragraph(value)
                    .SetFont(bold).SetFontSize(12).SetFontColor(DarkBlue).SetMarginBottom(0));
                pillTbl.AddCell(pc);
            }

            AddPill("Date",   match.MatchDate);
            AddPill("Time",   match.MatchTime);
            AddPill("Format", match.MatchFormat);
            body.Add(pillTbl);

            // ── Stand / Tickets dark strip ────────────────────────────────
            var stripTbl = new Table(new float[] { 1, 1 })
                .UseAllAvailableWidth()
                .SetBackgroundColor(DarkBlue)
                .SetBorderRadius(R10)
                .SetMarginBottom(18);

            var sc1 = new Cell().SetBorder(NoBorder)
                .SetBackgroundColor(DarkBlue)
                .SetPaddingTop(16).SetPaddingBottom(16).SetPaddingLeft(18);
            sc1.Add(new Paragraph("STAND")
                .SetFont(regular).SetFontSize(8).SetFontColor(GreyAA)
                .SetCharacterSpacing(1).SetMarginBottom(3));
            sc1.Add(new Paragraph(standName)
                .SetFont(bold).SetFontSize(12).SetFontColor(White).SetMarginBottom(0));
            stripTbl.AddCell(sc1);

            var sc2 = new Cell().SetBorder(NoBorder)
                .SetBackgroundColor(DarkBlue)
                .SetPaddingTop(16).SetPaddingBottom(16).SetPaddingRight(18)
                .SetTextAlignment(TextAlignment.RIGHT);
            sc2.Add(new Paragraph("TICKETS")
                .SetFont(regular).SetFontSize(8).SetFontColor(GreyAA)
                .SetCharacterSpacing(1).SetMarginBottom(3));
            sc2.Add(new Paragraph(ticket.NoOfTickets + " x Rs." + ticket.TicketPrice.ToString("N0"))
                .SetFont(bold).SetFontSize(12).SetFontColor(White).SetMarginBottom(0));
            stripTbl.AddCell(sc2);
            body.Add(stripTbl);

            // ── Booking details grid ──────────────────────────────────────
            var detTbl = new Table(new float[] { 1, 1 })
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(GreyEE, 1))
                .SetBorderRadius(R8)
                .SetMarginBottom(18);

            void AddDetailRow(string l1, string v1, string l2, string v2, bool shade)
            {
                var bg = shade ? LightGrey : White;

                var dc1 = new Cell().SetBorder(NoBorder).SetBackgroundColor(bg)
                    .SetPaddingLeft(16).SetPaddingTop(10).SetPaddingBottom(10).SetPaddingRight(8);
                dc1.Add(new Paragraph(l1).SetFont(regular).SetFontSize(9)
                    .SetFontColor(MidGrey).SetMarginBottom(2));
                dc1.Add(new Paragraph(v1).SetFont(bold).SetFontSize(12)
                    .SetFontColor(DarkBlue).SetMarginBottom(0));

                var dc2 = new Cell().SetBorder(NoBorder).SetBackgroundColor(bg)
                    .SetPaddingLeft(16).SetPaddingTop(10).SetPaddingBottom(10).SetPaddingRight(8);
                dc2.Add(new Paragraph(l2).SetFont(regular).SetFontSize(9)
                    .SetFontColor(MidGrey).SetMarginBottom(2));
                dc2.Add(new Paragraph(v2).SetFont(bold).SetFontSize(12)
                    .SetFontColor(DarkBlue).SetMarginBottom(0));

                detTbl.AddCell(dc1);
                detTbl.AddCell(dc2);
            }

            AddDetailRow("Booking ID",       "#" + ticket.TicketId,
                         "Ticket Holder",    userName,              false);
            AddDetailRow("User ID",           ticket.UserId.ToString(),
                         "Payment Method",   ticket.PaymentMethod,  true);
            AddDetailRow("Price per Ticket", "Rs." + ticket.TicketPrice.ToString("N0"),
                         "Quantity",          ticket.NoOfTickets + " ticket(s)", false);
            body.Add(detTbl);

            // ── Total paid ────────────────────────────────────────────────
            var totTbl = new Table(new float[] { 1, 1 })
                .UseAllAvailableWidth()
                .SetBackgroundColor(DarkBlue)
                .SetBorderRadius(R10)
                .SetMarginBottom(18);

            var tc1 = new Cell().SetBorder(NoBorder).SetBackgroundColor(DarkBlue)
                .SetPaddingTop(18).SetPaddingBottom(18).SetPaddingLeft(20)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            tc1.Add(new Paragraph("TOTAL AMOUNT PAID")
                .SetFont(bold).SetFontSize(10).SetFontColor(GreyAA)
                .SetCharacterSpacing(1).SetMarginBottom(3));
            tc1.Add(new Paragraph("Inclusive of all taxes")
                .SetFont(oblique).SetFontSize(9).SetFontColor(Grey77));
            totTbl.AddCell(tc1);

            var tc2 = new Cell().SetBorder(NoBorder).SetBackgroundColor(DarkBlue)
                .SetPaddingTop(18).SetPaddingBottom(18).SetPaddingRight(20)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            tc2.Add(new Paragraph("Rs." + ticket.TotalPayments.ToString("N0"))
                .SetFont(bold).SetFontSize(28).SetFontColor(Gold).SetMarginBottom(0));
            totTbl.AddCell(tc2);
            body.Add(totTbl);

            // ── Instructions (left-border via table trick) ────────────────
            // Use a 2-col table: narrow green left-border cell + content cell
            var instrTbl = new Table(new float[] { 5, 1 })
                .UseAllAvailableWidth()
                .SetBorderRadius(R8)
                .SetMarginBottom(18);

            var instrBdrCell = new Cell().SetBorder(NoBorder)
                .SetBackgroundColor(GreenBdr)
                .SetPaddingLeft(0).SetPaddingRight(0);
            instrTbl.AddCell(instrBdrCell);

            var instrContent = new Cell().SetBorder(NoBorder)
                .SetBackgroundColor(GreenBg)
                .SetPaddingTop(14).SetPaddingBottom(14)
                .SetPaddingLeft(16).SetPaddingRight(16);
            instrContent.Add(new Paragraph("IMPORTANT INSTRUCTIONS")
                .SetFont(bold).SetFontSize(10).SetFontColor(GreenBdr)
                .SetMarginBottom(8).SetCharacterSpacing(0.5f));

            string[] instrs = {
                "Carry a valid photo ID: Aadhaar / PAN / Passport / Driving Licence.",
                "Arrive at least 45 minutes before match start time.",
                "Electronic tickets on mobile will be accepted at entry gates.",
                "Outside food and beverages are not permitted inside the stadium.",
                "This ticket is non-transferable and non-refundable."
            };
            foreach (var instr in instrs)
            {
                instrContent.Add(new Paragraph("• " + instr)
                    .SetFont(regular).SetFontSize(9).SetFontColor(GreenDark)
                    .SetMarginBottom(3));
            }
            instrTbl.AddCell(instrContent);
            body.Add(instrTbl);

            // ── Barcode strip ─────────────────────────────────────────────
            var barcodeTbl = new Table(1).UseAllAvailableWidth().SetMarginBottom(6);
            var barcodeCell = new Cell().SetBorder(new SolidBorder(GreyEE, 1))
                .SetBackgroundColor(GreyF8)
                .SetBorderRadius(R8)
                .SetPaddingTop(12).SetPaddingBottom(8)
                .SetPaddingLeft(14).SetPaddingRight(14)
                .SetTextAlignment(TextAlignment.CENTER);

            var rnd  = new Random(ticket.TicketId);
            var bars = new System.Text.StringBuilder();
            for (int i = 0; i < 55; i++)
            {
                int w = rnd.Next(1, 4);
                for (int j = 0; j < w; j++) bars.Append('|');
                for (int j = 0; j < rnd.Next(1, 3); j++) bars.Append(' ');
            }
            barcodeCell.Add(new Paragraph(bars.ToString())
                .SetFont(regular).SetFontSize(18).SetFontColor(DarkBlue)
                .SetCharacterSpacing(-1.5f).SetMarginBottom(4));
            barcodeCell.Add(new Paragraph(
                    "NMSTADIUM-" + ticket.TicketId.ToString("D6") +
                    "-" + match.MatchId.ToString("D4") +
                    "-" + ticket.Stand + "-" + ticket.NoOfTickets.ToString("D2"))
                .SetFont(regular).SetFontSize(8).SetFontColor(MidGrey));
            barcodeTbl.AddCell(barcodeCell);
            body.Add(barcodeTbl);

            doc.Add(body);

            // ── FOOTER ────────────────────────────────────────────────────
            var footerTbl = new Table(1).UseAllAvailableWidth();
            var fCell = new Cell()
                .SetBackgroundColor(Maroon).SetBorder(NoBorder)
                .SetPaddingTop(16).SetPaddingBottom(16)
                .SetTextAlignment(TextAlignment.CENTER);
            fCell.Add(new Paragraph(
                    "Narendra Modi Stadium, Motera, Ahmedabad 380005  |  support@nmstadium.in")
                .SetFont(regular).SetFontSize(9).SetFontColor(GreyCC).SetMarginBottom(4));
            fCell.Add(new Paragraph("This is a computer-generated ticket. No signature required.")
                .SetFont(oblique).SetFontSize(8).SetFontColor(Gold));
            footerTbl.AddCell(fCell);
            doc.Add(footerTbl);

            doc.Close();
            return ms.ToArray();
        }
    }
}
