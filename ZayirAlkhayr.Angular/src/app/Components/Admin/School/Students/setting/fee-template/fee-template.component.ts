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
import { FormDropdownModel } from '../../../../../../Models/shared/FormDropdownModel';

@Component({
  selector: 'app-fee-template',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent, NgbModule,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, RoleCheckerDirective, NgxLoadingModule],
  templateUrl: './fee-template.component.html',
  styleUrl: './fee-template.component.css'
})
export class FeeTemplateComponent {
  TitleList = ['مركز بشائر القرآن', 'الإعدادات', 'قوالب الرسوم'];
  Results: any[] = [];
  FeeTypes: FormDropdownModel[] = [];
  AcademicStages: FormDropdownModel[] = [];
  FilterList: FilterModel[] = [];
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  ItemForm: FormGroup;
  UserId: any;
  FeeTemplateId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    academicStageId: '',
    academicYearId: '',
    feeTypeId: '',
    amount: ''
  };

  constructor(private modalService: NgbModal, private schoolService: SchoolStudentService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService, private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllFeeTemplateData();
    this.GetAllFeeTemplateFilter();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      academicStageId: ['', [Validators.required]],
      academicYearId: ['', [Validators.required]],
      feeTypeId: ['', [Validators.required]],
      amount: ['', [Validators.required]],
      insertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      academicStageId: item?.academicStageId,
      academicYearId: item?.academicYearId,
      feeTypeId: item?.feeTypeId,
      amount: item?.amount,
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
    this.FeeTemplateId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllFeeTemplateData() {
    this.showLoader = true;
    this.schoolService.GetAllFeeTemplateData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllFeeTemplateFilter() {
    this.schoolService.GetAllFeeTemplateFilter().subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllFeeTemplateData();
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
      this.schoolService.AddNewFeeTemplate(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFeeTemplateData();
          this.GetAllFeeTemplateFilter();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.schoolService.UpdateFeeTemplate(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFeeTemplateData();
          this.GetAllFeeTemplateFilter();
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
    this.schoolService.DeleteFeeTemplate(this.FeeTemplateId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllFeeTemplateData();
        this.GetAllFeeTemplateFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
