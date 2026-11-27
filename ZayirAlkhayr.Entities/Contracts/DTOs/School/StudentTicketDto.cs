namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class StudentTicketDto
    {
        public int TotalCount { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Grade { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public DateTime? InstallmentRenewalDate { get; set; }
        public DateTime? LastPrintedAt { get; set; }
    }
}
