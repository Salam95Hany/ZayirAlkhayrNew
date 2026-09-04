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

  GetStudentHistoryById(StudentId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'Student/GetStudentHistoryById?StudentId=' + StudentId);
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

  // ============================= StudentFee ==============================

  GetCurrentAcademicYearFinancialSummary() {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'StudentFee/GetCurrentAcademicYearFinancialSummary');
  }

  GetAllStudentFeeData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'StudentFee/GetAllStudentFeeData', PagingFilter);
  }

  GetAllStudentFeeFilters(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'StudentFee/GetAllStudentFeeFilters', PagingFilter);
  }

  AddNewStudentFee(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'StudentFee/AddNewStudentFee', Model);
  }

  UpdateStudentFee(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'StudentFee/UpdateStudentFee', Model);
  }

  CancelStudentFee(StudentFeeId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'StudentFee/CancelStudentFee?StudentFeeId=' + StudentFeeId);
  }

  GetFeeTemplates(EnrollmentId: number) {
    return this.http.get<any[]>(this.apiURL + 'StudentFee/GetFeeTemplates?EnrollmentId=' + EnrollmentId);
  }

  GetStudents() {
    return this.http.get<any[]>(this.apiURL + 'StudentFee/GetStudents');
  }

  GetDiscountTypes() {
    return this.http.get<any[]>(this.apiURL + 'StudentFee/GetDiscountTypes');
  }

  // ============================= ReceivePayment ==============================

  GetAllStudentFeesByEnrollmentId(EnrollmentId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'ReceivePayment/GetAllStudentFeesByEnrollmentId?EnrollmentId=' + EnrollmentId);
  }

  ReceivePayment(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'ReceivePayment/ReceivePayment', Model);
  }

  CancelPayment(StudentPaymentId: number, CancelledBy: string) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'ReceivePayment/CancelPayment?StudentPaymentId=' + StudentPaymentId + '&CancelledBy=' + CancelledBy);
  }

  GetStudentFees(EnrollmentId: number) {
    return this.http.get<any[]>(this.apiURL + 'ReceivePayment/GetStudentFees?EnrollmentId=' + EnrollmentId);
  }

  GetReceiveStudents() {
    return this.http.get<any[]>(this.apiURL + 'ReceivePayment/GetReceiveStudents');
  }

  // ============================= StudentPayment ==============================

  GetAllStudentPaymentData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'StudentPayment/GetAllStudentPaymentData', PagingFilter);
  }

  GetAllStudentPaymentFilters(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'StudentPayment/GetAllStudentPaymentFilters', PagingFilter);
  }
}
