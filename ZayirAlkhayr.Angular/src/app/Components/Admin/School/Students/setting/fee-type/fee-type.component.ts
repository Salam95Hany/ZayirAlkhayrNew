import { Component } from '@angular/core';
import { ZaBreadcrumbComponent } from '../../../../../../Shared/za-breadcrumb/za-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { NgxLoadingModule } from 'ngx-loading';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../../Auth/auth.service';

@Component({
  selector: 'app-fee-type',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent, NgbModule,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, RoleCheckerDirective, NgxLoadingModule],
  templateUrl: './fee-type.component.html',
  styleUrl: './fee-type.component.css'
})
export class FeeTypeComponent {
  TitleList = ['مركز بشائر القرآن', 'الإعدادات', 'أنواع الرسوم'];
  Results: any[] = [];
  FilterList: FilterModel[] = [];
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  ItemForm: FormGroup;
  UserId: any;
  FeeTypeId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    name: ''
  };

  constructor(private modalService: NgbModal, private schoolService: SchoolStudentService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService, private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllFeeTypeData();
    this.GetAllFeeTypeFilter();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      name: ['', [Validators.required]],
      insertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      name: item?.name,
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
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.FeeTypeId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllFeeTypeData() {
    this.showLoader = true;
    this.schoolService.GetAllFeeTypeData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllFeeTypeFilter() {
    this.schoolService.GetAllFeeTypeFilter().subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllFeeTypeData();
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
      this.schoolService.AddNewFeeType(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFeeTypeData();
          this.GetAllFeeTypeFilter();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.schoolService.UpdateFeeType(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFeeTypeData();
          this.GetAllFeeTypeFilter();
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
    this.schoolService.DeleteFeeType(this.FeeTypeId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllFeeTypeData();
        this.GetAllFeeTypeFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
