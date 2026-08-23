import { Injectable } from '@angular/core';
import { StudentPrintSlot, TicketStudent } from '../../Models/school/student/StudentPrintSlot';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagingFilterModel } from '../../Models/shared/PagingFilterModel ';
import { ApiResponseModel } from '../../Models/shared/ErrorResponseModel';

@Injectable({ providedIn: 'root' })
export class StudentTicketPrintService {
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  // ============================= StudentTicket ==============================

  GetAllStudentTicketData(PagingFilter: PagingFilterModel) {
    return this.http.post<ApiResponseModel<TicketStudent[]>>(this.apiURL + 'StudentTicket/GetAllStudentTicketData', PagingFilter);
  }

  GetAcademicStages() {
    return this.http.get<any[]>(this.apiURL + 'StudentTicket/GetAcademicStages');
  }

  GetAcademicYear() {
    return this.http.get<any[]>(this.apiURL + 'StudentTicket/GetAcademicYear');
  }
  generateAndPrintPdf(slots: StudentPrintSlot[]): void {
    console.info('[StudentTicketPrintService] PDF print payload', slots);
  }
}
