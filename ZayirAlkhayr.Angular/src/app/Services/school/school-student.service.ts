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
  
    GetUpdateStudentLookups(StudentId: number) {
      return this.http.get<ApiResponseModel<UpdateStudentLookups>>(this.apiURL + 'Student/GetUpdateStudentLookups?StudentId=' + StudentId);
    }
  
    AddNewStudent(Model: AddStudentModel) {
      return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Student/AddNewStudent', Model);
    }
  
    UpdateStudent(Model: AddStudentModel) {
      return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Student/UpdateStudent', Model);
    }
  
    DeleteStudent(StudentId: number) {
      return this.http.get<ApiResponseModel<any>>(this.apiURL + 'Student/DeleteStudent?StudentId=' + StudentId);
    }
}
