using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.Sms;

namespace ZayirAlkhayr.Interfaces
{
    public interface ISmsSenderService
    {
        Task<bool> SaveMessage(SmsMessage Model);
        Task<ApiResponseModel<List<SmsMessage>>> GetAllSmsMessageData(PagingFilterModel PagingFilter);
    }
}
