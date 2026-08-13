import { Component, OnInit } from '@angular/core';
import { AdminBreadcrumbComponent } from '../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { ZaPaginationComponent } from "../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { BenefactorService } from '../../../../../Services/zainstitution/benefactor.service';
import { FormService } from '../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { AuthService } from '../../../../../Auth/auth.service';
import { RoleCheckerDirective } from '../../../../../Directives/role-checker.directive';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-benefactor-nationalities',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminBreadcrumbComponent, ZaPaginationComponent, RoleCheckerDirective,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule, NgxLoadingModule],
  templateUrl: './benefactor-nationalities.component.html',
  styleUrl: './benefactor-nationalities.component.css'
})
export class BenefactorNationalitiesComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'إدارة المتبرعين', 'جنسيات المتبرعين'];
  filterList: FilterModel[] = [];
  ItemForm: FormGroup;
  showLoader = false;
  isFilter = true;
  UserId: any;
  NationalityId: number;
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

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder,
    private formService: FormService, private benefactorService: BenefactorService, private authService: AuthService) {

  }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllBeneFactorNationalities();
    this.GetAllBeneFactorNationalityFilters();
  }

  get isEditing(): boolean {
    return Number(this.ItemForm?.get('id')?.value) > 0;
  }

  get activeFiltersCount(): number {
    return this.pagingFilterModel.filterList?.length ?? 0;
  }

  trackByNationality(_index: number, item: any): number {
    return item.id;
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
    })
  }

  openDeleteItemModal(content: any, item: any) {
    this.NationalityId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    })
  }

  GetAllBeneFactorNationalities() {
    this.showLoader = true;
    this.benefactorService.GetAllBeneFactorNationalities(this.pagingFilterModel).subscribe(data => {
      this.showLoader = false;
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

  GetAllBeneFactorNationalityFilters() {
    this.benefactorService.GetAllBeneFactorNationalityFilters().subscribe(data => {
      this.filterList = data.results;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetAllBeneFactorNationalities();
  }

  filterChecked(filterList: FilterModel[]) {
    this.pagingFilterModel.filterList = filterList;
    this.pagingFilterModel.currentPage = 1;
    this.GetAllBeneFactorNationalities();
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
      this.benefactorService.AddNewBeneFactorNationality(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllBeneFactorNationalities();
          this.GetAllBeneFactorNationalityFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.benefactorService.UpdateBeneFactorNationality(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllBeneFactorNationalities();
          this.GetAllBeneFactorNationalityFilters();
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
    this.benefactorService.DeleteBeneFactorNationality(this.NationalityId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllBeneFactorNationalities();
        this.GetAllBeneFactorNationalityFilters();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
