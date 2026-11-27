namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class SchoolExpenseStatisticsDto
    {
        public double? TotalMoney { get; set; }
        public double? CurrentMonthMony { get; set; }
        public double TotalMoneyPercentage { get; set; }
        public double CurrentMonthMonyPercentage { get; set; }
    }
}
