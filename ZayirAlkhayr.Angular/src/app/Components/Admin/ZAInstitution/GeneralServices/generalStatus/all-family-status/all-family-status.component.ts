import { Component, Injector } from '@angular/core';
import { ZaPaginationComponent } from "../../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../../Shared/za-filters/za-filters.component";
import { ZaBreadcrumbComponent } from "../../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaLoaderComponent } from "../../../../../../Shared/za-loader/za-loader.component";
import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { ToastrService } from 'ngx-toastr';
import { FormService } from '../../../../../../Services/shared/form.service';
import { RouterModule } from '@angular/router';
import { SearchReportModel } from '../../../../../../Models/shared/SearchReportModel';
import { GeneralStatusService } from '../../../../../../Services/zainstitution/general-status.service';
import { PdfDownloadService } from '../../../../../../Services/shared/pdf-download.service';
import { AuthService } from '../../../../../../Auth/auth.service';
import { FamilyStatusSidepanelComponent } from '../family-status-sidepanel/family-status-sidepanel.component';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';

@Component({
  selector: 'app-all-family-status',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent,
    CommonModule, FormsModule, ReactiveFormsModule, NgbModule, RouterModule,RoleCheckerDirective,
    NgIf, NgFor, ZaLoaderComponent],
  templateUrl: './all-family-status.component.html',
  styleUrl: './all-family-status.component.css',
  providers: [DatePipe]
})
export class AllFamilyStatusComponent {
  TitleList = ['مؤسسة زائر الخير', 'خدمات اجتماعية', 'حالات عامة'];
  FamilyStatusData: any[] = [];
  FilterList: FilterModel[] = [];
  SearchReport: SearchReportModel = {
    reportType: 'FamilyStatusPdf',
    headers: [],
    filterItems: []
  };
  FamilyStatusHeaders: any[] = [];
  FamilyStatusId: any
  UserName: any;
  showLoader = false;
  isFilter = false;
  TotalCount = 0;
  RowCount = 25;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  }

  constructor(private generalService: GeneralStatusService, private offcanvasService: NgbOffcanvas, private injector: Injector,
    private modalService: NgbModal, private pdfService: PdfDownloadService, private toaster: ToastrService, private datePipe: DatePipe
    , private formService: FormService, private authSerive: AuthService
  ) {

  }

  ngOnInit(): void {
    this.UserName = this.authSerive.userName;
    this.GetAllFamilyStatusData();
    this.GetAllFamilyStatusFilter();
  }

  openUpdateFamilyStatusSidePanel(item: any, UpdateMode: boolean, DetailsMode: boolean) {
    const injector = Injector.create({
      providers: [
        { provide: 'FamilyStatusId', useValue: item.id },
        { provide: 'FamilyStatusCode', useValue: item.code },
        { provide: 'FamilyStatusName', useValue: item.statusName },
        { provide: 'UpdateMode', useValue: UpdateMode },
        { provide: 'DetailsMode', useValue: DetailsMode }
      ],
      parent: this.injector
    });

    const ref = this.offcanvasService.open(FamilyStatusSidepanelComponent, {
      injector: injector,
      position: 'end'
    });

    ref.dismissed.subscribe((result: any) => {
      if (result?.reload == 'reload') {
        this.GetAllFamilyStatusData();
        this.GetAllFamilyStatusFilter();
      }
    });
  }

  openDeleteItemModal(content: any, familyStatusId: any) {
    this.FamilyStatusId = familyStatusId;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  OpenPdfFileItemModal(content: any) {
    this.SearchReport.headers = this.pdfService.ConverHeaderToPDFModel(this.FamilyStatusHeaders);
    this.RowCount = 15;
    this.modalService.open(content, {
      size: 'lg',
      scrollable: true,
      centered: true
    });
  }



  GetAllFamilyStatusData() {
    this.showLoader = true;
    this.generalService.GetAllFamilyStatusData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.FamilyStatusData = data.results.table;
      this.FamilyStatusHeaders = data.results.table1;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllFamilyStatusFilter() {
    this.generalService.GetAllFamilyStatusFilter(this.PagingFilter).subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllFamilyStatusData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.SearchReport.filterItems = filterList;
    this.PagingFilter.currentPage = 1;
    this.GetAllFamilyStatusData();
  }

  NumbersOnly(key: any) {
    return this.formService.NumbersOnly(key);
  }

  DeleteItem() {
    this.showLoader = true;
    this.generalService.DeleteFamilyStatus(this.FamilyStatusId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllFamilyStatusData();
        this.GetAllFamilyStatusFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    })
  }

  DownloadPdfFile() {
    if (this.FamilyStatusData.length == 0) {
      this.toaster.warning('لا يوجد بيانات للتنزيل');
      return;
    }

    let checked = this.SearchReport.headers.filter(i => i.isSelected);
    let isAllowSummation = this.SearchReport.headers.filter(i => i.isAllowSummation);
    if (isAllowSummation.length > 1) {
      this.toaster.warning('لا يمكن اختيار جمع قيم العامود الا لعامود واحد فقط');
      return;
    }

    if (checked.length == 0) {
      this.toaster.warning('اختر عامود واحد على الاقل');
      return;
    }

    if (checked.length > 6) {
      this.toaster.warning('لا يمكن اختيار أكثر من 6 أعمدة');
      return;
    }

    if (this.RowCount == 0 || !this.RowCount) {
      this.toaster.warning('أدخل عدد الاسطر');
      return;
    }

    let today = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    let fileName = 'الحالات' + '_' + today;
    this.SearchReport.headers = this.SearchReport.headers.filter(i => i.isSelected);
    this.SearchReport.rowCount = this.RowCount;
    this.SearchReport.reportType = 'FamilyStatusPdf';
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.pdf').subscribe(data => {
      this.showLoader = false;
    });
    this.modalService.dismissAll();
  }

  DownloadExcelFile() {
    if (this.FamilyStatusData.length == 0) {
      this.toaster.warning('لا يوجد بيانات للتنزيل');
      return;
    }

    let today = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    let fileName = 'الحالات' + '_' + today;
    this.SearchReport.headers = this.pdfService.ConverHeaderToPDFModel(this.FamilyStatusHeaders);
    this.SearchReport.userName = this.UserName;
    this.SearchReport.reportType = 'FamilyStatusExcel';
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.xlsx').subscribe(data => {
      this.showLoader = false;
    });
  }
}
