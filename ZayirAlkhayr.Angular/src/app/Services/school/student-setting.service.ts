import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';

@Injectable({
  providedIn: 'root'
})
export class StudentSettingService {
  apiURL = environment.apiUrl;
  
  constructor(private http: HttpClient) { }

    // ============================= AcademicStage ==============================

  GetAllAcademicStageData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AcademicStage/GetAllAcademicStageData', PagingFilter);
  }

  GetAllAcademicStageFilter() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'AcademicStage/GetAllAcademicStageFilter');
  }

  AddNewAcademicStage(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AcademicStage/AddNewAcademicStage', Model);
  }

  UpdateAcademicStage(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AcademicStage/UpdateAcademicStage', Model);
  }

  DeleteAcademicStage(AcademicStageId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'AcademicStage/DeleteAcademicStage?AcademicStageId=' + AcademicStageId);
  }

  GetAcademicStages() {
    return this.http.get<any[]>(this.apiURL + 'AcademicStage/GetAcademicStages');
  }

  // ============================= AcademicYear ==============================

  GetAllAcademicYearData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AcademicYear/GetAllAcademicYearData', PagingFilter);
  }

  GetAllAcademicYearFilter() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'AcademicYear/GetAllAcademicYearFilter');
  }

  AddNewAcademicYear(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AcademicYear/AddNewAcademicYear', Model);
  }

  UpdateAcademicYear(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AcademicYear/UpdateAcademicYear', Model);
  }

  DeleteAcademicYear(AcademicYearId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'AcademicYear/DeleteAcademicYear?AcademicYearId=' + AcademicYearId);
  }

  GetCurrentAcademicYear() {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'AcademicYear/GetCurrentAcademicYear');
  }

  // ============================= FeeType ==============================

  GetAllFeeTypeData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FeeType/GetAllFeeTypeData', PagingFilter);
  }

  GetAllFeeTypeFilter() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'FeeType/GetAllFeeTypeFilter');
  }

  AddNewFeeType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FeeType/AddNewFeeType', Model);
  }

  UpdateFeeType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FeeType/UpdateFeeType', Model);
  }

  DeleteFeeType(FeeTypeId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'FeeType/DeleteFeeType?FeeTypeId=' + FeeTypeId);
  }

   GetFeeTypes() {
    return this.http.get<any[]>(this.apiURL + 'FeeType/GetFeeTypes');
  }

   // ============================= FeeTemplate ==============================

  GetAllFeeTemplateData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FeeTemplate/GetAllFeeTemplateData', PagingFilter);
  }

  GetAllFeeTemplateFilter() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'FeeTemplate/GetAllFeeTemplateFilter');
  }

  AddNewFeeTemplate(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FeeTemplate/AddNewFeeTemplate', Model);
  }

  UpdateFeeTemplate(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'FeeTemplate/UpdateFeeTemplate', Model);
  }

  DeleteFeeTemplate(FeeTemplateId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'FeeTemplate/DeleteFeeTemplate?FeeTemplateId=' + FeeTemplateId);
  }

  // ============================= DiscountType ==============================

  GetAllDiscountTypeData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'DiscountType/GetAllDiscountTypeData', PagingFilter);
  }

  GetAllDiscountTypeFilter() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'DiscountType/GetAllDiscountTypeFilter');
  }

  AddNewDiscountType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'DiscountType/AddNewDiscountType', Model);
  }

  UpdateDiscountType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'DiscountType/UpdateDiscountType', Model);
  }

  DeleteDiscountType(DiscountTypeId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'DiscountType/DeleteDiscountType?DiscountTypeId=' + DiscountTypeId);
  }

  // ============================= StudentNationality ==============================

  GetAllStudentNationalityData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'StudentNationality/GetAllStudentNationalityData', PagingFilter);
  }

  GetAllStudentNationalityFilter() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'StudentNationality/GetAllStudentNationalityFilter');
  }

  AddNewStudentNationality(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'StudentNationality/AddNewStudentNationality', Model);
  }

  UpdateStudentNationality(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'StudentNationality/UpdateStudentNationality', Model);
  }

  DeleteStudentNationality(StudentNationalityId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'StudentNationality/DeleteStudentNationality?StudentNationalityId=' + StudentNationalityId);
  }
}
