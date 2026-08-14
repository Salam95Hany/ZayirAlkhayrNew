import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminBreadcrumbComponent } from '../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../Shared/za-pagination/za-pagination.component';
import { RoleCheckerDirective } from '../../../../../Directives/role-checker.directive';
import { ZaFiltersComponent } from '../../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxLoadingModule } from 'ngx-loading';
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../Auth/auth.service';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { FormService } from '../../../../../Services/shared/form.service';

@Component({
  selector: 'app-percentage',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminBreadcrumbComponent, ZaPaginationComponent, RoleCheckerDirective,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule, NgxLoadingModule],
  templateUrl: './percentage.component.html',
  styleUrl: './percentage.component.css'
})
export class PercentageComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'إدارة الحسابات', 'النسب المئوية'];
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
    value: ''
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
    private formService: FormService, private taskService: TaskService, private authService: AuthService) {

  }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllPercentageData();
    this.GetAllPercentageFilters();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      value: ['', [Validators.required]],
      insertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      value: item?.name,
      insertUser: this.UserId,
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

  GetAllPercentageData() {
    this.showLoader = true;
    this.taskService.GetAllPercentageData(this.pagingFilterModel).subscribe(data => {
      this.showLoader = false;
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

   GetAllPercentageFilters() {
    this.taskService.GetAllPercentageFilters().subscribe(data => {
      this.filterList = data.results;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetAllPercentageData();
  }

  filterChecked(filterList: FilterModel[]) {
    this.pagingFilterModel.filterList = filterList;
    this.pagingFilterModel.currentPage = 1;
    this.GetAllPercentageData();
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
      this.taskService.AddNewPercentage(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllPercentageData();
          this.GetAllPercentageFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.taskService.UpdatePercentage(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllPercentageData();
          this.GetAllPercentageFilters();
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
    this.taskService.DeletePercentage(this.TypeId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllPercentageData();
        this.GetAllPercentageFilters();
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
}
