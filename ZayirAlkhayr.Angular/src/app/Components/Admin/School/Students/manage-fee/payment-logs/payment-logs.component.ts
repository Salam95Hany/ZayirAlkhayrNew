import { Component } from '@angular/core';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../../Auth/auth.service';
import { NgxLoadingModule } from "ngx-loading";
import { ZaFiltersComponent } from "../../../../../../Shared/za-filters/za-filters.component";
import { ZaPaginationComponent } from "../../../../../../Shared/za-pagination/za-pagination.component";
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgFor, NgIf } from '@angular/common';
import { ArabicDateWithTimePipe } from '../../../../../../Pipes/arabic-date-with-time.pipe';
import { PosPrinterService } from '../../../../../../Services/school/pos-printer.service';
import { QzPrintService } from '../../../../../../Services/shared/qz-print.service';

@Component({
  selector: 'app-payment-logs',
  standalone: true,
  imports: [NgxLoadingModule, ZaFiltersComponent, ZaPaginationComponent, AdminBreadcrumbComponent, NgbModule, NgIf, NgFor, ArabicDateWithTimePipe],
  templateUrl: './payment-logs.component.html',
  styleUrl: './payment-logs.component.css'
})
export class PaymentLogsComponent {
  TitleList = ['مركز بشائر القرآن', 'إدارة الرسوم', 'سجل الدفعات'];
  Results: any[] = [];
  FilterList: FilterModel[] = [];
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  UserId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };

  constructor(private studentService: SchoolStudentService, private toaster: ToastrService, private authService: AuthService, private qzPrintService: QzPrintService, private posPrinterService: PosPrinterService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.GetAllStudentPaymentData();
    this.GetAllStudentPaymentFilters();
  }

  GetAllStudentPaymentData() {
    this.showLoader = true;
    this.studentService.GetAllStudentPaymentData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results.table;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllStudentPaymentFilters() {
    this.studentService.GetAllStudentPaymentFilters(this.PagingFilter).subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllStudentPaymentData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllStudentPaymentData();
    
  }

  Print(enrollmentId: number, paymentId: number) {
    this.showLoader = true;
    this.posPrinterService.GetStudentReceiptData(enrollmentId, paymentId).subscribe({
      next: async (arrayBuffer) => {
        this.showLoader = false;
        const base64Pdf = this.posPrinterService.arrayBufferToBase64(arrayBuffer);
        await this.qzPrintService.Print(base64Pdf);
      }
    });
  }
}
