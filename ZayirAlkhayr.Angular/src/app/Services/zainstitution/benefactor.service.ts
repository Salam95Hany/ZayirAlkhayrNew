import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class BenefactorService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient, private router: Router) { }

  GetAllBeneFactorData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/GetAllBeneFactorData', PagingFilter);
  }

  GetAllBeneFactorFilters() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorFilters');
  }

  GetAllBeneFactorParentById(BeneFactorId: number) {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorParentById?BeneFactorId=' + BeneFactorId);
  }

  GetAllBeneFactorTypes(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorTypes', PagingFilter);
  }

  GetAllBeneFactorTypeFilters() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorTypeFilters');
  }

  GetAllBeneFactorDetails(PagingFilter: PagingFilterModel, BeneFactorId: number) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorDetails?BeneFactorId=' + BeneFactorId, PagingFilter);
  }

  GetAllBeneFactorCashDetails(BeneFactorId: number, ParentId: any) {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorCashDetails?BeneFactorId=' + BeneFactorId + '&ParentId=' + ParentId);
  }

  GetBeneFactorDetailsByBeneFactorId(BeneFactorId: number, BeneFactorTypeId: number) {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetBeneFactorDetailsByBeneFactorId?BeneFactorId=' + BeneFactorId + '&BeneFactorTypeId=' + BeneFactorTypeId);
  }

  GetBeneFactorDetailsStatistics(BeneFactorId: number) {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetBeneFactorDetailsStatistics?BeneFactorId=' + BeneFactorId);
  }

  GetBeneFactorTypeByIds(BeneFactorTypeIds: number[]) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetBeneFactorTypeByIds', BeneFactorTypeIds);
  }

  GetBeneFactorNotes(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetBeneFactorNotes', PagingFilter);
  }

  GetAllBeneFactorNationalities(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorNationalities', PagingFilter);
  }

  GetAllBeneFactorNationalityFilters() {
    return this.http.get<ApiResponseModel<any[]>>(this.apiURL + 'BeneFactor/GetAllBeneFactorNationalityFilters');
  }

  AddNewBeneFactor(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/AddNewBeneFactor', Model);
  }

  AddNewBeneFactorValues(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/AddNewBeneFactorValues', Model);
  }

  AddNewBeneFactorType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/AddNewBeneFactorType', Model);
  }

  UpdateBeneFactorType(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/UpdateBeneFactorType', Model);
  }

  DeleteBeneFactorType(TypeId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/DeleteBeneFactorType?TypeId=' + TypeId);
  }

  AddNewBeneFactorDetails(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/AddNewBeneFactorDetails', Model);
  }

  AddNewBeneFactorNotes(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/AddNewBeneFactorNotes', Model);
  }

  AddNewBeneFactorNationality(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/AddNewBeneFactorNationality', Model);
  }

  UpdateBeneFactorNationality(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/UpdateBeneFactorNationality', Model);
  }

  DeleteBeneFactorNationality(NationalityId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/DeleteBeneFactorNationality?NationalityId=' + NationalityId);
  }

  UpdateBeneFactor(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/UpdateBeneFactor', Model);
  }

  DeleteBeneFactor(BeneFactorId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/DeleteBeneFactor?BeneFactorId=' + BeneFactorId);
  }

  DeleteBeneFactorDetails(DetailsId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/DeleteBeneFactorDetails?DetailsId=' + DetailsId);
  }

  BeneFactorLogin(Code: number, BeneFactorName: string) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'BeneFactor/BeneFactorLogin?Code=' + Code + '&BeneFactorName=' + BeneFactorName);
  }

  isAuthenticated(): boolean {
    let currentUser = JSON.parse(localStorage.getItem('BeneFactorModel'));
    if (!currentUser || this.isLoginExpired(currentUser.loginDate))
      return false;

    return true;
  }

  loginRedirect(): void {
    localStorage.removeItem('BeneFactorModel');
    this.router.navigateByUrl('/benefactor-login');
  }

  isLoginExpired(date: any): boolean {
    let now = new Date().getTime();
    let loginDate = new Date(date);
    const expirationDate = loginDate.setMinutes(loginDate.getMinutes(), 1800);
    return expirationDate < now;
  }
}
