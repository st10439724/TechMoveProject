

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechMove.Models
{
    public class Contract
    {
        public int Id { get; set; }

        // which client does this contract belong to
        [Required]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // possible values: Draft, Active, Expired, On Hold
        [Required]
        public string Status { get; set; } = "Draft";

        // e.g. Gold, Silver, Bronze - depends on the SLA agreed with client
        [Required]
        public string ServiceLevel { get; set; }

        // PDF upload does not upload -> ill redo for part 3(nb) 
        public string? SignedAgreementPath { get; set; }

        // navigation property - not posted from form so must be nullable
        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}