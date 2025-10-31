import { Component, OnInit } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../../../../Auth/auth.service';
import { FormService } from '../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { RoleCheckerDirective } from '../../../../../Directives/role-checker.directive';
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { SharedService } from '../../../../../Services/shared/shared.service';
import { PdfDownloadService } from '../../../../../Services/shared/pdf-download.service';
import { SearchReportModel } from '../../../../../Models/shared/SearchReportModel';
import { ZaDropDownFormControlComponent } from '../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-account-import-mony',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent,
    CommonModule, FormsModule, ReactiveFormsModule, NgbModule, RoleCheckerDirective, ZaInputWithLabelComponent,
    NgIf, NgFor, ZaDropDownFormControlComponent, NgxLoadingModule],
  templateUrl: './account-import-mony.component.html',
  styleUrl: './account-import-mony.component.css',
  providers: [DatePipe]
})
export class AccountImportMonyComponent {
  TitleList = ['مؤسسة زائر الخير', 'إدارة الحسابات', 'الايرادات'];
  filterList: FilterModel[] = [];
  AccountMoneyList: any[] = [];
  BeneFactors: any[] = [];
  BeneFactorTypes: any[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  TotalCount = 0;
  TotalImportValue = 0;
  ImportValueMonth = 0;
  isFilter = true;
  ActivityId: any;
  ImageFile: any;
  UserId: any;
  AccountId: any;
  PagingFilter: PagingFilterModel = {
    currentPage: 1,
    pageSize: 20,
    filterList: []
  };
  SearchReport: SearchReportModel = {
    reportType: 'AccountImportMonyExcel',
    headers: [],
    filterItems: []
  };
  formErrors = {
    beneFactorId: '',
    beneFactorTypeId: '',
    details: '',
    totalValue: '',
    insertDate: ''
  };

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder, private authService: AuthService,
    private formService: FormService, private taskService: TaskService, private datepipe: DatePipe, private sharedService: SharedService
    , private pdfService: PdfDownloadService
  ) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllAccountsImportMonyData();
    this.GetAllAccountsImportMonyFilters();
    this.GetAllImportExportMonyStatistics();
    this.GetAllBeneFactorData();
    this.GetAllBeneFactorTypes();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      beneFactorId: ['', Validators.required],
      beneFactorTypeId: ['', Validators.required],
      details: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      totalValue: ['', Validators.required],
      insertUser: null,
      insertDate: ['', Validators.required]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      beneFactorId: item?.beneFactorId ?? '0',
      beneFactorTypeId: item?.beneFactorTypeId,
      details: item?.details,
      totalValue: item?.totalValue,
      insertUser: this.UserId,
      insertDate: this.datepipe.transform(item?.insertDate, 'yyyy-MM-dd')
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('insertUser').setValue(this.UserId);
  }

  openItemModal(content: any, item: any) {
    this.ResetForm();
    if (item)
      this.FillEditForm(item);
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.AccountId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    })
  }

  GetAllAccountsImportMonyData() {
    this.showLoader = true;
    this.taskService.GetAllAccountsImportMonyData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.AccountMoneyList = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllAccountsImportMonyFilters() {
    this.taskService.GetAllAccountsImportMonyFilters(this.PagingFilter).subscribe(data => {
      this.filterList = data.results;
    });
  }

  GetAllBeneFactorData() {
    this.sharedService.GetAllBeneFactorsSelector().subscribe(data => {
      this.BeneFactors = data.results;
      let checked = this.BeneFactors.find(i => i.value == 0);
      if (!checked)
        this.BeneFactors.unshift({ value: '0', name: 'فاعل خير', isSelected: false, extraData: null });
    });
  }

  GetAllBeneFactorTypes() {
    this.sharedService.GetAllBeneFactorTypesSelector().subscribe(data => {
      this.BeneFactorTypes = data.results;
    });
  }

  GetAllImportExportMonyStatistics() {
    this.taskService.GetAllImportExportMonyStatistics(this.PagingFilter).subscribe(data => {
      this.TotalImportValue = data.results[0].importMoney ?? 0;
      this.ImportValueMonth = data.results[0].thisMonthImport ?? 0;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllAccountsImportMonyData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.SearchReport.filterItems = filterList;
    this.GetAllAccountsImportMonyData();
    this.GetAllImportExportMonyStatistics();
  }

  validateForm(): boolean {
    this.formService.markFormGroupTouched(this.ItemForm);
    if (this.ItemForm.valid) {
      return true;
    } else {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, false)
      return false;
    }
  }

  AddNewItem() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();
    if (!isValid)
      return;

    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.taskService.AddNewAccountsImportMony(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllAccountsImportMonyData();
          this.GetAllAccountsImportMonyFilters();
          this.GetAllImportExportMonyStatistics();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.showLoader = true;
      this.taskService.UpdateAccountsImportMony(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllAccountsImportMonyData();
          this.GetAllAccountsImportMonyFilters();
          this.GetAllImportExportMonyStatistics();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    }
  }

  DeleteItem() {
    this.showLoader = true;
    this.taskService.DeleteAccountsImportMony(this.AccountId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllAccountsImportMonyData();
        this.GetAllAccountsImportMonyFilters();
        this.GetAllImportExportMonyStatistics();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  NumbersOnly(key: any) {
    return this.formService.NumbersOnly(key);
  }

  DownloadExcelFile() {
    if (this.AccountMoneyList.length == 0) {
      this.toaster.warning('لا يوجد بيانات للتنزيل');
      return;
    }

    this.SearchReport.userName = this.authService.userName;
    this.SearchReport.reportType = 'AccountImportMonyExcel';
    let today = this.datepipe.transform(new Date(), 'yyyy-MM-dd');
    let fileName = 'الايرادات' + '_' + today;
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.xlsx').subscribe(data => {
      this.showLoader = false;
    });
  }
}
