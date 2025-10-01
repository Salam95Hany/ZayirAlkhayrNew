import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';
import { FormDropdownModel } from '../../Models/shared/FormDropdownModel';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  apiURL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  GetAllBeneFactorsSelector() {
    return this.http.get<ApiResponseModel<FormDropdownModel[]>>(this.apiURL + 'Shared/GetAllBeneFactorsSelector');
  }

  GetAllBeneFactorNationalitiesSelector() {
    return this.http.get<ApiResponseModel<FormDropdownModel[]>>(this.apiURL + 'Shared/GetAllBeneFactorNationalitiesSelector');
  }

  GetAllBeneFactorParentSelectorById(BeneFactorId: number) {
    return this.http.get<ApiResponseModel<FormDropdownModel[]>>(this.apiURL + 'Shared/GetAllBeneFactorParentSelectorById?BeneFactorId=' + BeneFactorId);
  }

  GetAllBeneFactorTypesSelector() {
    return this.http.get<ApiResponseModel<FormDropdownModel[]>>(this.apiURL + 'Shared/GetAllBeneFactorTypesSelector');
  }
}
