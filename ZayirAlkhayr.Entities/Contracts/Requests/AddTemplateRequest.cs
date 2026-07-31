using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.Requests
{
    public class AddTemplateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string InsertUser { get; set; }
        public List<int> VariableIds { get; set; }
    }
}
