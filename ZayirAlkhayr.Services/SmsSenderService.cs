using Microsoft.AspNetCore.Hosting;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.Sms;
using ZayirAlkhayr.Entities.Specifications.Sms;
using ZayirAlkhayr.Interfaces;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services
{
    public class SmsSenderService : ISmsSenderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SmsSenderService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> SaveMessage(SmsMessage Model)
        {
            try
            {
                await _unitOfWork.Repository<SmsMessage>().AddAsync(Model);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                var FolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Logs");
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string FilePath = Path.Combine(FolderPath, "LogParams.txt");

                System.IO.File.AppendAllText(FilePath, ex.Message);

                return false;
            }
        }

        public async Task<ApiResponseModel<List<SmsMessage>>> GetAllSmsMessageData(PagingFilterModel PagingFilter)
        {
            var DataSpec = new SmsMessageDataSpecification(PagingFilter);
            var CountSpec = new SmsMessageDataSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<SmsMessage>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            return ApiResponseModel<List<SmsMessage>>.Success(GenericErrors.GetSuccess, Data, TotalCount);
        }
    }
}
