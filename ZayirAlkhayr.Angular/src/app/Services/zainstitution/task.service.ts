import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  // ============================= AccountsMony ==============================

  GetAllAccountsExportMonyData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'AccountsMony/GetAllAccountsExportMonyData', PagingFilter);
  }

  GetAllAccountsExportMonyFilters(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'AccountsMony/GetAllAccountsExportMonyFilters', PagingFilter);
  }

  GetAllImportExportMonyStatistics(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/GetAllImportExportMonyStatistics', PagingFilter);
  }

  AddNewAccountsExportMony(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/AddNewAccountsExportMony', Model);
  }

  UpdateAccountsExportMony(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/UpdateAccountsExportMony', Model);
  }

  DeleteAccountsExportMony(AccountId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/DeleteAccountsExportMony?AccountId=' + AccountId);
  }

  GetAllAccountsImportMonyData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'AccountsMony/GetAllAccountsImportMonyData', PagingFilter);
  }

  GetAllAccountsImportMonyFilters(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'AccountsMony/GetAllAccountsImportMonyFilters', PagingFilter);
  }

  AddNewAccountsImportMony(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/AddNewAccountsImportMony', Model);
  }

  UpdateAccountsImportMony(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/UpdateAccountsImportMony', Model);
  }

  DeleteAccountsImportMony(AccountId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/DeleteAccountsImportMony?AccountId=' + AccountId);
  }

  // ============================= GeneralTasks ==============================

  GetAllUserTasks(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'GeneralTasks/GetAllUserTasks', PagingFilter);
  }

  ConvertTaskStatus(TaskId: number, StatusId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'GeneralTasks/ConvertTaskStatus?TaskId=' + TaskId + '&StatusId=' + StatusId);
  }

  GetAllGeneralTasksData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'GeneralTasks/GetAllGeneralTasksData', PagingFilter);
  }

  GetAllGeneralTasksFilter(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'GeneralTasks/GetAllGeneralTasksFilter', PagingFilter);
  }

  AddNewGeneralTask(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'GeneralTasks/AddNewGeneralTask', Model);
  }

  AddEditTaskComment(TaskId: number, Comment: string) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'GeneralTasks/AddEditTaskComment?TaskId=' + TaskId + '&Comment=' + Comment);
  }

  UpdateGeneralTask(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'GeneralTasks/UpdateGeneralTask', Model);
  }

  DeleteGeneralTask(TaskId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'GeneralTasks/DeleteGeneralTask?TaskId=' + TaskId);
  }
}
