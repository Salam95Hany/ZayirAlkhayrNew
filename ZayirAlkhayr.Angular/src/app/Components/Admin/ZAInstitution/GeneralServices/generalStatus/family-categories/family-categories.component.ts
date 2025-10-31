import { Component, OnInit } from '@angular/core';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStatusService } from '../../../../../../Services/zainstitution/general-status.service';
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { ZaBreadcrumbComponent } from "../../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../../Shared/za-filters/za-filters.component";
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgFor, NgIf } from '@angular/common';
import { AuthService } from '../../../../../../Auth/auth.service';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-family-categories',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaInputWithLabelComponent, ZaEmptyDataComponent, ReactiveFormsModule,
    NgFor, NgIf, NgbModule, RoleCheckerDirective, NgxLoadingModule],
  templateUrl: './family-categories.component.html',
  styleUrl: './family-categories.component.css'
})
export class FamilyCategoriesComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'خدمات اجتماعية', 'الفئات'];
  FamilyNationalityData: any[] = [];
  FilterList: FilterModel[] = [];
  showLoader = false;
  isFilter = false;
  TotalCount = 0;
  ItemForm: FormGroup;
  UserId: any;
  NationalityId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    name: ''
  };

  constructor(private modalService: NgbModal, private generalStatusService: GeneralStatusService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService,private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllFamilyCategoryData();
    this.GetAllFamilyCategoryFilter();
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
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.NationalityId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllFamilyCategoryData() {
    this.showLoader = true;
    this.generalStatusService.GetAllFamilyCategoryData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.FamilyNationalityData = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllFamilyCategoryFilter() {
    this.generalStatusService.GetAllFamilyCategoryFilter().subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllFamilyCategoryData();
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

    if (!isValid) {
      return;
    }


    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.generalStatusService.AddNewFamilyCategory(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyCategoryData();
          this.GetAllFamilyCategoryFilter();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.generalStatusService.UpdateFamilyCategory(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyCategoryData();
          this.GetAllFamilyCategoryFilter();
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
    this.generalStatusService.DeleteFamilyCategory(this.NationalityId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllFamilyCategoryData();
        this.GetAllFamilyCategoryFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
