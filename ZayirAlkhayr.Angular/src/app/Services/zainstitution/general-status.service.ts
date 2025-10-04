import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { FamilyStatusLookups } from '../../Models/zainstitution/GeneralStatus/FamilyStatusLookups';
import { UpdateFamilyStatusLookups } from '../../Models/zainstitution/GeneralStatus/UpdateFamilyStatusLookups';
import { AddFamilyStatusModel } from '../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';

@Injectable({
  providedIn: 'root'
})
export class GeneralStatusService {
apiURL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // ============================= FamilyStatus ==============================

  GetAllFamilyStatusData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyStatus/GetAllFamilyStatusData', PagingFilter);
  }

  GetAllFamilyStatusFilter(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyStatus/GetAllFamilyStatusFilter', PagingFilter);
  }

  GetFamilyStatusLookups() {
    return this.http.get<ApiResponseModel<FamilyStatusLookups>>(this.apiURL + 'FamilyStatus/GetFamilyStatusLookups');
  }

  GetUpdateFamilyStatusLookups(FamilyStatusId: number) {
    return this.http.get<ApiResponseModel<UpdateFamilyStatusLookups>>(this.apiURL + 'FamilyStatus/GetUpdateFamilyStatusLookups?FamilyStatusId=' + FamilyStatusId);
  }

  AddNewFamilyStatus(Model: AddFamilyStatusModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyStatus/AddNewFamilyStatus', Model);
  }

  UpdateFamilyStatus(Model: AddFamilyStatusModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyStatus/UpdateFamilyStatus', Model);
  }

  DeleteFamilyStatus(FamilyStatusId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'FamilyStatus/DeleteFamilyStatus?FamilyStatusId=' + FamilyStatusId);
  }

  // ============================= FamilyNeeds ==============================

  GetAllFamilyNeedTypesData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyNeeds/GetAllFamilyNeedTypesData', PagingFilter);
  }

  GetAllFamilyNeedTypesFilters(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyNeeds/GetAllFamilyNeedTypesFilters', PagingFilter);
  }

  GetAllFamilyNeedCategoriesData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyNeeds/GetAllFamilyNeedCategoriesData', PagingFilter);
  }

  GetAllFamilyNeedCategoriesFilters(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyNeeds/GetAllFamilyNeedCategoriesFilters', PagingFilter);
  }

  GetAllFamilyNeedCategories() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'FamilyNeeds/GetAllFamilyNeedCategories');
  }

  AddNewFamilyNeedType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyNeeds/AddNewFamilyNeedType', Model);
  }

  AddNewFamilyNeedCategory(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyNeeds/AddNewFamilyNeedCategory', Model);
  }

  UpdateFamilyNeedType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyNeeds/UpdateFamilyNeedType', Model);
  }

  UpdateFamilyNeedCategory(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyNeeds/UpdateFamilyNeedCategory', Model);
  }

  DeleteFamilyNeedType(NeedTypeId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'FamilyNeeds/DeleteFamilyNeedType?NeedTypeId=' + NeedTypeId);
  }

  DeleteFamilyNeedCategory(CategoryId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'FamilyNeeds/DeleteFamilyNeedCategory?CategoryId=' + CategoryId);
  }

  // ============================= FamilyNationality ==============================

  GetAllFamilyNationalitiesData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyNationality/GetAllFamilyNationalitiesData', PagingFilter);
  }

  GetAllFamilyNationalitiesFilter(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyNationality/GetAllFamilyNationalitiesFilter', PagingFilter);
  }

  AddNewFamilyNationality(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyNationality/AddNewFamilyNationality', Model);
  }

  UpdateFamilyNationality(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyNationality/UpdateFamilyNationality', Model);
  }

  DeleteFamilyNationality(NationalityId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'FamilyNationality/DeleteFamilyNationality?NationalityId=' + NationalityId);
  }

  // ============================= FamilyCategory ==============================

  GetAllFamilyCategoryData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyCategory/GetAllFamilyCategoryData', PagingFilter);
  }

  GetAllFamilyCategoryFilter(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyCategory/GetAllFamilyCategoryFilter', PagingFilter);
  }

  AddNewFamilyCategory(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyCategory/AddNewFamilyCategory', Model);
  }

  UpdateFamilyCategory(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyCategory/UpdateFamilyCategory', Model);
  }

  DeleteFamilyCategory(CategoryId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'FamilyCategory/DeleteFamilyCategory?CategoryId=' + CategoryId);
  }

  // ============================= FamilyPatient ==============================

  GetAllFamilyPatientData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyPatient/GetAllFamilyPatientData', PagingFilter);
  }

  GetAllFamilyPatientFilter(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'FamilyPatient/GetAllFamilyPatientFilter', PagingFilter);
  }

  AddNewFamilyPatient(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyPatient/AddNewFamilyPatient', Model);
  }

  UpdateFamilyPatient(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FamilyPatient/UpdateFamilyPatient', Model);
  }

  DeleteFamilyPatient(PatientId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'FamilyPatient/DeleteFamilyPatient?PatientId=' + PatientId);
  }
}
