using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.WebSite
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TotalDonationAmount { get; set; }
        public string BenefactorCount { get; set; }
        public string TotalAmount { get; set; }
        public string RemainingAmount { get; set; }
        public string ProjectUrl { get; set; }
        public string InsertDateStr { get; set; }
        public bool IsVisible { get; set; }
        public string CreatedBy { get; set; }
    }
}
