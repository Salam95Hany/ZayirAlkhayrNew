import { Component, Injector } from '@angular/core';
import { NgxLoadingModule } from "ngx-loading";
import { ZaPaginationComponent } from "../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../Shared/za-filters/za-filters.component";
import { ZaBreadcrumbComponent } from "../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaEmptyDataComponent } from '../../../../Shared/za-empty-data/za-empty-data.component';
import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModal, NgbModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { RouterModule } from '@angular/router';
import { RoleCheckerDirective } from '../../../../Directives/role-checker.directive';
import { FilterModel } from '../../../../Models/shared/FilterModel';
import { SearchReportModel } from '../../../../Models/shared/SearchReportModel';
import { PagingFilterModel } from '../../../../Models/shared/PagingFilterModel ';
import { SchoolStudentService } from '../../../../Services/school/school-student.service';
import { ToastrService } from 'ngx-toastr';
import { FormService } from '../../../../Services/shared/form.service';
import { AuthService } from '../../../../Auth/auth.service';
import { PdfDownloadService } from '../../../../Services/shared/pdf-download.service';
import { StudentSidepanelComponent } from '../student-sidepanel/student-sidepanel.component';

@Component({
  selector: 'app-student',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent,
    CommonModule, FormsModule, ReactiveFormsModule, NgbModule, RouterModule, RoleCheckerDirective,
    NgIf, NgFor, NgxLoadingModule],
  templateUrl: './student.component.html',
  styleUrl: './student.component.css',
  providers: [DatePipe]
})
export class StudentComponent {
  TitleList = ['مركز بشائر القرآن', 'إدارة الطلاب', 'الطلاب'];
  Results: any[] = [];
  FilterList: FilterModel[] = [];
  SearchReport: SearchReportModel = {
    reportType: 'StudentPdf',
    headers: [],
    filterItems: []
  };
  StudentHeaders: any[] = [];
  StudentId: any;
  ParentId: any;
  UserName: any;
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  RowCount = 25;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  }

  constructor(private schoolService: SchoolStudentService, private offcanvasService: NgbOffcanvas, private injector: Injector,
    private modalService: NgbModal, private pdfService: PdfDownloadService, private toaster: ToastrService, private datePipe: DatePipe
    , private formService: FormService, private authSerive: AuthService
  ) {

  }

  ngOnInit(): void {
    this.UserName = this.authSerive.userName;
    this.GetAllStudentData();
    this.GetAllStudentFilter();
  }

  openUpdateStudentSidePanel(item: any, UpdateMode: boolean, DetailsMode: boolean) {
    const injector = Injector.create({
      providers: [
        { provide: 'StudentId', useValue: item.id },
        { provide: 'StudentName', useValue: item.studentName },
        { provide: 'AcademicStage', useValue: item.academicStageName },
        { provide: 'UpdateMode', useValue: UpdateMode },
        { provide: 'DetailsMode', useValue: DetailsMode }
      ],
      parent: this.injector
    });

    const ref = this.offcanvasService.open(StudentSidepanelComponent, {
      injector: injector,
      position: 'end'
    });

    ref.dismissed.subscribe((result: any) => {
      if (result?.reload == 'reload') {
        this.GetAllStudentData();
        this.GetAllStudentFilter();
      }
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.StudentId = item.id;
    this.ParentId = item.parentId;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  OpenPdfFileItemModal(content: any) {
    this.SearchReport.headers = this.pdfService.ConverHeaderToPDFModel(this.StudentHeaders);
    this.RowCount = 15;
    this.modalService.open(content, {
      size: 'lg',
      scrollable: true,
      centered: true
    });
  }



  GetAllStudentData() {
    this.showLoader = true;
    this.schoolService.GetAllStudentData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results.table;
      this.StudentHeaders = data.results.table1;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllStudentFilter() {
    this.schoolService.GetAllStudentFilter(this.PagingFilter).subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllStudentData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.SearchReport.filterItems = filterList;
    this.PagingFilter.currentPage = 1;
    this.GetAllStudentData();
  }

  NumbersOnly(key: any) {
    return this.formService.NumbersOnly(key);
  }

  DeleteItem() {
    this.showLoader = true;
    this.schoolService.DeleteStudent(this.ParentId,this.StudentId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllStudentData();
        this.GetAllStudentFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    })
  }

  DownloadPdfFile() {
    if (this.Results.length == 0) {
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
    if (this.Results.length == 0) {
      this.toaster.warning('لا يوجد بيانات للتنزيل');
      return;
    }

    let today = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    let fileName = 'الطلاب' + '_' + today;
    this.SearchReport.headers = this.pdfService.ConverHeaderToPDFModel(this.StudentHeaders);
    this.SearchReport.userName = this.UserName;
    this.SearchReport.reportType = 'FamilyStatusExcel';
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.xlsx').subscribe(data => {
      this.showLoader = false;
    });
  }
}
