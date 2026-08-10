import { Component } from '@angular/core';
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { NgxLoadingModule } from 'ngx-loading';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../../Auth/auth.service';
import { ArabicDateWithTimePipe } from '../../../../../../Pipes/arabic-date-with-time.pipe';
import { StudentSettingService } from '../../../../../../Services/school/student-setting.service';

@Component({
  selector: 'app-academic-year',
  standalone: true,
  imports: [AdminBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent, NgbModule,FormsModule,
    NgIf, NgFor, ZaInputWithLabelComponent, ReactiveFormsModule, RoleCheckerDirective, NgxLoadingModule, ArabicDateWithTimePipe],
  providers: [DatePipe],
  templateUrl: './academic-year.component.html',
  styleUrl: './academic-year.component.css'
})
export class AcademicYearComponent {
  TitleList = ['مركز بشائر القرآن', 'الإعدادات', 'السنوات الدراسية'];
  Results: any[] = [];
  FilterList: FilterModel[] = [];
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  ItemForm: FormGroup;
  UserId: any;
  AcademicYearId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    name1: '',
    name2: '',
    startDate: '',
    endDate: '',
    promotionOpenDate: '',
    promotionCloseDate: ''
  };

  constructor(private modalService: NgbModal, private studentSettingService: StudentSettingService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService, private authService: AuthService, private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllAcademicYearData();
    this.GetAllAcademicYearFilter();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      name1: ['', [Validators.required]],
      name2: ['', [Validators.required]],
      name: [''],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]],
      promotionOpenDate: ['', [Validators.required]],
      promotionCloseDate: ['', [Validators.required]],
      isCurrent: false,
      insertUser: null
    }, {
      validators: this.academicYearValidator()
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  academicYearValidator(): ValidatorFn {
    return (group: AbstractControl): ValidationErrors | null => {

      const fromControl = group.get('name1');
      const toControl = group.get('name2');

      if (!fromControl || !toControl) {
        return null;
      }

      const from = Number(fromControl.value);
      const to = Number(toControl.value);

      if (!from || !to) {
        return null;
      }

      if (from >= to) {
        toControl.setErrors({ ...(toControl.errors || {}), invalidAcademicYear: true });
      } else if (toControl.errors?.['invalidAcademicYear']) {
        const errors = { ...toControl.errors };
        delete errors['invalidAcademicYear'];
        toControl.setErrors(Object.keys(errors).length ? errors : null);
      }

      return null;
    };
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      name1: item?.name?.split('-')[1],
      name2: item?.name?.split('-')[0],
      name: item?.name,
      startDate: this.datePipe.transform(item?.startDate, 'yyyy-MM-dd'),
      endDate: this.datePipe.transform(item?.endDate, 'yyyy-MM-dd'),
      promotionOpenDate: this.datePipe.transform(item?.promotionOpenDate, 'yyyy-MM-dd'),
      promotionCloseDate: this.datePipe.transform(item?.promotionCloseDate, 'yyyy-MM-dd'),
      isCurrent: item?.isCurrent,
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
    this.AcademicYearId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllAcademicYearData() {
    this.showLoader = true;
    this.studentSettingService.GetAllAcademicYearData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllAcademicYearFilter() {
    this.studentSettingService.GetAllAcademicYearFilter().subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllAcademicYearData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllAcademicYearData();
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

    if (this.Results.length == 0)
      this.ItemForm.get('isCurrent').setValue(true);
    this.showLoader = true;
    let name = this.ItemForm.value.name2 + '-' + this.ItemForm.value.name1;
    this.ItemForm.get('name').setValue(name);
    console.log(this.ItemForm.value);

    if (this.ItemForm.controls['id'].value == 0) {
      this.studentSettingService.AddNewAcademicYear(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllAcademicYearData();
          this.GetAllAcademicYearFilter();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.studentSettingService.UpdateAcademicYear(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllAcademicYearData();
          this.GetAllAcademicYearFilter();
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
    this.studentSettingService.DeleteAcademicYear(this.AcademicYearId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllAcademicYearData();
        this.GetAllAcademicYearFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
