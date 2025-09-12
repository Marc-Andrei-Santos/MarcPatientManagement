using System;
using System.Text.RegularExpressions;

namespace UL
{
    public static class ValidationUtil
    {
        // Patient Names (letters, spaces, hyphen, apostrophe, max 50 chars, must have at least 1 letter)
        public const string PatientPattern = @"^(?=.*[A-Za-z])[A-Za-z\s'-]+$";

        // Drug Names (alphanumeric + space, max 50 chars)
        public const string DrugPattern = @"^[A-Za-z0-9]+(\s[A-Za-z0-9]+)*$";

        // Dosage (numeric > 0, up to 4 decimal places)
        public const string DosagePattern = @"^\d+(\.\d{1,4})?$";

        public static bool IsValidPatient(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 50)
                return false;

            // Regex match
            var match = Regex.Match(input, PatientPattern);

            // Reject leading/trailing spaces
            if (match.Success && (input.StartsWith(" ") || input.EndsWith(" ")))
                return false;

            return match.Success;
        }

        public static bool IsValidDrug(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 50)
                return false;

            return Regex.IsMatch(input, DrugPattern);
        }

        public static bool IsValidDosage(decimal? input)
        {
            if (!input.HasValue || input <= 0)
                return false;

            // Convert decimal to string with up to 4 decimals to match pattern
            string value = input.Value.ToString("0.####");
            return Regex.IsMatch(value, DosagePattern);
        }
    }
}
