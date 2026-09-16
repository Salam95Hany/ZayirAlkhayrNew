import { Component } from '@angular/core';
import { DonationMethodPipe } from '../../../../../../Pipes/donation-method.pipe';
import { NgxLoadingModule } from 'ngx-loading';
import { ZaDropDownFormControlComponent } from '../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaPaginationComponent } from '../../../../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../../../../Shared/za-filters/za-filters.component';
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { SearchReportModel } from '../../../../../../Models/shared/SearchReportModel';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../../Auth/auth.service';
import { FormService } from '../../../../../../Services/shared/form.service';
import { PdfDownloadService } from '../../../../../../Services/shared/pdf-download.service';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { SchoolAccountService } from '../../../../../../Services/school/school-account.service';

@Component({
  selector: 'app-school-expenses',
  standalone: true,
  imports: [AdminBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent,
    CommonModule, FormsModule, ReactiveFormsModule, NgbModule, RoleCheckerDirective, ZaInputWithLabelComponent,
    NgIf, NgFor, ZaDropDownFormControlComponent, NgxLoadingModule, DonationMethodPipe],
  templateUrl: './school-expenses.component.html',
  styleUrl: './school-expenses.component.css',
  providers: [DatePipe]
})
export class SchoolExpensesComponent {
  TitleList = ['مؤسسة زائر الخير', 'إدارة الحسابات', 'المصروفات'];
  filterList: FilterModel[] = [];
  ExpensesList: any[] = [];
  ExpensesTypes: any[] = [];
  AccountHeaders: any[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  TotalCount = 0;
  TotalExportValue = 0;
  ExportValueMonth = 0;
  TotalValue = 0;
  CurrentMonthMonyPercentage = 0;
  TotalMoneyPercentage = 0;
  selectedPercentageValue: string | null = null;
  IsSelectedAll = false;
  isFilter = true;
  UserId: any;
  ExpensesId: any;
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
    reportType: 'AccountExportMonyExcel',
    headers: [],
    filterItems: []
  };
  formErrors = {
    expensesTypeId: '',
    totalValue: '',
    expensesDate: '',
    donationMethodId: ''
  };

  get isEditing(): boolean {
    return Number(this.ItemForm?.get('id')?.value) > 0;
  }

  get selectedCount(): number {
    return this.ExpensesList.filter(item => item.isSelected).length;
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
    private formService: FormService, private accountService: SchoolAccountService, private datepipe: DatePipe
    , private pdfService: PdfDownloadService
  ) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetExpensesData();
    this.GetExpensesFilters();
    this.GetExpensesStatistics();
    this.GetAllExpensesTypesSelector();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      expensesTypeId: ['', Validators.required],
      details: ['', [CustomValidators.regexPattern(RegexType.noSpace)]],
      totalValue: ['', Validators.required],
      donationMethodId: ['', Validators.required],
      insertUser: null,
      expensesDate: ['', Validators.required]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.patchValue({
      id: item.id,
      expensesTypeId: item?.expensesTypeId?.toString() ?? '',
      donationMethodId: item?.donationMethodId?.toString() ?? '',
      details: item?.details ?? '',
      totalValue: item?.totalValue,
      insertUser: this.UserId,
      expensesDate: this.datepipe.transform(item?.expensesDate, 'yyyy-MM-dd')
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
    this.ExpensesId = item.id;
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

  GetExpensesData() {
    this.showLoader = true;
    this.accountService.GetExpensesData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.ExpensesList = data.results.table;
      this.AccountHeaders = data.results?.table1?.filter(i => i.displayValue != 'DonationMethod') ?? [];
      this.TotalCount = data.totalCount;
      this.onInputSelecetAll(this.IsSelectedAll);
    });
  }

  GetExpensesFilters() {
    this.accountService.GetExpensesFilters(this.PagingFilter).subscribe(data => {
      this.filterList = data.results;
    });
  }

  GetAllExpensesTypesSelector() {
    this.accountService.GetAllExpensesTypesSelector().subscribe(data => {
      this.ExpensesTypes = data.results;
    });
  }

  GetExpensesStatistics() {
    this.accountService.GetExpensesStatistics(this.PagingFilter).subscribe(data => {
      this.TotalExportValue = data.results[0].totalMoney ?? 0;
      this.ExportValueMonth = data.results[0].currentMonthMony ?? 0;
      this.TotalValue = data.results[0].totalMoney ?? 0;
      this.CurrentMonthMonyPercentage = data.results[0].currentMonthMonyPercentage ?? 0;
      this.TotalMoneyPercentage = data.results[0].totalMoneyPercentage ?? 0;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetExpensesData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.PagingFilter.currentPage = 1;
    this.SearchReport.filterItems = filterList;
    const percentageFilter = this.findSelectedPercentageFilter(filterList);
    this.selectedPercentageValue = percentageFilter
      ? String(percentageFilter.itemKey ?? percentageFilter.itemId ?? '').replace('%', '').trim()
      : null;
    this.GetExpensesData();
    this.GetExpensesStatistics();
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

    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.accountService.AddNewExpenses(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetExpensesData();
          this.GetExpensesFilters();
          this.GetExpensesStatistics();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.showLoader = true;
      this.accountService.UpdateExpenses(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetExpensesData();
          this.GetExpensesFilters();
          this.GetExpensesStatistics();
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
    this.accountService.DeleteExpenses(this.ExpensesId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetExpensesData();
        this.GetExpensesFilters();
        this.GetExpensesStatistics();
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
    if (this.ExpensesList.length == 0) {
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
    let fileName = 'المصروفات' + '_' + today;
    this.SearchReport.headers = this.SearchReport.headers.filter(i => i.isSelected);
    this.SearchReport.rowCount = 0;
    this.SearchReport.reportType = 'AccountExportMonyPdf';
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.pdf').subscribe(data => {
      this.showLoader = false;
    });
    this.modalService.dismissAll();
  }

  DownloadExcelFile() {
    if (this.ExpensesList.length == 0) {
      this.toaster.warning('لا يوجد بيانات للتنزيل');
      return;
    }

    this.SearchReport.userName = this.authService.userNameAr;
    this.SearchReport.reportType = 'AccountExportMonyExcel';
    let today = this.datepipe.transform(new Date(), 'yyyy-MM-dd');
    let fileName = 'المصروفات' + '_' + today;
    this.SearchReport.headers = this.pdfService.ConverHeaderToPDFModel(this.AccountHeaders);
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.xlsx').subscribe(data => {
      this.showLoader = false;
    });
  }

  onInputSelecetAll(isSelected: boolean) {
    this.ExpensesList.forEach(item => item.isSelected = isSelected);
    if (this.ExpensesList.every(i => !i.isSelected))
      this.TotalValue = this.TotalExportValue;
    else {
      this.TotalValue = 0;
      this.ExpensesList.forEach(item => {
        if (item.isSelected)
          this.TotalValue += item.totalValue;
      });
    }
  }

  onInputSelected() {
    if (this.ExpensesList.every(i => !i.isSelected))
      this.TotalValue = this.TotalExportValue;
    else {
      this.TotalValue = 0;
      this.ExpensesList.forEach(item => {
        if (item.isSelected)
          this.TotalValue += item.totalValue;
      });
    }
  }

  trackByAccount(_: number, item: any): number {
    return item.id;
  }
}
