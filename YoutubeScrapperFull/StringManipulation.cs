using System.Linq;
using System.Text.RegularExpressions;

namespace YoutubeScrapperFull
{
    class StringManipulation
    {

        static public MatchCollection SearchEmailFromString0(string txt)
        {
            const string RegexPattern =
           @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
             + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
            MatchCollection matches = Regex.Matches(txt, RegexPattern, RegexOptions.IgnoreCase);
            return matches;
        }
        static public MatchCollection SearchEmailFromString1(string txt)
        {
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
            MatchCollection matches = emailRegex.Matches(txt);
            return matches;
        }
        static public MatchCollection SearchEmailFromString2(string txt)
        {
            string RegexPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z]+(\.[a-zA-Z0-9]+)+";
            MatchCollection matches = Regex.Matches(txt, RegexPattern, RegexOptions.IgnoreCase);
            return matches;
        }
        static public MatchCollection SearchEmailFromString3(string txt)
        {
            string RegexPattern = @"\b[A-Z0-9._-]+@[A-Z0-9][A-Z0-9.-]{0,61}[A-Z0-9]\.[A-Z.]{2,6}\b";
            MatchCollection matches = Regex.Matches(txt, RegexPattern, RegexOptions.IgnoreCase);
            return matches;
        }
        

        static public string SearchEmail(string content)
        {
            MatchCollection emails;
            emails = SearchEmailFromString0(content);
            if (emails.Count == 0)
            {
                emails = SearchEmailFromString1(content);
            }
            else if (emails.Count == 0)
            {
                emails = SearchEmailFromString2(content);
            }
            else if (emails.Count == 0)
            {
                emails = SearchEmailFromString3(content);
            }

            string email = string.Empty;
            foreach (Match match in emails)
            {
                if (!email.Contains(match.Value))
                {
                    email = email + "\n" + match.Value;
                }
            }
            if (email.Count() > 2)
            {
                email = email.Remove(0, 1);
            }
            if (email.Contains(".png"))
                return null;
            return email;
        }


        static public string SearchSocialLinks(string content)
        {
            MatchCollection mc = Regex.Matches(content, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
            
            string sLinks = string.Empty;
            foreach (Match link in mc)
            {
                if (!sLinks.Contains(link.Value))
                {
                    link.Value.Replace(',', ' ');
                    sLinks = sLinks + "\n" + link.Value;
                }
            }
            if (sLinks.Count() > 2)
            {
                sLinks = sLinks.Remove(0, 1);
            }
            return sLinks;
        }
    }
}
