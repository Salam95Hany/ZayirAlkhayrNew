using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.Setting
{
    public interface IAcademicYearService
    {
        Task<ApiResponseModel<List<AcademicYearDto>>> GetAllAcademicYearData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllAcademicYearFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewAcademicYear(AcademicYear Model);
        Task<ApiResponseModel<string>> UpdateAcademicYear(AcademicYear Model);
        Task<ApiResponseModel<string>> DeleteAcademicYear(int AcademicYearId);
        Task<ApiResponseModel<string>> GetCurrentAcademicYear();
    }
}
