using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor
{
    public class BeneFactorNoteDto
    {
        public int Code { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Nationality { get; set; }
        public string Note { get; set; }
        public string Suggestion { get; set; }
        public string InsertDate { get; set; }
    }
}
