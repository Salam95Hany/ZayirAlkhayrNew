import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';
import { FinancialChartsResponseModel } from '../../Models/zainstitution/FinancialDashboardModel';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  // ============================= AccountsMony ==============================

  GetFinancialTransactionData(PagingFilter: PagingFilterModel, TransactionType: string) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/GetFinancialTransactionData?TransactionType=' + TransactionType, PagingFilter);
  }

  GetFinancialTransactionFilters(PagingFilter: PagingFilterModel, TransactionType: string) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'AccountsMony/GetFinancialTransactionFilters?TransactionType=' + TransactionType, PagingFilter);
  }

  GetFinancialTransactionStatistics(PagingFilter: PagingFilterModel, TransactionType: string) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/GetFinancialTransactionStatistics?TransactionType=' + TransactionType, PagingFilter);
  }

  AddNewFinancialTransaction(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/AddNewFinancialTransaction', Model);
  }

  UpdateFinancialTransaction(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/UpdateFinancialTransaction', Model);
  }

  DeleteFinancialTransaction(AccountId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/DeleteFinancialTransaction?AccountId=' + AccountId);
  }

  GetFinancialTransactionStatisticsNetValue(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/GetFinancialTransactionStatisticsNetValue', PagingFilter);
  }

  GetFinancialTransactionStatisticFilter() {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'AccountsMony/GetFinancialTransactionStatisticFilter');
  }

  GetFinancialNetValueChartsData() {
    return this.http.get<ApiResponseModel<FinancialChartsResponseModel>>(this.apiURL + 'AccountsMony/GetFinancialNetValueChartsData');
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

  GetAllGeneralTaskStatistics() {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'GeneralTasks/GetAllGeneralTaskStatistics');
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
