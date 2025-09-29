using System;
using System.Text.RegularExpressions;

namespace UL
{
    public static class ValidationUtil
    {
        private const string PatientPattern = @"^[A-Za-z' -]+$";
        private const string DrugPattern = @"^[A-Za-z0-9 ]+$";
        private const string DosagePattern = @"^(?:0\.\d{1,4}|[1-9]\d{0,2}(?:\.\d{1,4})?)$";

        //  IsValid Patient
        public static bool IsValidPatient(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            if (input.Length == 0 || input.Length > 50)
                return false;

            while (input.Contains("  "))
                input = input.Replace("  ", " ");

            return Regex.IsMatch(input, PatientPattern);
        }

        //  IsValid Drug
        public static bool IsValidDrug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            if (input.Length == 0 || input.Length > 50)
                return false;

            while (input.Contains("  "))
                input = input.Replace("  ", " ");

            return Regex.IsMatch(input, DrugPattern);
        }

        //  IsValid Dosage
        public static bool IsValidDosage(decimal? input)
        {
            if (!input.HasValue)
                return false;

            decimal value = input.Value;

            if (value <= 0 || value < 0.0001m || value > 999.9999m)
                return false;

            if (value != Math.Round(value, 4))
                return false;

            string strValue = value.ToString("0.####");
            return Regex.IsMatch(strValue, DosagePattern);
        }
    }
}
