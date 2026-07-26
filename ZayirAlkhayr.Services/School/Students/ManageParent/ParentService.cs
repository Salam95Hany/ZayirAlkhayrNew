using Microsoft.Data.SqlClient;
using System.Globalization;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageParent;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageParent
{
    public class ParentService : IParentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public ParentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<ParentDto>>> GetAllParentData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new ParentSpecification(PagingFilter);
            var CountSpec = new ParentSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<Parent>();
            var TotalCount = await Entity.GetCountAsync(CountSpec, cancellationToken);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec, cancellationToken);
            var Results = Data.Select(fc => new ParentDto
            {
                Id = fc.Id,
                Name = fc.Name,
                ParentPhone = fc.ParentPhone,
                MotherPhone = fc.MotherPhone,
                Address = fc.Address,
                WhatsappNumber = fc.WhatsappNumber,
                IsActive = fc.IsActive,
                ChildrenCount = fc.Students?.Count ?? 0,
                CreatedBy = fc.Students?.FirstOrDefault()?.CreatedBy?.UserName,
                InsertDateStr = fc.Students?.FirstOrDefault()?.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<ParentDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<string>> AddNewParent(Parent Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Parent>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var parent = new Parent
                {
                    Name = Model.Name,
                    ParentPhone = Model.ParentPhone,
                    MotherPhone = Model.MotherPhone,
                    WhatsappNumber = Model.WhatsappNumber,
                    Address = Model.Address,
                    TelegramCode = GenerateTelCode()
                };


                await _unitOfWork.Repository<Parent>().AddAsync(parent);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateParent(Parent Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Parent>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = await _unitOfWork.Repository<Parent>().GetByIdAsync(Model.Id);
                if (PatientObj != null)
                {
                    PatientObj.Name = Model.Name;
                    PatientObj.ParentPhone = Model.ParentPhone;
                    PatientObj.MotherPhone = Model.MotherPhone;
                    PatientObj.WhatsappNumber = Model.WhatsappNumber;
                    PatientObj.Address = Model.Address;

                    await _unitOfWork.CompleteAsync();

                    return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteParent(int ParentId)
        {
            try
            {
                var Patient = await _unitOfWork.Repository<Parent>().GetByIdAsync(ParentId);
                if (Patient != null)
                {
                    _unitOfWork.Repository<Parent>().Delete(Patient);
                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Message.Contains("REFERENCE constraint"))
                    {
                        return ApiResponseModel<string>.Failure(GenericErrors.DeleteRelationRow);
                    }
                }

                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<List<FormDropdownModel>> GetParents()
        {
            var results = await _unitOfWork.Repository<Parent>().GetAllAsync();
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.Name
            }).ToList();
            return data;
        }

        string GenerateTelCode()
        {
            var random = Random.Shared;
            return new string(Enumerable.Range(0, 8).Select(_ => Chars[random.Next(Chars.Length)]).ToArray());
        }
    }
}
