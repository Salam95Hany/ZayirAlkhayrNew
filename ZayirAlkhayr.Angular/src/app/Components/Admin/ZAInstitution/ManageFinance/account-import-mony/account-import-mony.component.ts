import { Component, OnInit } from '@angular/core';
import { AdminBreadcrumbComponent } from '../../../shared/admin-breadcrumb/admin-breadcrumb.component';
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
import { DonationMethodPipe } from '../../../../../Pipes/donation-method.pipe';

@Component({
  selector: 'app-account-import-mony',
  standalone: true,
  imports: [AdminBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent,
    CommonModule, FormsModule, ReactiveFormsModule, NgbModule, RoleCheckerDirective, ZaInputWithLabelComponent,
    NgIf, NgFor, ZaDropDownFormControlComponent, NgxLoadingModule, DonationMethodPipe],
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
  AccountHeaders: any[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  TotalCount = 0;
  TotalImportValue = 0;
  ImportValueMonth = 0;
  TotalValue = 0;
  CurrentMonthMonyPercentage = 0;
  TotalMoneyPercentage = 0;
  selectedPercentageValue: string | null = null;
  isFilter = true;
  IsSelectedAll = false;
  ActivityId: any;
  ImageFile: any;
  UserId: any;
  AccountId: any;
  SelectedAccount: any;
  DonationMethods: any[] = [
    { value: 1, name: 'فودافون كاش' },
    { value: 2, name: 'انستا باي' },
    { value: 3, name: 'نقداً' }
  ];
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
    totalValue: '',
    insertDate: '',
    donationMethodId: ''
  };

  get isEditing(): boolean {
    return Number(this.ItemForm?.get('id')?.value) > 0;
  }

  get selectedCount(): number {
    return this.AccountMoneyList.filter(item => item.isSelected).length;
  }

  get activeFiltersCount(): number {
    return this.PagingFilter.filterList?.filter((filter: any) =>
      filter?.isChecked || filter?.checked || filter?.selected
    ).length ?? 0;
  }

  get hasSelectedPercentage(): boolean {
    return this.selectedPercentageValue !== null;
  }

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder, private authService: AuthService,
    private formService: FormService, private taskService: TaskService, private datepipe: DatePipe, private sharedService: SharedService
    , private pdfService: PdfDownloadService
  ) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetFinancialTransactionData();
    this.GetFinancialTransactionFilters();
    this.GetFinancialTransactionStatistics();
    this.GetAllBeneFactorData();
    this.GetAllBeneFactorTypes();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      beneFactorId: ['', Validators.required],
      beneFactorTypeId: ['', Validators.required],
      details: ['', [CustomValidators.regexPattern(RegexType.noSpace)]],
      totalValue: ['', Validators.required],
      donationMethodId: ['', Validators.required],
      insertUser: null,
      insertDate: ['', Validators.required],
      transactionType: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.patchValue({
      id: item.id,
      beneFactorId: item?.beneFactorId?.toString() ?? '0',
      beneFactorTypeId: item?.beneFactorTypeId?.toString() ?? '',
      details: item?.details ?? '',
      totalValue: item?.totalValue,
      donationMethodId: item?.donationMethodId?.toString() ?? '',
      insertUser: this.UserId,
      insertDate: this.datepipe.transform(item?.insertDate, 'yyyy-MM-dd'),
      transactionType: null
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
    this.SelectedAccount = item;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    })
  }

  OpenPdfFileItemModal(content: any) {
    this.SearchReport.headers = this.pdfService.ConverHeaderToPDFModel(this.AccountHeaders);
    this.modalService.open(content, {
      size: 'lg',
      scrollable: true,
      centered: true
    });
  }

  GetFinancialTransactionData() {
    this.showLoader = true;
    this.taskService.GetFinancialTransactionData(this.PagingFilter, 'Income').subscribe(data => {
      this.showLoader = false;
      this.AccountMoneyList = data.results.table;
      this.AccountHeaders = data.results.table1;
      this.TotalCount = data.totalCount;
      this.onInputSelecetAll(this.IsSelectedAll);
    });
  }

  GetFinancialTransactionFilters() {
    this.taskService.GetFinancialTransactionFilters(this.PagingFilter, 'Income').subscribe(data => {
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

  GetFinancialTransactionStatistics() {
    this.taskService.GetFinancialTransactionStatistics(this.PagingFilter, 'Income').subscribe(data => {
      this.TotalImportValue = data.results[0].totalMoney ?? 0;
      this.ImportValueMonth = data.results[0].currentMonthMony ?? 0;
      this.TotalValue = data.results[0].totalMoney ?? 0;
      this.CurrentMonthMonyPercentage = data.results[0].currentMonthMonyPercentage ?? 0;
      this.TotalMoneyPercentage = data.results[0].totalMoneyPercentage ?? 0;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetFinancialTransactionData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.PagingFilter.currentPage = 1;
    this.SearchReport.filterItems = filterList;
    const percentageFilter = this.findSelectedPercentageFilter(filterList);
    this.selectedPercentageValue = percentageFilter
      ? String(percentageFilter.itemKey ?? percentageFilter.itemId ?? '').replace('%', '').trim()
      : null;
    this.GetFinancialTransactionData();
    this.GetFinancialTransactionStatistics();
  }

  private findSelectedPercentageFilter(filterList: FilterModel[] = []): FilterModel | undefined {
    for (const filter of filterList) {
      if (filter.categoryName?.toLowerCase() === 'percentage' && filter.isChecked) {
        return filter;
      }

      const selectedChild = this.findSelectedPercentageFilter(filter.filterItems ?? []);
      if (selectedChild) {
        return selectedChild;
      }
    }

    return undefined;
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

    this.ItemForm.patchValue({ transactionType: 'Income' });

    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.taskService.AddNewFinancialTransaction(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetFinancialTransactionData();
          this.GetFinancialTransactionFilters();
          this.GetFinancialTransactionStatistics();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.showLoader = true;
      this.taskService.UpdateFinancialTransaction(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetFinancialTransactionData();
          this.GetFinancialTransactionFilters();
          this.GetFinancialTransactionStatistics();
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
    this.taskService.DeleteFinancialTransaction(this.AccountId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetFinancialTransactionData();
        this.GetFinancialTransactionFilters();
        this.GetFinancialTransactionStatistics();
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

  DownloadPdfFile() {
    if (this.AccountMoneyList.length == 0) {
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

    let today = this.datepipe.transform(new Date(), 'yyyy-MM-dd');
    let fileName = 'الايرادات' + '_' + today;
    this.SearchReport.headers = this.SearchReport.headers.filter(i => i.isSelected);
    this.SearchReport.rowCount = 0;
    this.SearchReport.reportType = 'AccountImportMonyPdf';
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.pdf').subscribe(data => {
      this.showLoader = false;
    });
    this.modalService.dismissAll();
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
    this.SearchReport.headers = this.pdfService.ConverHeaderToPDFModel(this.AccountHeaders);
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.xlsx').subscribe(data => {
      this.showLoader = false;
    });
  }

  onInputSelecetAll(isSelected: boolean) {
    this.AccountMoneyList.forEach(item => item.isSelected = isSelected);
    if (this.AccountMoneyList.every(i => !i.isSelected))
      this.TotalValue = this.TotalImportValue;
    else {
      this.TotalValue = 0;
      this.AccountMoneyList.forEach(item => {
        if (item.isSelected)
          this.TotalValue += item.totalValue;
      });
    }
  }

  onInputSelected() {
    if (this.AccountMoneyList.every(i => !i.isSelected))
      this.TotalValue = this.TotalImportValue;
    else {
      this.TotalValue = 0;
      this.AccountMoneyList.forEach(item => {
        if (item.isSelected)
          this.TotalValue += item.totalValue;
      });
    }
  }

  trackByAccount(_: number, item: any): number {
    return item.id;
  }
}
