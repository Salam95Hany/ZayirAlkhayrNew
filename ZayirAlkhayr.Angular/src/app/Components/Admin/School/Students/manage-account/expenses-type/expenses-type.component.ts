import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../../Shared/za-pagination/za-pagination.component';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { ZaFiltersComponent } from '../../../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxLoadingModule } from 'ngx-loading';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { FormService } from '../../../../../../Services/shared/form.service';
import { AuthService } from '../../../../../../Auth/auth.service';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { SchoolAccountService } from '../../../../../../Services/school/school-account.service';

@Component({
  selector: 'app-expenses-type',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminBreadcrumbComponent, ZaPaginationComponent, RoleCheckerDirective,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule, NgxLoadingModule],
  templateUrl: './expenses-type.component.html',
  styleUrl: './expenses-type.component.css'
})
export class ExpensesTypeComponent {
  TitleList = ['مؤسسة زائر الخير', 'إدارة المتبرعين', 'أنواع التبرع'];
  filterList: FilterModel[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  isFilter = true;
  UserId: any;
  TypeId: number;
  pagingFilterModel: PagingFilterModel = {
    currentPage: 1,
    pageSize: 20,
    filterList: []
  };
  pagedResponseModel: PagedResponseModel<any[]> = {
    totalCount: 0,
    results: [],
  };
  formErrors = {
    name: ''
  };

  get isEditing(): boolean {
    return Number(this.ItemForm?.get('id')?.value) > 0;
  }

  get activeFiltersCount(): number {
    return this.pagingFilterModel.filterList?.filter((filter: any) =>
      filter?.isChecked || filter?.checked || filter?.selected
    ).length ?? 0;
  }

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder,
    private formService: FormService, private accountService: SchoolAccountService, private authService: AuthService) {

  }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllExpensesTypes();
    this.GetAllExpensesTypeFilters();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      name: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      InsertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      name: item?.name,
      InsertUser: this.UserId,
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('InsertUser').setValue(this.UserId);
  }

  openItemModal(content: any, item: any) {
    this.ResetForm();
    if (item)
      this.FillEditForm(item);

    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.TypeId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    })
  }

  GetAllExpensesTypes() {
    this.showLoader = true;
    this.accountService.GetAllExpensesTypes(this.pagingFilterModel).subscribe(data => {
      this.showLoader = false;
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

  GetAllExpensesTypeFilters() {
    this.accountService.GetAllExpensesTypeFilters().subscribe(data => {
      this.filterList = data.results;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetAllExpensesTypes();
  }

  filterChecked(filterList: FilterModel[]) {
    this.pagingFilterModel.filterList = filterList;
    this.pagingFilterModel.currentPage = 1;
    this.GetAllExpensesTypes();
  }

  trackByType(_: number, item: any): number {
    return item.id;
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
      this.accountService.AddNewExpensesType(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllExpensesTypes();
          this.GetAllExpensesTypeFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.accountService.UpdateExpensesType(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllExpensesTypes();
          this.GetAllExpensesTypeFilters();
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
    this.accountService.DeleteExpensesType(this.TypeId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllExpensesTypes();
        this.GetAllExpensesTypeFilters();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
