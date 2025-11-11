using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.WebSite
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Month { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public string InsertDate { get; set; }
        public bool IsVisible { get; set; }
        public string CreatedBy { get; set; }
    }
}
