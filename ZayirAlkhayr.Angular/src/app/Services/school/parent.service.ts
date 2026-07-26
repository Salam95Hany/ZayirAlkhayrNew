import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';

@Injectable({
  providedIn: 'root'
})
export class ParentService {
  apiURL = environment.apiUrl;

  constructor(private http: HttpClient) { }


  // ============================= Parent ==============================

  GetAllParentData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Parent/GetAllParentData', PagingFilter);
  }

  AddNewParent(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Parent/AddNewParent', Model);
  }

  UpdateParent(Model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Parent/UpdateParent', Model);
  }

  DeleteParent(ParentId: number) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'Parent/DeleteParent?ParentId=' + ParentId);
  }

  GetParents() {
    return this.http.get<any[]>(this.apiURL + 'Parent/GetParents');
  }
}
