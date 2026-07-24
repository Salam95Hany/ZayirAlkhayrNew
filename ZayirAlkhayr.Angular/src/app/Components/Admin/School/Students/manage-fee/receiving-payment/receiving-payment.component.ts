import { Component } from '@angular/core';
import { FormDropdownModel } from '../../../../../../Models/shared/FormDropdownModel';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../../Auth/auth.service';
import { ZaBreadcrumbComponent } from "../../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaDropDownFormControlComponent } from "../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component";
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgFor, NgIf } from '@angular/common';
import { ArabicDateWithTimePipe } from '../../../../../../Pipes/arabic-date-with-time.pipe';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { NgxLoadingModule } from "ngx-loading";
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';

@Component({
  selector: 'app-receiving-payment',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaDropDownFormControlComponent, ZaEmptyDataComponent, NgIf, NgFor, FormsModule, ArabicDateWithTimePipe, NgxLoadingModule,
    ReactiveFormsModule, ZaInputWithLabelComponent
  ],
  templateUrl: './receiving-payment.component.html',
  styleUrl: './receiving-payment.component.css'
})
export class ReceivingPaymentComponent {
  TitleList = ['مركز بشائر القرآن', 'إدارة الرسوم', 'استلام دفعة'];
  Results: any;
  Fees: any;
  Payments: any[] = [];
  StudentFees: FormDropdownModel[] = [];
  Students: FormDropdownModel[] = [];
  FilterList: FilterModel[] = [];
  StudentId: any;
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  ItemForm: FormGroup;
  StudentFeeId: number;
  EnrollmentId: number;
  StudentPaymentId: number;
  UserId: any;
  formErrors = {
    paymentDate: '',
    amount: '',
    nextAmount: '',
    nextInstallmentDate: '',
    paymentMethod: '',
    note: ''
  };
  PaymentMethods = [
    { value: 1, name: 'كاش' },
    { value: 2, name: 'انستاباي' },
    { value: 3, name: 'فودافون كاش' }
  ]

  constructor(private modalService: NgbModal, private studentService: SchoolStudentService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService, private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetStudents();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      studentFeeId: [''],
      paymentDate: ['', [Validators.required]],
      amount: ['', [Validators.required]],
      nextAmount: [''],
      nextInstallmentDate: [''],
      paymentMethod: ['', [Validators.required]],
      note: ['', CustomValidators.regexPattern(RegexType.noSpace)],
      remainingAmount: [{ value: 0, disabled: true }],
      insertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });

    this.ItemForm.get('amount')?.valueChanges.subscribe((value) => {
      this.updateNextInstallmentValidators(value);
    });
  }

  updateNextInstallmentValidators(amount: number | null) {
    const remaining = this.Fees?.remainingAmount ?? 0;
    const enteredAmount = Number(amount) ?? 0;

    if (remaining === 0) {
      this.formService.updateFieldValidators(this.ItemForm, 'nextAmount', false, []);
      this.formService.updateFieldValidators(this.ItemForm, 'nextInstallmentDate', false, []);
      return;
    }

    if (remaining - enteredAmount === 0) {
      this.formService.updateFieldValidators(this.ItemForm, 'nextAmount', false, []);
      this.formService.updateFieldValidators(this.ItemForm, 'nextInstallmentDate', false, []);
      return;
    }

    this.formService.updateFieldValidators(this.ItemForm, 'nextAmount', true, [Validators.required]);
    this.formService.updateFieldValidators(this.ItemForm, 'nextInstallmentDate', true, [Validators.required,CustomValidators.greaterThanToday()]);
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('insertUser').setValue(this.UserId);
  }

  openItemModal(content: any) {
    if (!this.StudentId) {
      this.toaster.warning('برجاء اختيار طالب');
      return;
    }

    if (!this.StudentFeeId) {
      this.toaster.warning('برجاء اختيار رسوم');
      return;
    }

    this.ResetForm();
    this.ItemForm.patchValue({ remainingAmount: this.Fees?.remainingAmount ?? 0 });
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.StudentPaymentId = item.id;
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

  GetStudentFees() {
    this.studentService.GetStudentFees(this.EnrollmentId).subscribe(data => {
      this.StudentFees = data;
    });
  }

  GetAllStudentFeesByEnrollmentId() {
    this.showLoader = true;
    this.studentService.GetAllStudentFeesByEnrollmentId(this.EnrollmentId).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results;
      this.Fees = this.Results?.fees?.find(i => i.studentFeeId == this.StudentFeeId) ?? null;
      this.Payments = this.Fees?.payments ?? [];
      this.TotalCount = this.Fees?.payments?.length ?? 0;
    });
  }

  StudentChanged(studentId: any) {
    this.RestoreData();
    let obj = this.Students.find(i => i.value == studentId)?.extraData;
    if (obj) {
      this.EnrollmentId = obj['enrollmentId'];
      this.GetStudentFees();
    }
  }

  StudentFeeChanged(studentFeeId: any) {
    if (this.EnrollmentId) {
      this.GetAllStudentFeesByEnrollmentId();
    }
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

    if (+this.ItemForm.value.amount > this.Fees?.remainingAmount) {
      this.toaster.warning('المبلغ أكبر من المبلغ المتبقي');
      return;
    }

    let remainingAmount = this.Fees?.remainingAmount - +this.ItemForm.value.amount;
    if (+this.ItemForm.value.nextAmount > remainingAmount) {
      this.toaster.warning('القسط القادم أكبر من المبلغ المتبقي');
      return;
    }

    this.ItemForm.get('studentFeeId').setValue(this.StudentFeeId);
    this.showLoader = true;
    this.studentService.ReceivePayment(this.ItemForm.getRawValue()).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllStudentFeesByEnrollmentId();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });

  }

  DeleteItem() {
    this.showLoader = true;
    this.studentService.CancelPayment(this.StudentPaymentId, this.authService.userId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllStudentFeesByEnrollmentId();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  RestoreData() {
    this.Results = null;
    this.StudentFees = [];
    this.Fees = null;
    this.Payments = [];
    this.StudentFeeId = null;
    this.EnrollmentId = null;
    this.StudentPaymentId = null;
  }
}
