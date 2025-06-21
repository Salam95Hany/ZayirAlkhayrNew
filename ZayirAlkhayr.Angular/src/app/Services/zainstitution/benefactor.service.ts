import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../Models/shared/PagedResponseModel';

@Injectable({
  providedIn: 'root'
})
export class BenefactorService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  GetAllBeneFactorData(PagingFilter: PagingFilterModel) {
    return this.http.post<any>(this.apiURL + 'BeneFactor/GetAllBeneFactorData', PagingFilter);
  }

  GetAllBeneFactorFilters(PagingFilter: PagingFilterModel) {
    return this.http.post<any[]>(this.apiURL + 'BeneFactor/GetAllBeneFactorFilters', PagingFilter);
  }

  GetAllBeneFactorParentById(BeneFactorId: number) {
    return this.http.get<any[]>(this.apiURL + 'BeneFactor/GetAllBeneFactorParentById?BeneFactorId=' + BeneFactorId);
  }

  GetAllBeneFactorTypes(PagingFilter: PagingFilterModel) {
    return this.http.post<PagedResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorTypes', PagingFilter);
  }

  GetAllBeneFactorDetails(PagingFilter: PagingFilterModel, BeneFactorId: number) {
    return this.http.post<any[]>(this.apiURL + 'BeneFactor/GetAllBeneFactorDetails?BeneFactorId=' + BeneFactorId, PagingFilter);
  }

  GetAllBeneFactorCashDetails(BeneFactorId: number, ParentId: any) {
    return this.http.get<any[]>(this.apiURL + 'BeneFactor/GetAllBeneFactorCashDetails?BeneFactorId=' + BeneFactorId + '&ParentId=' + ParentId);
  }

  GetBeneFactorDetailsByBeneFactorId(BeneFactorId: number, BeneFactorTypeId: number) {
    return this.http.get<any[]>(this.apiURL + 'BeneFactor/GetBeneFactorDetailsByBeneFactorId?BeneFactorId=' + BeneFactorId + '&BeneFactorTypeId=' + BeneFactorTypeId);
  }

  GetBeneFactorDetailsStatistics(BeneFactorId: number) {
    return this.http.get<any[]>(this.apiURL + 'BeneFactor/GetBeneFactorDetailsStatistics?BeneFactorId=' + BeneFactorId);
  }

  GetBeneFactorTypeByIds(BeneFactorTypeIds: number[]) {
    return this.http.post<any[]>(this.apiURL + 'BeneFactor/GetBeneFactorTypeByIds', BeneFactorTypeIds);
  }

  GetBeneFactorNotes(PagingFilter: PagingFilterModel) {
    return this.http.post<PagedResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetBeneFactorNotes', PagingFilter);
  }

  GetAllBeneFactorNationalities(PagingFilter: PagingFilterModel) {
    return this.http.post<PagedResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorNationalities', PagingFilter);
  }

  AddNewBeneFactor(Model: any) {
    return this.http.post<any>(this.apiURL + 'BeneFactor/AddNewBeneFactor', Model);
  }

  AddNewBeneFactorValues(Model: any) {
    return this.http.post<any>(this.apiURL + 'BeneFactor/AddNewBeneFactorValues', Model);
  }

  AddNewBeneFactorType(Model: any) {
    return this.http.post<any>(this.apiURL + 'BeneFactor/AddNewBeneFactorType', Model);
  }

  AddNewBeneFactorDetails(Model: any) {
    return this.http.post<any>(this.apiURL + 'BeneFactor/AddNewBeneFactorDetails', Model);
  }

  AddNewBeneFactorNotes(Model: any) {
    return this.http.post<any>(this.apiURL + 'BeneFactor/AddNewBeneFactorNotes', Model);
  }

  AddNewBeneFactorNationality(Model: any) {
    return this.http.post<any>(this.apiURL + 'BeneFactor/AddNewBeneFactorNationality', Model);
  }

  UpdateBeneFactor(Model: any) {
    return this.http.post<any>(this.apiURL + 'BeneFactor/UpdateBeneFactor', Model);
  }

  DeleteBeneFactor(BeneFactorId: number) {
    return this.http.get<any>(this.apiURL + 'BeneFactor/DeleteBeneFactor?BeneFactorId=' + BeneFactorId);
  }

  DeleteBeneFactorDetails(DetailsId: number) {
    return this.http.get<any>(this.apiURL + 'BeneFactor/DeleteBeneFactorDetails?DetailsId=' + DetailsId);
  }
}
