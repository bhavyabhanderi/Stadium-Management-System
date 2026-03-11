namespace StadiumWeb.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public string MatchName { get; set; } = "";
        public string SeriesTournamentName { get; set; } = "";
        public string MatchFormat { get; set; } = "";
        public string MatchDate { get; set; } = "";
        public string MatchTime { get; set; } = "";

        public string[] GetFlagUrls()
        {
            var name = MatchName.ToUpper();
            var flags = new Dictionary<string, string>
            {
                {"IND", "https://flagcdn.com/w80/in.png"},
                {"PAK", "https://flagcdn.com/w80/pk.png"},
                {"AUS", "https://flagcdn.com/w80/au.png"},
                {"ENG", "https://flagcdn.com/w80/gb-eng.png"},
                {"SA",  "https://flagcdn.com/w80/za.png"},
                {"NZ",  "https://flagcdn.com/w80/nz.png"},
                {"SL",  "https://flagcdn.com/w80/lk.png"},
                {"WI",  "https://flagcdn.com/w80/bb.png"},
                {"BAN", "https://flagcdn.com/w80/bd.png"},
                {"AFG", "https://flagcdn.com/w80/af.png"},
            };
            var result = new List<string>();
            foreach (var kv in flags)
            {
                if (name.Contains(kv.Key)) result.Add(kv.Value);
                if (result.Count == 2) break;
            }
            return result.ToArray();
        }

        public bool IsIPL()
        {
            var iplTeams = new[] { "MI", "CSK", "RCB", "KKR", "DC", "SRH", "RR", "PBKS", "LSG", "GT" };
            var name = MatchName.ToUpper();
            return iplTeams.Any(t => name.Contains(t));
        }

        public List<IplTeam> GetIplTeams()
        {
            var catalog = new Dictionary<string, IplTeam>
            {
                ["CSK"]  = new IplTeam("CSK",  "Chennai Super Kings",  "#F9CD1B", "#1A4B8C"),
                ["MI"]   = new IplTeam("MI",   "Mumbai Indians",        "#004BA0", "#D1AB3E"),
                ["RCB"]  = new IplTeam("RCB",  "Royal Challengers",     "#EC1C24", "#000000"),
                ["KKR"]  = new IplTeam("KKR",  "Kolkata Knight Riders", "#3A225D", "#B3A123"),
                ["DC"]   = new IplTeam("DC",   "Delhi Capitals",        "#0078BC", "#EF1C25"),
                ["SRH"]  = new IplTeam("SRH",  "Sunrisers Hyderabad",   "#F7A721", "#E8461B"),
                ["RR"]   = new IplTeam("RR",   "Rajasthan Royals",      "#EA1A85", "#254AA5"),
                ["PBKS"] = new IplTeam("PBKS", "Punjab Kings",          "#ED1B24", "#A7A9AC"),
                ["LSG"]  = new IplTeam("LSG",  "Lucknow Super Giants",  "#A72056", "#FEBE10"),
                ["GT"]   = new IplTeam("GT",   "Gujarat Titans",        "#1C1C6B", "#B8860B"),
            };
            var name = MatchName.ToUpper();
            var found = new List<IplTeam>();
            foreach (var kv in catalog)
                if (name.Contains(kv.Key)) { found.Add(kv.Value); if (found.Count == 2) break; }
            return found;
        }

        public string GetMatchBg()
        {
            if (IsIPL()) return "https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=600&q=75";
            return "https://images.unsplash.com/photo-1624526267942-ab0ff8a3e972?w=600&q=75";
        }
    }

    public class IplTeam
    {
        public string Code       { get; set; }
        public string FullName   { get; set; }
        public string PrimaryColor { get; set; }
        public string AccentColor  { get; set; }
        public IplTeam(string code, string full, string primary, string accent)
        { Code = code; FullName = full; PrimaryColor = primary; AccentColor = accent; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class Ticket
    {
        public int TicketId { get; set; }
        public int MatchId { get; set; }
        public int UserId { get; set; }
        public string Stand { get; set; } = "";
        public int TicketPrice { get; set; }
        public int NoOfTickets { get; set; }
        public int TotalPayments { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string? MatchName { get; set; }
        public string? MatchDate { get; set; }
        public string? MatchTime { get; set; }
        public string? UserName { get; set; }
        public string? MatchFormat { get; set; }
    }

    public class LoginViewModel
    {
        public int Id { get; set; }
        public string Password { get; set; } = "";
    }

    public class RegisterViewModel
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class BookTicketViewModel
    {
        public int MatchId { get; set; }
        public string Stand { get; set; } = "";
        public int NoOfTickets { get; set; }
        public string PaymentMethod { get; set; } = "";
        public int TicketPrice { get; set; }
        public int TotalAmount { get; set; }
    }

    public class AddMatchViewModel
    {
        public int MatchId { get; set; }
        public string MatchName { get; set; } = "";
        public string SeriesTournamentName { get; set; } = "";
        public string MatchFormat { get; set; } = "";
        public string MatchDate { get; set; } = "";
        public string MatchTime { get; set; } = "";
    }
}
