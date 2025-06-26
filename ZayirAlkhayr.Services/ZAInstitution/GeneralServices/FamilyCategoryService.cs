using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.GeneralServices
{
    public class FamilyCategoryService : IFamilyCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        private readonly IGenerateFiltersService _generateFiltersService;
        public FamilyCategoryService(ISQLHelper sQLHelper, IUnitOfWork unitOfWork, IGenerateFiltersService generateFiltersService)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
            _generateFiltersService = generateFiltersService;
        }

        public async Task<ApiResponseModel<List<FamilyCategoryDto>>> GetAllFamilyCategoryData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new FamilyCategoryFilterSpecification(PagingFilter);
            var CountSpec = new FamilyCategoryFilterSpecification(PagingFilter,false);
            var Entity = _unitOfWork.Repository<FamilyCategory>();
            var TotalCount = await Entity.GetCountAsync(CountSpec, cancellationToken);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec, cancellationToken);
            var Results = Data.Select(fc => new FamilyCategoryDto
            {
                Id = fc.Id,
                Name = fc.Name,
                CreatedBy = fc.CreatedBy.UserName,
                UserId = fc.InsertUser,
                InsertDate = fc.InsertDate,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<FamilyCategoryDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyCategoryFilter(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<FamilyCategory>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new FamilyCategory
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<FamilyCategory>>
            {
                new()
                {
                    CategoryName = "المستخدمين",
                    Source = data,
                    ItemIdSelector = x => x.InsertUser,
                    ItemKeySelector = x => x.CreatedBy?.UserName ?? ""
                }
            };

            var results = await _generateFiltersService.GenerateManyAsync(filterRequests, cancellationToken);
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, results);
        }

        //public HandleErrorResponseModel AddNewFamilyCategory(FamilyCategory Model)
        //{
        //    try
        //    {
        //        var Response = new HandleErrorResponseModel();
        //        var CategoryObj = new FamilyCategories();
        //        CategoryObj.Name = Model.Name;
        //        CategoryObj.InsertUser = Model.InsertUser;
        //        CategoryObj.InsertDate = DateTime.Now.AddHours(1);

        //        _Context.FamilyCategories.Add(CategoryObj);
        //        _Context.SaveChanges();

        //        Response.Done = true;
        //        Response.Message = "تم اضافة عنصر جديد بنجاح";
        //        return Response;
        //    }
        //    catch (Exception)
        //    {
        //        var Response = new HandleErrorResponseModel();
        //        Response.Done = false;
        //        Response.Message = "لقد حدث خطا";
        //        return Response;
        //    }
        //}

        //public HandleErrorResponseModel UpdateFamilyCategory(FamilyCategories Model)
        //{
        //    try
        //    {
        //        var Response = new HandleErrorResponseModel();
        //        var CategoryObj = _Context.FamilyCategories.FirstOrDefault(x => x.Id == Model.Id);
        //        CategoryObj.Name = Model.Name;
        //        CategoryObj.UpdateUser = Model.InsertUser;
        //        CategoryObj.UpdateDate = DateTime.Now.AddHours(1);

        //        _Context.SaveChanges();

        //        Response.Done = true;
        //        Response.Message = "تم تعديل العنصر بنجاح";
        //        return Response;
        //    }
        //    catch (Exception)
        //    {
        //        var Response = new HandleErrorResponseModel();
        //        Response.Done = false;
        //        Response.Message = "لقد حدث خطا";
        //        return Response;
        //    }
        //}

        //public HandleErrorResponseModel DeleteFamilyCategory(int CategoryId)
        //{
        //    try
        //    {
        //        var Response = new HandleErrorResponseModel();
        //        var Category = _Context.FamilyCategories.FirstOrDefault(i => i.Id == CategoryId);

        //        _Context.FamilyCategories.Remove(Category);
        //        _Context.SaveChanges();
        //        Response.Done = true;
        //        Response.Message = "تم حذف العنصر بنجاح";
        //        return Response;
        //    }
        //    catch (Exception)
        //    {
        //        var Response = new HandleErrorResponseModel();
        //        Response.Done = false;
        //        Response.Message = "لقد حدث خطا";
        //        return Response;
        //    }
        //}
    }
}
