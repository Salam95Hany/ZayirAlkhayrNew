import { Component } from '@angular/core';
import { NgxLoadingModule } from "ngx-loading";
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgFor, NgIf } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ZaBreadcrumbComponent } from '../../../../../../Shared/za-breadcrumb/za-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { FormService } from '../../../../../../Services/shared/form.service';
import { AuthService } from '../../../../../../Auth/auth.service';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { StudentSettingService } from '../../../../../../Services/school/student-setting.service';

@Component({
  selector: 'app-discount-type',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent, NgbModule,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, RoleCheckerDirective, NgxLoadingModule],
  templateUrl: './discount-type.component.html',
  styleUrl: './discount-type.component.css'
})
export class DiscountTypeComponent {
  TitleList = ['مركز بشائر القرآن', 'إدارة الطلاب', 'أنواع الخصم'];
  Results: any[] = [];
  FilterList: FilterModel[] = [];
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  ItemForm: FormGroup;
  UserId: any;
  DiscountTypeId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    name: ''
  };

  constructor(private modalService: NgbModal, private studentSettingService: StudentSettingService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService, private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllDiscountTypeData();
    this.GetAllDiscountTypeFilter();
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
    this.DiscountTypeId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllDiscountTypeData() {
    this.showLoader = true;
    this.studentSettingService.GetAllDiscountTypeData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllDiscountTypeFilter() {
    this.studentSettingService.GetAllDiscountTypeFilter().subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllDiscountTypeData();
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
      this.studentSettingService.AddNewDiscountType(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllDiscountTypeData();
          this.GetAllDiscountTypeFilter();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.studentSettingService.UpdateDiscountType(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllDiscountTypeData();
          this.GetAllDiscountTypeFilter();
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
    this.studentSettingService.DeleteDiscountType(this.DiscountTypeId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllDiscountTypeData();
        this.GetAllDiscountTypeFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
