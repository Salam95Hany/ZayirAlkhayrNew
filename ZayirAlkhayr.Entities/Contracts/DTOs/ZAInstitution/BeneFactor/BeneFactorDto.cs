using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor
{
    public class BeneFactorDto
    {
        public List<BeneFactorData> Data { get; set; }
        public List<PDFHeader> Header { get; set; }
    }


    public class BeneFactorData
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int NationalityId { get; set; }
        public int Code { get; set; }
        public string FullName { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? WelcomeMessage { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }
        public string? FaceBook { get; set; }
        public string? CreatedBy { get; set; }
        public string? Image { get; set; }
        public string? InsertDate { get; set; }
    }
}
