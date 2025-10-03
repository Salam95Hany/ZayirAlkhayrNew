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

  // ============================= SliderImage ==============================

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
}
