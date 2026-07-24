import { Component } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../../Shared/za-filters/za-filters.component";
import { NgxLoadingModule } from "ngx-loading";
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { ZaDropDownFormControlComponent } from '../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { FormDropdownModel } from '../../../../../../Models/shared/FormDropdownModel';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../../Auth/auth.service';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { ArabicDateWithTimePipe } from '../../../../../../Pipes/arabic-date-with-time.pipe';

@Component({
  selector: 'app-student-fees',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ArabicDateWithTimePipe,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, RoleCheckerDirective, NgxLoadingModule, ZaDropDownFormControlComponent],
  templateUrl: './student-fees.component.html',
  styleUrl: './student-fees.component.css'
})
export class StudentFeesComponent {
  TitleList = ['مركز بشائر القرآن', 'إدارة الرسوم', 'رسوم الطلاب'];
  Results: any[] = [];
  FeeTemplates: FormDropdownModel[] = [];
  Students: FormDropdownModel[] = [];
  DiscountTypes: FormDropdownModel[] = [];
  FilterList: FilterModel[] = [];
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  ItemForm: FormGroup;
  StudentFeeId: number;
  EnrollmentId: number;
  UserId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    studentId: '',
    feeTemplateId: '',
    discountAmountPer: '',
    discountReason: ''
  };

  constructor(private modalService: NgbModal, private studentService: SchoolStudentService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService, private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllStudentFeeData();
    this.GetAllStudentFeeFilters();
    this.GetStudents();
    this.GetDiscountTypes();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      studentId: ['', [Validators.required]],
      studentEnrollmentId: [''],
      feeTemplateId: ['', [Validators.required]],
      feeTypeId: [''],
      discountTypeId: [''],
      discountAmountPer: [''],
      discountReason: [''],
      totalAmount: [{ value: '', disabled: true }],
      insertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });

    this.ItemForm.get('discountTypeId')?.valueChanges.subscribe(value => {
      if (value) {
        this.formService.updateFieldValidators(this.ItemForm, 'discountReason', true, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]);
        this.formService.updateFieldValidators(this.ItemForm, 'discountAmountPer', true, [Validators.required]);
      }
      else {
        this.formService.updateFieldValidators(this.ItemForm, 'discountReason', false, []);
        this.formService.updateFieldValidators(this.ItemForm, 'discountAmountPer', false, []);
      }
    });

    this.ItemForm.get('studentId')?.valueChanges.subscribe(value => {
      if (value) {
        let obj = this.Students.find(i => i.value == value)?.extraData;
        if (obj) {
          this.EnrollmentId = obj['enrollmentId'];
          this.ItemForm.get('studentEnrollmentId').setValue(this.EnrollmentId);
          this.GetFeeTemplates();
        }
      }
    });

    this.ItemForm.get('feeTemplateId')?.valueChanges.subscribe(value => {
      if (value) {
        let obj = this.FeeTemplates.find(i => i.value == value)?.extraData;
        if (obj) {
          this.ItemForm.patchValue({ feeTypeId: obj['feeTypeId'], totalAmount: obj['totalAmount'] });
        }
      }
    });
  }

  FillEditForm(item: any) {
    debugger;
    this.ItemForm.setValue({
      id: item.id,
      studentId: item?.studentId,
      studentEnrollmentId: item?.enrollmentId,
      feeTemplateId: item?.feeTemplateId,
      feeTypeId: item?.feeTypeId,
      discountTypeId: item?.discountTypeId ?? '',
      discountAmountPer: item?.discountAmountPer ?? '',
      discountReason: item?.discountReason ?? '',
      totalAmount: item?.totalAmount,
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
      this.ItemForm.get('studentId')?.enable();
    if (item) {
       this.ItemForm.get('studentId')?.disable();
      this.FillEditForm(item);
    }

    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.StudentFeeId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetStudents() {
    this.studentService.GetStudents().subscribe(data => {
      this.Students = data;
    });
  }

  GetFeeTemplates() {
    this.studentService.GetFeeTemplates(this.EnrollmentId).subscribe(data => {
      this.FeeTemplates = data;
    });
  }

  GetDiscountTypes() {
    this.studentService.GetDiscountTypes().subscribe(data => {
      this.DiscountTypes = data;
    });
  }

  GetAllStudentFeeData() {
    this.showLoader = true;
    this.studentService.GetAllStudentFeeData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results.table;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllStudentFeeFilters() {
    this.studentService.GetAllStudentFeeFilters(this.PagingFilter).subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllStudentFeeData();
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
      this.studentService.AddNewStudentFee(this.ItemForm.getRawValue()).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllStudentFeeData();
          this.GetAllStudentFeeFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.studentService.UpdateStudentFee(this.ItemForm.getRawValue()).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllStudentFeeData();
          this.GetAllStudentFeeFilters();
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
    this.studentService.CancelStudentFee(this.StudentFeeId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllStudentFeeData();
        this.GetAllStudentFeeFilters();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
