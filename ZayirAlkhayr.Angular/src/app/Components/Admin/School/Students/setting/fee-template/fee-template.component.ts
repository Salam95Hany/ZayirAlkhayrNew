import { Component } from '@angular/core';
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';
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
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../../Auth/auth.service';
import { FormDropdownModel } from '../../../../../../Models/shared/FormDropdownModel';
import { StudentSettingService } from '../../../../../../Services/school/student-setting.service';
import { ZaDropDownFormControlComponent } from '../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';

@Component({
  selector: 'app-fee-template',
  standalone: true,
  imports: [AdminBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent, NgbModule,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, RoleCheckerDirective, NgxLoadingModule, ZaDropDownFormControlComponent],
  templateUrl: './fee-template.component.html',
  styleUrl: './fee-template.component.css'
})
export class FeeTemplateComponent {
  TitleList = ['مركز بشائر القرآن', 'الإعدادات', 'قوالب الرسوم'];
  Results: any[] = [];
  FeeTypes: FormDropdownModel[] = [];
  AcademicStages: FormDropdownModel[] = [];
  FilterList: FilterModel[] = [];
  AcademicYear: any;
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
    academicYearName: '',
    feeTypeId: '',
    amount: ''
  };

  constructor(private modalService: NgbModal, private studentSettingService: StudentSettingService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService, private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllFeeTemplateData();
    this.GetAllFeeTemplateFilter();
    this.GetCurrentAcademicYear();
    this.GetAcademicStages();
    this.GetFeeTypes();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      academicStageId: ['', [Validators.required]],
      academicYearId: [''],
      academicYearName: [{ value: '', disabled: true }, [Validators.required]],
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
      academicYearName: item?.academicYearId,
      feeTypeId: item?.feeTypeId,
      amount: item?.amount,
      insertUser: this.UserId,
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('insertUser').setValue(this.UserId);
    this.ItemForm.patchValue({ academicYearId: this.AcademicYear.split(';;;')[1], academicYearName: this.AcademicYear.split(';;;')[0] });
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

  GetCurrentAcademicYear() {
    this.showLoader = true;
    this.studentSettingService.GetCurrentAcademicYear().subscribe(data => {
      this.AcademicYear = data.results;
    });
  }

  GetAcademicStages() {
    this.showLoader = true;
    this.studentSettingService.GetAcademicStages().subscribe(data => {
      this.AcademicStages = data;
    });
  }

  GetFeeTypes() {
    this.showLoader = true;
    this.studentSettingService.GetFeeTypes().subscribe(data => {
      this.FeeTypes = data;
    });
  }

  GetAllFeeTemplateData() {
    this.showLoader = true;
    this.studentSettingService.GetAllFeeTemplateData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllFeeTemplateFilter() {
    this.studentSettingService.GetAllFeeTemplateFilter().subscribe(data => {
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
      this.studentSettingService.AddNewFeeTemplate(this.ItemForm.value).subscribe(data => {
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
      this.studentSettingService.UpdateFeeTemplate(this.ItemForm.value).subscribe(data => {
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
    this.studentSettingService.DeleteFeeTemplate(this.FeeTemplateId).subscribe(data => {
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
