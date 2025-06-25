using System;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.GeneralServices
{
    public class AddFamilyStatusService : IAddFamilyStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddFamilyStatusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<string>> AddNewFamilyStatus(AddFamilyStatusModel Model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var Family = new FamilyStatus
                {
                    StatusTypeId = Model.FamilyStatus.StatusTypeId,
                    CategoryId = Model.FamilyStatus.CategoryId,
                    NationalityId = Model.FamilyStatus.NationalityId,
                    Name = Model.FamilyStatus.Name,
                    Fname = Model.FamilyStatus.Fname,
                    Address = Model.FamilyStatus.Address,
                    NationalId = Model.FamilyStatus.NationalId,
                    Village = Model.FamilyStatus.Village,
                    Center = Model.FamilyStatus.Center,
                    Governorate = Model.FamilyStatus.Governorate,
                    Phone = Model.FamilyStatus.Phone,
                    Phone1 = Model.FamilyStatus.Phone1,
                    SupportingParty = Model.FamilyStatus.SupportingParty,
                    ReasonOfRefuse = Model.FamilyStatus.ReasonOfRefuse,
                    Relevance = Model.FamilyStatus.Relevance,
                    Age = Model.FamilyStatus.Age,
                    MaritalStatus = Model.FamilyStatus.MaritalStatus,
                    Education = Model.FamilyStatus.Education,
                    Jop = Model.FamilyStatus.Jop,
                    InsertUser = Model.FamilyStatus.InsertUser,
                    InsertDate = DateTime.UtcNow,
                    AddedDate = Model.FamilyStatus.AddedDate
                };

                var lastCode = await _unitOfWork.Repository<FamilyStatus>().MaxAsync(i => (int?)i.Code) ?? 0;
                Family.Code = lastCode + 1;

                await _unitOfWork.Repository<FamilyStatus>().AddAsync(Family);
                await _unitOfWork.CompleteAsync();

                var tasks = new List<Task>
                {
                    AddNewFamilyIncome(Model.FamilyIncome, Family.Id),
                    AddNewFamilyExpenses(Model.FamilyExpenses, Family.Id),
                    AddNewFamilyExtraDetails(Model.FamilyExtraDetails, Family.Id),
                    AddNewFamilyDetails(Model.FamilyDetails, Family.Id),
                    AddNewFamilyPatient(Model.FamilyPatient, Family.Id),
                    AddNewFamilyNeeds(Model.FamilyNeeds, Family.Id)
                };

                await Task.WhenAll(tasks);
                await _unitOfWork.CompleteAsync();

                await transaction.CommitAsync(cancellationToken);

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        private async Task AddNewFamilyIncome(FamilyIncome Model, int FamilyStatusId)
        {
            var IncomeObj = new FamilyIncome
            {
                FamilyStatusId = FamilyStatusId,
                FatherJop = Model.FatherJop,
                MotherJop = Model.MotherJop,
                ChildernsJop = Model.ChildernsJop,
                AffairSpension_SocialSolidarity = Model.AffairSpension_SocialSolidarity,
                Project = Model.Project,
                LiveStock_Lands = Model.LiveStock_Lands,
                Organization_ZakatCommittee = Model.Organization_ZakatCommittee,
                InsurancePension = Model.InsurancePension,
                Comments = Model.Comments,
                Other = Model.Other,
                TotalFamilyIncome = Model.TotalFamilyIncome
            };

            await _unitOfWork.Repository<FamilyIncome>().AddAsync(IncomeObj);
        }

        private async Task AddNewFamilyExpenses(FamilyExpense Model, int FamilyStatusId)
        {
            var ExpensesObj = new FamilyExpense
            {
                FamilyStatusId = FamilyStatusId,
                Rent_Electricity_Water_Gas_Sewage = Model.Rent_Electricity_Water_Gas_Sewage,
                MedicalExamination_Treatment = Model.MedicalExamination_Treatment,
                SchoolExpenses = Model.SchoolExpenses,
                Installment_debts = Model.Installment_debts,
                PhysiotherapySessions = Model.PhysiotherapySessions,
                Analysis = Model.Analysis,
                SatisfactoryTransfers = Model.SatisfactoryTransfers,
                MedicalXRays = Model.MedicalXRays,
                IsMinisterialSupply = Model.IsMinisterialSupply.GetValueOrDefault(false),
                IsFoodBank = Model.IsFoodBank.GetValueOrDefault(false),
                TotalFamilyExpenses = Model.TotalFamilyExpenses,
                NetFamilyIncome = Model.NetFamilyIncome,
                FamilyCount = Model.FamilyCount,
                AvgPersonIncome = Model.AvgPersonIncome
            };

            await _unitOfWork.Repository<FamilyExpense>().AddAsync(ExpensesObj);
        }

        private async Task AddNewFamilyExtraDetails(FamilyExtraDetail Model, int FamilyStatusId)
        {
            bool isEmpty = Model.AreAllPropertiesDefault();
            if (!isEmpty)
            {
                var ExtraDetailsObj = new FamilyExtraDetail
                {
                    FamilyStatusId = FamilyStatusId,
                    StatusDescription = Model.StatusDescription,
                    HousingNeedsAndStatus = Model.HousingNeedsAndStatus,
                    ResearcherNotes = Model.ResearcherNotes,
                    ReferencesNotes = Model.ReferencesNotes,
                    LastVisitDate = Model.LastVisitDate,
                    PersonalPapers = Model.PersonalPapers
                };

                await _unitOfWork.Repository<FamilyExtraDetail>().AddAsync(ExtraDetailsObj);
            }
        }

        private async Task AddNewFamilyDetails(List<FamilyDetail> Model, int FamilyStatusId)
        {
            if (Model.Count == 0) { return; }

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

        private async Task AddNewFamilyPatient(List<FamilyPatientGroup> Model, int FamilyStatusId)
        {
            if (Model.Count == 0) { return; }

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

        private async Task AddNewFamilyNeeds(List<FamilyNeed> Model, int FamilyStatusId)
        {
            if (Model.Count == 0) { return; }

            var FamilyNeeds = Model.Select(i => new FamilyNeed
            {
                StatusId = FamilyStatusId,
                NeedTypeId = i.NeedTypeId,
                IsWaiting = i.IsWaiting,
                DeliveryDate = i.DeliveryDate
            }).ToList();

            await _unitOfWork.Repository<FamilyNeed>().AddRangeAsync(FamilyNeeds);
        }

        public async Task<ApiResponseModel<string>> DeleteFamilyStatus(int FamilyStatusId, CancellationToken cancellationToken = default)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var familyStatus = await _unitOfWork.Repository<FamilyStatus>().GetByIdAsync(FamilyStatusId);
                if (familyStatus == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                await _unitOfWork.Repository<FamilyIncome>().DeleteWhereAsync(i => i.FamilyStatusId == FamilyStatusId);
                await _unitOfWork.Repository<FamilyExpense>().DeleteWhereAsync(i => i.FamilyStatusId == FamilyStatusId);
                await _unitOfWork.Repository<FamilyExtraDetail>().DeleteWhereAsync(i => i.FamilyStatusId == FamilyStatusId);
                await _unitOfWork.Repository<FamilyDetail>().DeleteWhereAsync(i => i.FamilyStatusId == FamilyStatusId);
                await _unitOfWork.Repository<FamilyPatient>().DeleteWhereAsync(i => i.FamilyStatusId == FamilyStatusId);
                await _unitOfWork.Repository<FamilyNeed>().DeleteWhereAsync(i => i.StatusId == FamilyStatusId);

                _unitOfWork.Repository<FamilyStatus>().Delete(familyStatus);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync(cancellationToken);

                return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
