using System;
using System.ComponentModel.DataAnnotations;

namespace EL
{
    public class MedicationEntity : Entity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient name is required.")]
        [StringLength(50, ErrorMessage = "Patient name cannot exceed 50 characters.")]
        public string Patient { get; set; }

        [Required(ErrorMessage = "Drug name is required.")]
        [StringLength(50, ErrorMessage = "Drug name cannot exceed 50 characters.")]
        public string Drug { get; set; }

        [Required(ErrorMessage = "Dosage is required.")]
        [Range(0.0001, 999.9999)]
        public decimal? Dosage { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime ModifiedDate { get; set; }
    }
}
