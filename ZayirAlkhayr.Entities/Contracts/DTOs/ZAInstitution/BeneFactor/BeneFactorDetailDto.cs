using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor
{
    public class BeneFactorDetailDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public int BeneFactorTypeId { get; set; }
        public string Name { get; set; }
        public double? TotalValue { get; set; }
        public string? Details { get; set; }
        public string? InsertUser { get; set; }
        public string? Image { get; set; }
        public string? InsertDate { get; set; }
        public string? PaymentDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
