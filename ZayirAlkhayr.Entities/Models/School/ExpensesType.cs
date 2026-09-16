using System.ComponentModel.DataAnnotations.Schema;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "ExpensesTypes", Schema = "school")]
    public class ExpensesType: AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
