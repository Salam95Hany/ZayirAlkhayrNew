using System;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.GeneralServices
{
    public class UpdateFamilyStatusService : IUpdateFamilyStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateFamilyStatusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<string>> UpdateFamilyStatus(AddFamilyStatusModel Model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var FamilyObj = await _unitOfWork.Repository<FamilyStatus>().GetByIdAsync(Model.FamilyStatus.Id);
                if (FamilyObj != null)
                {
                    FamilyObj.StatusTypeId = Model.FamilyStatus.StatusTypeId;
                    FamilyObj.CategoryId = Model.FamilyStatus.CategoryId;
                    FamilyObj.NationalityId = Model.FamilyStatus.NationalityId;
                    FamilyObj.Name = Model.FamilyStatus.Name;
                    FamilyObj.Fname = Model.FamilyStatus.Fname;
                    FamilyObj.Address = Model.FamilyStatus.Address;
                    FamilyObj.NationalId = Model.FamilyStatus.NationalId;
                    FamilyObj.Village = Model.FamilyStatus.Village;
                    FamilyObj.Center = Model.FamilyStatus.Center;
                    FamilyObj.Governorate = Model.FamilyStatus.Governorate;
                    FamilyObj.Phone = Model.FamilyStatus.Phone;
                    FamilyObj.Phone1 = Model.FamilyStatus.Phone1;
                    FamilyObj.SupportingParty = Model.FamilyStatus.SupportingParty;
                    FamilyObj.ReasonOfRefuse = Model.FamilyStatus.ReasonOfRefuse;
                    FamilyObj.Relevance = Model.FamilyStatus.Relevance;
                    FamilyObj.Age = Model.FamilyStatus.Age;
                    FamilyObj.MaritalStatus = Model.FamilyStatus.MaritalStatus;
                    FamilyObj.Education = Model.FamilyStatus.Education;
                    FamilyObj.Jop = Model.FamilyStatus.Jop;
                    FamilyObj.UpdateUser = Model.FamilyStatus.InsertUser;
                    FamilyObj.UpdateDate = DateTime.UtcNow;

                    var tasks = new List<Task>
                    {
                        UpdateFamilyIncome(Model.FamilyIncome, Model.FamilyStatus.Id),
                        UpdateFamilyExpenses(Model.FamilyExpenses, Model.FamilyStatus.Id),
                        UpdateFamilyExtraDetails(Model.FamilyExtraDetails, Model.FamilyStatus.Id),
                        UpdateFamilyDetails(Model.FamilyDetails, Model.FamilyStatus.Id),
                        UpdateFamilyPatient(Model.FamilyPatient, Model.FamilyStatus.Id),
                        UpdateFamilyNeeds(Model.FamilyNeeds, Model.FamilyStatus.Id)
                    };

                    await Task.WhenAll(tasks);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync(cancellationToken);
                    return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        private async Task UpdateFamilyIncome(FamilyIncome Model, int FamilyStatusId)
        {
            var IncomeObj = await _unitOfWork.Repository<FamilyIncome>().GetByIdAsync(i => i.FamilyStatusId == FamilyStatusId);
            if (IncomeObj != null)
            {
                IncomeObj.FatherJop = Model.FatherJop;
                IncomeObj.MotherJop = Model.MotherJop;
                IncomeObj.ChildernsJop = Model.ChildernsJop;
                IncomeObj.AffairSpension_SocialSolidarity = Model.AffairSpension_SocialSolidarity;
                IncomeObj.Project = Model.Project;
                IncomeObj.LiveStock_Lands = Model.LiveStock_Lands;
                IncomeObj.Organization_ZakatCommittee = Model.Organization_ZakatCommittee;
                IncomeObj.InsurancePension = Model.InsurancePension;
                IncomeObj.Comments = Model.Comments;
                IncomeObj.Other = Model.Other;
                IncomeObj.TotalFamilyIncome = Model.TotalFamilyIncome;
            }
        }

        private async Task UpdateFamilyExpenses(FamilyExpense Model, int FamilyStatusId)
        {
            var ExpensesObj = await _unitOfWork.Repository<FamilyExpense>().GetByIdAsync(i => i.FamilyStatusId == FamilyStatusId);
            if (ExpensesObj != null)
            {
                ExpensesObj.Rent_Electricity_Water_Gas_Sewage = Model.Rent_Electricity_Water_Gas_Sewage;
                ExpensesObj.MedicalExamination_Treatment = Model.MedicalExamination_Treatment;
                ExpensesObj.SchoolExpenses = Model.SchoolExpenses;
                ExpensesObj.Installment_debts = Model.Installment_debts;
                ExpensesObj.PhysiotherapySessions = Model.PhysiotherapySessions;
                ExpensesObj.Analysis = Model.Analysis;
                ExpensesObj.SatisfactoryTransfers = Model.SatisfactoryTransfers;
                ExpensesObj.MedicalXRays = Model.MedicalXRays;
                ExpensesObj.IsMinisterialSupply = Model.IsMinisterialSupply.GetValueOrDefault(false);
                ExpensesObj.IsFoodBank = Model.IsFoodBank.GetValueOrDefault(false);
                ExpensesObj.TotalFamilyExpenses = Model.TotalFamilyExpenses;
                ExpensesObj.NetFamilyIncome = Model.NetFamilyIncome;
                ExpensesObj.FamilyCount = Model.FamilyCount;
                ExpensesObj.AvgPersonIncome = Model.AvgPersonIncome;
            }
        }

        private async Task UpdateFamilyExtraDetails(FamilyExtraDetail Model, int FamilyStatusId)
        {
            bool isEmpty = Model.AreAllPropertiesDefault();
            if (Model != null && !isEmpty)
            {
                await _unitOfWork.Repository<FamilyExtraDetail>().DeleteWhereAsync(i => i.FamilyStatusId == FamilyStatusId);

                var ExtraDetailsObj = new FamilyExtraDetail();
                ExtraDetailsObj.FamilyStatusId = FamilyStatusId;
                ExtraDetailsObj.StatusDescription = Model.StatusDescription;
                ExtraDetailsObj.HousingNeedsAndStatus = Model.HousingNeedsAndStatus;
                ExtraDetailsObj.ResearcherNotes = Model.ResearcherNotes;
                ExtraDetailsObj.ReferencesNotes = Model.ReferencesNotes;
                ExtraDetailsObj.LastVisitDate = Model.LastVisitDate;
                ExtraDetailsObj.PersonalPapers = Model.PersonalPapers;
                await _unitOfWork.Repository<FamilyExtraDetail>().AddAsync(ExtraDetailsObj);
            }
        }

        private async Task UpdateFamilyDetails(List<FamilyDetail> Model, int FamilyStatusId)
        {
            if (!Model.Any()) return;

            await _unitOfWork.Repository<FamilyDetail>().DeleteWhereAsync(i => i.FamilyStatusId == FamilyStatusId);

            var FamilyDetails = Model.Select(i => new FamilyDetail
            {
                FamilyStatusId = FamilyStatusId,
                Name = i.Name,
                Relevance = i.Relevance,
                Age = i.Age,
                MaritalStatus = i.MaritalStatus,
                Education = i.Education,
                Jop = i.Jop,
                NationalId = i.NationalId,
                ChildernsCount = i.ChildernsCount,
                FamilyMembersCount = i.FamilyMembersCount
            }).ToList();

            await _unitOfWork.Repository<FamilyDetail>().AddRangeAsync(FamilyDetails);
        }

        private async Task UpdateFamilyPatient(List<FamilyPatientGroup> Model, int FamilyStatusId)
        {
            if (!Model.Any()) return;

            await _unitOfWork.Repository<FamilyPatient>().DeleteWhereAsync(i => i.FamilyStatusId == FamilyStatusId);

            foreach (var FamilyPatient in Model)
            {
                var FamilyPatients = FamilyPatient.PatientTypeIds.Select(typeId => new FamilyPatient
                {
                    FamilyStatusId = FamilyStatusId,
                    Name = FamilyPatient.Name,
                    PatientTypeId = typeId,
                    PatientDate = FamilyPatient.PatientDate,
                    Specialization = FamilyPatient.Specialization,
                    IsMedicalReport = FamilyPatient.IsMedicalReport.GetValueOrDefault(false),
                    IsNeedProcess = FamilyPatient.IsNeedProcess.GetValueOrDefault(false)
                }).ToList();

                await _unitOfWork.Repository<FamilyPatient>().AddRangeAsync(FamilyPatients);
            }
        }

        private async Task UpdateFamilyNeeds(List<FamilyNeed> Model, int FamilyStatusId)
        {
            if (!Model.Any()) return;

            await _unitOfWork.Repository<FamilyNeed>().DeleteWhereAsync(i => i.StatusId == FamilyStatusId);

            var FamilyNeeds = Model.Select(i => new FamilyNeed
            {
                StatusId = FamilyStatusId,
                NeedTypeId = i.NeedTypeId,
                IsWaiting = i.IsWaiting,
                DeliveryDate = i.DeliveryDate
            }).ToList();

            await _unitOfWork.Repository<FamilyNeed>().AddRangeAsync(FamilyNeeds);
        }
    }
}
