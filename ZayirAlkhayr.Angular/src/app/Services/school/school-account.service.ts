import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';
import { FormDropdownModel } from '../../Models/shared/FormDropdownModel';

@Injectable({
  providedIn: 'root'
})
export class SchoolAccountService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  // ============================= SchoolExpenses ==============================

  GetExpensesData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'SchoolExpenses/GetExpensesData', PagingFilter);
  }

  GetExpensesFilters(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'SchoolExpenses/GetExpensesFilters', PagingFilter);
  }

  GetExpensesStatistics(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'SchoolExpenses/GetExpensesStatistics', PagingFilter);
  }

  AddNewExpenses(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'SchoolExpenses/AddNewExpenses', Model);
  }

  UpdateExpenses(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'SchoolExpenses/UpdateExpenses', Model);
  }

  DeleteExpenses(ExpensesId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'SchoolExpenses/DeleteExpenses?ExpensesId=' + ExpensesId);
  }

  // ============================= ExpensesType ==============================

  GetAllExpensesTypes(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'ExpensesType/GetAllExpensesTypes', PagingFilter);
  }

  GetAllExpensesTypeFilters() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'ExpensesType/GetAllExpensesTypeFilters');
  }

  AddNewExpensesType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'ExpensesType/AddNewExpensesType', Model);
  }

  UpdateExpensesType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'ExpensesType/UpdateExpensesType', Model);
  }

  DeleteExpensesType(ExpensesTypeId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'ExpensesType/DeleteExpensesType?ExpensesTypeId=' + ExpensesTypeId);
  }

  GetAllExpensesTypesSelector() {
    return this.http.get<ApiResponseModel<FormDropdownModel[]>>(this.apiURL + 'Shared/GetAllExpensesTypesSelector');
  }
}
