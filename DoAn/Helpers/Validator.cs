using System.Text.RegularExpressions;

namespace DoAn.Helpers
{
    public class Validator
    {
        public static bool IsValidGmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Regex chỉ chấp nhận dạng ...@gmail.com
            string pattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
            return Regex.IsMatch(email, pattern);
        }
    }
}
