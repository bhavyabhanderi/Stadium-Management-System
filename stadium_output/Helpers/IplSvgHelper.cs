namespace StadiumWeb.Helpers
{
    /// <summary>
    /// Provides detailed inline SVG logos for all 10 IPL teams.
    /// Each logo is a faithful artistic representation using the team's official colors.
    /// </summary>
    public static class IplSvgHelper
    {
        public static string GetLogo(string code) => code.ToUpper() switch
        {
            "CSK"  => CskLogo(),
            "MI"   => MiLogo(),
            "RCB"  => RcbLogo(),
            "KKR"  => KkrLogo(),
            "DC"   => DcLogo(),
            "SRH"  => SrhLogo(),
            "RR"   => RrLogo(),
            "PBKS" => PbksLogo(),
            "LSG"  => LsgLogo(),
            "GT"   => GtLogo(),
            _      => DefaultLogo(code)
        };

        // ── CSK — Yellow & Blue, Lion emblem ─────────────────────────────
        private static string CskLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#F9CD1B'/>
  <circle cx='35' cy='35' r='31' fill='#1A4B8C'/>
  <circle cx='35' cy='35' r='27' fill='#F9CD1B'/>
  <!-- Lion face -->
  <ellipse cx='35' cy='33' rx='13' ry='14' fill='#C8860A'/>
  <!-- Mane -->
  <circle cx='22' cy='30' r='6' fill='#8B5E00'/>
  <circle cx='26' cy='22' r='6' fill='#8B5E00'/>
  <circle cx='35' cy='19' r='6' fill='#8B5E00'/>
  <circle cx='44' cy='22' r='6' fill='#8B5E00'/>
  <circle cx='48' cy='30' r='6' fill='#8B5E00'/>
  <circle cx='46' cy='38' r='5' fill='#8B5E00'/>
  <circle cx='24' cy='38' r='5' fill='#8B5E00'/>
  <!-- Face -->
  <ellipse cx='35' cy='34' rx='11' ry='12' fill='#E8A020'/>
  <!-- Eyes -->
  <circle cx='30' cy='30' r='3.5' fill='white'/>
  <circle cx='40' cy='30' r='3.5' fill='white'/>
  <circle cx='31' cy='30' r='2' fill='#1A1A1A'/>
  <circle cx='41' cy='30' r='2' fill='#1A1A1A'/>
  <circle cx='31.5' cy='29.5' r='.7' fill='white'/>
  <circle cx='41.5' cy='29.5' r='.7' fill='white'/>
  <!-- Nose -->
  <ellipse cx='35' cy='36' rx='3' ry='2' fill='#8B3A00'/>
  <!-- Mouth -->
  <path d='M30 39 Q35 43 40 39' stroke='#5A1F00' stroke-width='1.5' fill='none' stroke-linecap='round'/>
  <!-- CSK text -->
  <text x='35' y='61' text-anchor='middle' font-size='8' font-weight='900' fill='#1A4B8C' font-family='Arial'>CSK</text>
</svg>";

        // ── MI — Blue & Gold, Rising Sun / Wave ──────────────────────────
        private static string MiLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#004BA0'/>
  <circle cx='35' cy='35' r='31' fill='#D1AB3E'/>
  <circle cx='35' cy='35' r='27' fill='#004BA0'/>
  <!-- Wave pattern -->
  <path d='M8 42 Q17 36 26 42 Q35 48 44 42 Q53 36 62 42' stroke='#D1AB3E' stroke-width='3' fill='none'/>
  <path d='M8 47 Q17 41 26 47 Q35 53 44 47 Q53 41 62 47' stroke='#D1AB3E' stroke-width='2' fill='none' opacity='.6'/>
  <!-- Rising sun -->
  <circle cx='35' cy='28' r='10' fill='#D1AB3E'/>
  <!-- Sun rays -->
  <line x1='35' y1='14' x2='35' y2='11' stroke='#D1AB3E' stroke-width='2.5' stroke-linecap='round'/>
  <line x1='45' y1='17' x2='47' y2='15' stroke='#D1AB3E' stroke-width='2.5' stroke-linecap='round'/>
  <line x1='49' y1='28' x2='52' y2='28' stroke='#D1AB3E' stroke-width='2.5' stroke-linecap='round'/>
  <line x1='25' y1='17' x2='23' y2='15' stroke='#D1AB3E' stroke-width='2.5' stroke-linecap='round'/>
  <line x1='21' y1='28' x2='18' y2='28' stroke='#D1AB3E' stroke-width='2.5' stroke-linecap='round'/>
  <!-- MI inside sun -->
  <text x='35' y='32' text-anchor='middle' font-size='8' font-weight='900' fill='#004BA0' font-family='Arial'>MI</text>
  <!-- Bottom text -->
  <text x='35' y='61' text-anchor='middle' font-size='7' font-weight='900' fill='#D1AB3E' font-family='Arial'>MUMBAI</text>
</svg>";

        // ── RCB — Red & Black, Lion shield ───────────────────────────────
        private static string RcbLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#EC1C24'/>
  <circle cx='35' cy='35' r='31' fill='#111111'/>
  <!-- Shield -->
  <path d='M35 12 L54 20 L54 38 Q54 52 35 60 Q16 52 16 38 L16 20 Z' fill='#EC1C24'/>
  <path d='M35 16 L51 23 L51 38 Q51 50 35 57 Q19 50 19 38 L19 23 Z' fill='#8B0000'/>
  <!-- Gold cross/stripe -->
  <rect x='32' y='16' width='6' height='41' fill='#C9A84C' rx='1'/>
  <rect x='19' y='30' width='32' height='6' fill='#C9A84C' rx='1'/>
  <!-- RCB letters in quadrants -->
  <text x='27' y='28' text-anchor='middle' font-size='7' font-weight='900' fill='white' font-family='Arial'>R</text>
  <text x='43' y='28' text-anchor='middle' font-size='7' font-weight='900' fill='white' font-family='Arial'>C</text>
  <text x='35' y='50' text-anchor='middle' font-size='7' font-weight='900' fill='white' font-family='Arial'>B</text>
</svg>";

        // ── KKR — Purple & Gold, Knight on horse ─────────────────────────
        private static string KkrLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#3A225D'/>
  <circle cx='35' cy='35' r='31' fill='#B3A123'/>
  <circle cx='35' cy='35' r='27' fill='#3A225D'/>
  <!-- Knight helmet -->
  <ellipse cx='35' cy='28' rx='12' ry='13' fill='#B3A123'/>
  <rect x='29' y='33' width='12' height='8' rx='2' fill='#8A7A18'/>
  <!-- Visor slits -->
  <rect x='30' y='35' width='10' height='1.5' rx='.75' fill='#3A225D'/>
  <rect x='30' y='38' width='10' height='1.5' rx='.75' fill='#3A225D'/>
  <!-- Plume -->
  <path d='M35 15 Q30 18 28 22 Q32 20 35 22 Q38 20 42 22 Q40 18 35 15Z' fill='#EC1C24'/>
  <!-- KKR text -->
  <text x='35' y='56' text-anchor='middle' font-size='8.5' font-weight='900' fill='#B3A123' font-family='Arial'>KKR</text>
  <!-- Stars -->
  <text x='22' y='48' font-size='7' fill='#B3A123'>★</text>
  <text x='45' y='48' font-size='7' fill='#B3A123'>★</text>
</svg>";

        // ── DC — Blue & Red, Capital D ────────────────────────────────────
        private static string DcLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#0078BC'/>
  <circle cx='35' cy='35' r='31' fill='#EF1C25'/>
  <circle cx='35' cy='35' r='27' fill='#0078BC'/>
  <!-- Large D shape -->
  <path d='M22 17 L22 53 L35 53 Q52 53 52 35 Q52 17 35 17 Z' fill='white'/>
  <path d='M27 22 L27 48 L34 48 Q46 48 46 35 Q46 22 34 22 Z' fill='#0078BC'/>
  <!-- Red accent line -->
  <line x1='22' y1='17' x2='22' y2='53' stroke='#EF1C25' stroke-width='4'/>
  <!-- DC text -->
  <text x='37' y='39' text-anchor='middle' font-size='9' font-weight='900' fill='white' font-family='Arial'>DC</text>
  <!-- Bottom badge -->
  <path d='M20 57 Q35 63 50 57' stroke='#EF1C25' stroke-width='2.5' fill='none' stroke-linecap='round'/>
</svg>";

        // ── SRH — Orange & Black, Sun/Fire ───────────────────────────────
        private static string SrhLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#F7A721'/>
  <circle cx='35' cy='35' r='31' fill='#1A1A1A'/>
  <circle cx='35' cy='35' r='27' fill='#E8461B'/>
  <!-- Sun burst rays -->
  <circle cx='35' cy='35' r='12' fill='#F7A721'/>
  <path d='M35 8 L37 21 L35 23 L33 21 Z' fill='#F7A721'/>
  <path d='M62 35 L49 37 L47 35 L49 33 Z' fill='#F7A721'/>
  <path d='M35 62 L33 49 L35 47 L37 49 Z' fill='#F7A721'/>
  <path d='M8 35 L21 33 L23 35 L21 37 Z' fill='#F7A721'/>
  <path d='M54 16 L44 25 L42 23 L51 14 Z' fill='#F7A721'/>
  <path d='M54 54 L45 45 L47 43 L56 52 Z' fill='#F7A721'/>
  <path d='M16 54 L25 45 L23 43 L14 52 Z' fill='#F7A721'/>
  <path d='M16 16 L25 25 L23 27 L14 18 Z' fill='#F7A721'/>
  <!-- SRH text in sun -->
  <text x='35' y='39' text-anchor='middle' font-size='8.5' font-weight='900' fill='#1A1A1A' font-family='Arial'>SRH</text>
</svg>";

        // ── RR — Pink & Blue, Royal crown ────────────────────────────────
        private static string RrLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#EA1A85'/>
  <circle cx='35' cy='35' r='31' fill='#254AA5'/>
  <circle cx='35' cy='35' r='27' fill='#EA1A85'/>
  <!-- Crown -->
  <path d='M15 45 L15 32 L23 40 L35 22 L47 40 L55 32 L55 45 Z' fill='#FFD700'/>
  <path d='M15 45 L55 45 L53 52 L17 52 Z' fill='#FFD700'/>
  <!-- Crown gems -->
  <circle cx='35' cy='22' r='3.5' fill='#EA1A85'/>
  <circle cx='23' cy='40' r='3' fill='#254AA5'/>
  <circle cx='47' cy='40' r='3' fill='#254AA5'/>
  <!-- Band dots -->
  <circle cx='26' cy='48.5' r='2' fill='#EA1A85'/>
  <circle cx='35' cy='48.5' r='2' fill='#EA1A85'/>
  <circle cx='44' cy='48.5' r='2' fill='#EA1A85'/>
  <!-- RR text -->
  <text x='35' y='62' text-anchor='middle' font-size='8' font-weight='900' fill='#254AA5' font-family='Arial'>RR</text>
</svg>";

        // ── PBKS — Red & Silver, Lion / Punjab ───────────────────────────
        private static string PbksLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#ED1B24'/>
  <circle cx='35' cy='35' r='31' fill='#A7A9AC'/>
  <circle cx='35' cy='35' r='27' fill='#ED1B24'/>
  <!-- Lion silhouette stylized -->
  <ellipse cx='35' cy='32' rx='11' ry='12' fill='#FFD700'/>
  <!-- Mane spikes -->
  <polygon points='35,17 37,22 35,20 33,22' fill='#C8860A'/>
  <polygon points='43,20 44,26 41,23 42,21' fill='#C8860A'/>
  <polygon points='47,28 46,33 43,30 45,29' fill='#C8860A'/>
  <polygon points='27,20 26,26 29,23 28,21' fill='#C8860A'/>
  <polygon points='23,28 24,33 27,30 25,29' fill='#C8860A'/>
  <!-- Face -->
  <ellipse cx='35' cy='33' rx='9' ry='10' fill='#E8A020'/>
  <circle cx='31' cy='29.5' r='2.5' fill='#1A1A1A'/>
  <circle cx='39' cy='29.5' r='2.5' fill='#1A1A1A'/>
  <circle cx='31.5' cy='29' r='.8' fill='white'/>
  <circle cx='39.5' cy='29' r='.8' fill='white'/>
  <ellipse cx='35' cy='35' rx='2.5' ry='1.8' fill='#8B3A00'/>
  <path d='M30 38 Q35 41.5 40 38' stroke='#5A1F00' stroke-width='1.2' fill='none'/>
  <!-- PBKS text -->
  <text x='35' y='59' text-anchor='middle' font-size='7' font-weight='900' fill='#A7A9AC' font-family='Arial'>PBKS</text>
</svg>";

        // ── LSG — Teal & Gold, Superman inspired ─────────────────────────
        private static string LsgLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#A72056'/>
  <circle cx='35' cy='35' r='31' fill='#FEBE10'/>
  <circle cx='35' cy='35' r='27' fill='#A72056'/>
  <!-- Diamond / shield shape -->
  <path d='M35 14 L54 35 L35 56 L16 35 Z' fill='#FEBE10'/>
  <path d='M35 19 L50 35 L35 51 L20 35 Z' fill='#A72056'/>
  <!-- L & G stylized -->
  <text x='29' y='39' text-anchor='middle' font-size='10' font-weight='900' fill='#FEBE10' font-family='Arial'>L</text>
  <text x='41' y='39' text-anchor='middle' font-size='10' font-weight='900' fill='#FEBE10' font-family='Arial'>G</text>
  <!-- Outer diamonds -->
  <polygon points='35,8 38,14 35,11 32,14' fill='#FEBE10'/>
  <polygon points='35,62 38,56 35,59 32,56' fill='#FEBE10'/>
</svg>";

        // ── GT — Navy & Gold, Titan shield ───────────────────────────────
        private static string GtLogo() => @"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#1C1C6B'/>
  <circle cx='35' cy='35' r='31' fill='#B8860B'/>
  <circle cx='35' cy='35' r='27' fill='#1C1C6B'/>
  <!-- Titan shield -->
  <path d='M35 13 L53 21 L53 40 Q53 54 35 61 Q17 54 17 40 L17 21 Z' fill='#B8860B'/>
  <path d='M35 17 L50 24 L50 40 Q50 52 35 58 Q20 52 20 40 L20 24 Z' fill='#1C1C6B'/>
  <!-- GT large letters -->
  <text x='35' y='44' text-anchor='middle' font-size='18' font-weight='900' fill='#B8860B' font-family='Arial' letter-spacing='-1'>GT</text>
  <!-- Top star -->
  <polygon points='35,18 36.5,22 40.5,22 37.5,24.5 38.5,28.5 35,26 31.5,28.5 32.5,24.5 29.5,22 33.5,22' fill='#B8860B'/>
</svg>";

        // ── Fallback for unknown teams ────────────────────────────────────
        private static string DefaultLogo(string code) => $@"<svg viewBox='0 0 70 70' xmlns='http://www.w3.org/2000/svg'>
  <circle cx='35' cy='35' r='35' fill='#1c1c2e'/>
  <circle cx='35' cy='35' r='28' fill='none' stroke='#c9a84c' stroke-width='3'/>
  <text x='35' y='41' text-anchor='middle' font-size='14' font-weight='900' fill='#c9a84c' font-family='Arial'>{code}</text>
</svg>";
    }
}
