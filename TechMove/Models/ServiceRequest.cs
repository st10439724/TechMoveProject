using System.ComponentModel.DataAnnotations;

namespace TechMove.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        // ties this request back to the contract it was raised under
        [Required]
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }

        [Required(ErrorMessage = "Please describe the service request")]
        public string Description { get; set; }

        // amount entered in USD by the logistics manager
        [Range(0.01, 999999, ErrorMessage = "Cost must be greater than zero")]
        public decimal CostUSD { get; set; }

        // calculated from USD - saved by hidden field not readonly
        public decimal CostZAR { get; set; }

        // Pending, In Progress, Completed
        [Required]
        public string Status { get; set; } = "Pending";
    }
}




