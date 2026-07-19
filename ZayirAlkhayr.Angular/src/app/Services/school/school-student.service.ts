import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { StudentLookups } from '../../Models/school/student/StudentLookups';
import { UpdateStudentLookups } from '../../Models/school/student/UpdateStudentLookups';
import { AddStudentModel } from '../../Models/school/student/AddStudentModel';

@Injectable({
  providedIn: 'root'
})
export class SchoolStudentService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  // ============================= Student ==============================

  GetAllStudentData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Student/GetAllStudentData', PagingFilter);
  }

  GetAllStudentFilter(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'Student/GetAllStudentFilter', PagingFilter);
  }

  GetStudentLookups() {
    return this.http.get<ApiResponseModel<StudentLookups>>(this.apiURL + 'Student/GetStudentLookups');
  }

  GetUpdateStudentLookups(StudentId: number, ParentId: number) {
    return this.http.get<ApiResponseModel<UpdateStudentLookups>>(this.apiURL + 'Student/GetUpdateStudentLookups?StudentId=' + StudentId + '&ParentId=' + ParentId);
  }

  AddNewStudent(Model: AddStudentModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Student/AddNewStudent', Model);
  }

  UpdateStudent(Model: AddStudentModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Student/UpdateStudent', Model);
  }

  DeleteStudent(ParentId: number, StudentId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'Student/DeleteStudent?ParentId=' + ParentId + '&StudentId=' + StudentId);
  }

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
