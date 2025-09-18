using System;
using System.Text.RegularExpressions;

namespace UL
{
    public static class ValidationUtil
    {
        public const string PatientPattern = @"^(?=.*[A-Za-z])[A-Za-z\s'-]+$";
        public const string DrugPattern = @"^[A-Za-z0-9]+(\s[A-Za-z0-9]+)*$";
        public const string DosagePattern = @"^\d+(\.\d{1,4})?$";

        // IsValid Patient
        public static bool IsValidPatient(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 50)
                return false;

            var match = Regex.Match(input, PatientPattern);

            if (match.Success && (input.StartsWith(" ") || input.EndsWith(" ")))
                return false;

            return match.Success;
        }

        // IsValid Drug
        public static bool IsValidDrug(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 50)
                return false;

            return Regex.IsMatch(input, DrugPattern);
        }

        // IsValid Dosage
        public static bool IsValidDosage(decimal? input)
        {
            if (!input.HasValue || input <= 0)
                return false;

            string value = input.Value.ToString("0.####");
            return Regex.IsMatch(value, DosagePattern);
        }
    }
}
