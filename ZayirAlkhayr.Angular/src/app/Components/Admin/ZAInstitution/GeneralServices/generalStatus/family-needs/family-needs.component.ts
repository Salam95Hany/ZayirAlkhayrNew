import { Component, OnInit } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../../Shared/za-filters/za-filters.component";
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStatusService } from '../../../../../../Services/zainstitution/general-status.service';
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { SharedService } from '../../../../../../Services/shared/shared.service';
import { ZaDropDownFormControlComponent } from '../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { AuthService } from '../../../../../../Auth/auth.service';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-family-needs',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent, NgbModule,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, ZaDropDownFormControlComponent, RoleCheckerDirective, NgxLoadingModule],
  templateUrl: './family-needs.component.html',
  styleUrl: './family-needs.component.css'
})
export class FamilyNeedsComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'خدمات اجتماعية', 'الاحتياجات'];
  FamilyNeedsData: any[] = [];
  FamilyCategoriesData: any[] = [];
  CategoryList: any[] = [];
  NeedFilterList: FilterModel[] = [];
  CategoryFilterList: FilterModel[] = [];
  showLoader = false;
  NeedIsFilter = true;
  CategoryIsFilter = true;
  CategoryValidation = false;
  CategoryName = 'الفئات';
  CategoryId: any;
  NeedId: any;
  NeedTotalCount = 0;
  CategoryTotalCount = 0;
  NeedForm: FormGroup;
  CategoryForm: FormGroup;
  UserId: any;
  NeedsPagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  CategoriesPagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };

  needFormErrors = {
    name: '',
    categoryId: ''
  };

  categoryFormErrors = {
    name: ''
  };

  constructor(private modalService: NgbModal, private generalStatusService: GeneralStatusService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService, private sharedService: SharedService,private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.NeedFormInit();
    this.CategoryFormInit();
    this.GetAllFamilyNeedCategoriesSelector();
    this.GetAllFamilyNeedTypesData();
    this.GetAllFamilyNeedTypesFilters();
    this.GetAllFamilyNeedCategoriesData();
    this.GetAllFamilyNeedCategoriesFilters();
  }

  NeedFormInit() {
    this.NeedForm = this.fb.group({
      id: 0,
      categoryId: ['', [Validators.required]],
      name: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      InsertUser: null
    });

    this.NeedForm.valueChanges.subscribe((data) => {
      this.needFormErrors = this.formService.validateForm(this.NeedForm, this.needFormErrors, true);
    });
  }

  CategoryFormInit() {
    this.CategoryForm = this.fb.group({
      id: 0,
      name: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      InsertUser: null
    });

    this.CategoryForm.valueChanges.subscribe((data) => {
      this.categoryFormErrors = this.formService.validateForm(this.CategoryForm, this.categoryFormErrors, true);
    });
  }

  FillEditNeedForm(item: any) {
    debugger;
    this.NeedForm.setValue({
      id: item.id,
      categoryId: item.categoryId.toString(),
      name: item?.name,
      InsertUser: this.UserId,
    });
  }

  FillEditCategoryForm(item: any) {
    this.CategoryForm.setValue({
      id: item.id,
      name: item?.name,
      InsertUser: this.UserId,
    });
  }

  ResetNeedForm() {
    this.NeedForm.reset();
    this.NeedForm.get('id').setValue(0);
    this.NeedForm.get('InsertUser').setValue(this.UserId);
  }

  ResetCategoryForm() {
    this.CategoryForm.reset();
    this.CategoryForm.get('id').setValue(0);
    this.CategoryForm.get('InsertUser').setValue(this.UserId);
  }

  openNeedAddItemModal(content: any, item: any) {
    this.ResetNeedForm();
    this.CategoryValidation = false;
    if (item)
      this.FillEditNeedForm(item);
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openCategoryAddItemModal(content: any, item: any) {
    this.ResetCategoryForm();
    if (item)
      this.FillEditCategoryForm(item);
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteNeedItemModal(content: any, item: any) {
    this.NeedId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  openDeleteCategoryItemModal(content: any, item: any) {
    this.CategoryId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllFamilyNeedTypesData() {
    this.showLoader = true;
    this.generalStatusService.GetAllFamilyNeedTypesData(this.NeedsPagingFilter).subscribe(data => {
      this.showLoader = false;
      this.FamilyNeedsData = data.results;
      this.NeedTotalCount = data.totalCount;
    });
  }

  GetAllFamilyNeedCategoriesSelector() {
    this.sharedService.GetAllFamilyNeedCategoriesSelector().subscribe(data => {
      this.CategoryList = data.results;
    });
  }

  GetAllFamilyNeedTypesFilters() {
    this.generalStatusService.GetAllFamilyNeedTypesFilters().subscribe(data => {
      debugger
      this.NeedFilterList = data.results;
    });
  }

  NeedPageChange(obj: any) {
    this.NeedsPagingFilter.currentPage = obj.page;
    this.GetAllFamilyNeedTypesData();
  }

  NeedFilterChecked(filterList: FilterModel[]) {
    debugger;
    this.NeedsPagingFilter.filterList = filterList;
    this.GetAllFamilyNeedTypesData();
  }

  GetAllFamilyNeedCategoriesData() {
    this.generalStatusService.GetAllFamilyNeedCategoriesData(this.CategoriesPagingFilter).subscribe(data => {
      this.FamilyCategoriesData = data.results;
      this.CategoryTotalCount = data.totalCount;
    });
  }

  GetAllFamilyNeedCategoriesFilters() {
    this.generalStatusService.GetAllFamilyNeedCategoriesFilters().subscribe(data => {
      debugger
      this.CategoryFilterList = data.results;
    });
  }

  CategoryPageChange(obj: any) {
    this.CategoriesPagingFilter.currentPage = obj.page;
    this.GetAllFamilyNeedCategoriesData();
  }

  CategoryFilterChecked(filterList: FilterModel[]) {
    debugger;
    this.CategoriesPagingFilter.filterList = filterList;
    this.GetAllFamilyNeedCategoriesData();
  }

  validateForm(itemForm: FormGroup, formsError: any): boolean {
    this.formService.markFormGroupTouched(itemForm);
    if (itemForm.valid) {
      return true;
    } else {
      formsError = this.formService.validateForm(itemForm, formsError, false)
      return false;
    }
  }

  AddNewNeed() {
    this.NeedForm = this.formService.TrimFormInputValue(this.NeedForm);
    let isValid = this.validateForm(this.NeedForm, this.needFormErrors);

    if (!isValid) {
      return;
    }

    this.showLoader = true;
    if (this.NeedForm.controls['id'].value == 0) {
      this.generalStatusService.AddNewFamilyNeedType(this.NeedForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyNeedTypesData();
          this.GetAllFamilyNeedTypesFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.generalStatusService.UpdateFamilyNeedType(this.NeedForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyNeedTypesData();
          this.GetAllFamilyNeedTypesFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    }
  }

  AddNewCategory() {
    this.CategoryForm = this.formService.TrimFormInputValue(this.CategoryForm);
    let isValid = this.validateForm(this.CategoryForm, this.categoryFormErrors);;

    if (!isValid) {
      return;
    }

    this.showLoader = true;
    if (this.CategoryForm.controls['id'].value == 0) {
      this.generalStatusService.AddNewFamilyNeedCategory(this.CategoryForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyNeedCategoriesData();
          this.GetAllFamilyNeedCategoriesFilters();
          this.GetAllFamilyNeedCategoriesSelector();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.generalStatusService.UpdateFamilyNeedCategory(this.CategoryForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyNeedCategoriesData();
          this.GetAllFamilyNeedCategoriesFilters();
          this.GetAllFamilyNeedCategoriesSelector();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    }
  }

  DeleteNeed() {
    this.showLoader = true;
    this.generalStatusService.DeleteFamilyNeedType(this.NeedId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllFamilyNeedTypesData();
        this.GetAllFamilyNeedTypesFilters();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  DeleteCategory() {
    this.showLoader = true;
    this.generalStatusService.DeleteFamilyNeedCategory(this.CategoryId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllFamilyNeedCategoriesData();
        this.GetAllFamilyNeedCategoriesFilters();
        this.GetAllFamilyNeedCategoriesSelector();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
