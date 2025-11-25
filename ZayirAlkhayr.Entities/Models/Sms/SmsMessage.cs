using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models.Sms
{
    [Table(name: "SmsMessages", Schema = "sms")]
    public class SmsMessage
    {
        [Key]
        public int SmsMessageId { get; set; }
        public string DeviceName { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Sim { get; set; }
        public DateTime SentStamp { get; set; }
        public DateTime ReceivedStamp { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
