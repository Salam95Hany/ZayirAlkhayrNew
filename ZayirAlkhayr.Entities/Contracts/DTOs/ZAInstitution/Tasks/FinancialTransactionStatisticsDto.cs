using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.Tasks
{
    public class FinancialTransactionStatisticsDto
    {
        public double TotalIncome { get; set; }
        public double TotalExpenses { get; set; }
        public double NetValue { get; set; }
    }
}
