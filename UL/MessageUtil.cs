namespace UL
{
    public static class MessageUtil
    {
        // Success Messages
        public const string RecordCreated = "Record successfully saved.";
        public const string RecordUpdated = "Record successfully updated.";
        public const string RecordDeleted = "Record successfully deleted.";

        // Failed Messages
        public const string SaveFailed = "Failed to save record.";
        public const string UpdateFailed = "Failed to update record.";
        public const string DeleteFailed = "Failed to delete record.";

        // Validation Messages
        public const string AllFieldsRequired = "All field/s are required.";
        public const string DuplicateRecord = "Cannot add same drug to a patient.";
        public const string RecordAlreadyExists = "Record already exists.";
        public const string NoChanges = "No changes has made";

        // Input Validation Messages
        public const string InvalidPatient = "Patient name is invalid. It must only contain letters, spaces, hyphen (-), or apostrophe (')";
        public const string InvalidDrug = "Drug name is invalid. It must only contain letters, numbers and spaces";
        public const string InvalidDosage = "Dosage is invalid. Must be greater than zero and can have up to 4 decimal places.";
    }
}