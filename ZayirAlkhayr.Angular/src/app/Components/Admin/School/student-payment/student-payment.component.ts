import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZaBreadcrumbComponent } from '../../../../Shared/za-breadcrumb/za-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../Shared/za-pagination/za-pagination.component';
import { RoleCheckerDirective } from '../../../../Directives/role-checker.directive';
import { ZaEmptyDataComponent } from '../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ZaDropDownFormControlComponent } from '../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { NgxLoadingModule } from 'ngx-loading';
import { PagingFilterModel } from '../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { SharedService } from '../../../../Services/shared/shared.service';
import { FormService } from '../../../../Services/shared/form.service';
import { AuthService } from '../../../../Auth/auth.service';
import { SchoolStudentService } from '../../../../Services/school/school-student.service';
import { CustomValidators, RegexType } from '../../../../Services/shared/custom-validators';

@Component({
  selector: 'app-student-payment',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent, RoleCheckerDirective,
    ZaEmptyDataComponent, NgbModule, ReactiveFormsModule, ZaDropDownFormControlComponent, NgxLoadingModule],
  templateUrl: './student-payment.component.html',
  styleUrl: './student-payment.component.css'
})
export class StudentPaymentComponent {
  TitleList = ['مركز بشائر القرآن', 'إدارة الطلاب', 'الدفعات'];
  showLoader = false;
  StudentData: any[] = [];
  StudentPaymentData: any[] = [];
  UserId: any;
  TotalValue = 0;
  TotalCount = 0;
  DetailsId: any;
  StudentId: any;
  isFilter = true;
  ItemForm: FormGroup;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 10
  }

  formErrors = {
    totalValue: '',
    paymentDate: '',
    details: '',
    beneFactorTypeId: ''
  };

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder, private sharedService: SharedService,
    private formService: FormService, private schoolService: SchoolStudentService, private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      beneFactorId: null,
      beneFactorTypeId: ['', Validators.required],
      parentId: null,
      details: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      totalValue: ['', Validators.required],
      paymentDate: ['', Validators.required],
      insertUser: null,
      isFinalSubscribe: false,
      isParent: false,
      oldFileName: null,
      file: null,
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });

    this.formService.updateFieldsRequiredValidation(this.ItemForm, 'totalValue', false);
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('isFinalSubscribe').setValue(false);
    this.ItemForm.get('isParent').setValue(false);
    this.ItemForm.get('insertUser').setValue(this.UserId);
  }

  openItemModal(content: any, item: any) {
    if (!this.StudentId) {
      this.toaster.warning('برجاء اختيار متبرع');
      return;
    }

    this.ResetForm();
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, detailsId: any) {
    this.DetailsId = detailsId;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  OnChangeBeneFactor(beneFactorId: any) {
    let obj = this.StudentData.find(i => i.value == beneFactorId);
    this.TotalValue = 0;
  }


  pageChanged(obj: any) {
    this.PagingFilter.currentPage = obj.page;
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
    // let num = Number(this.ItemForm.controls['totalValue'].value);
    // let isFinalSubscribe = this.TotalValue - num;
    // if (this.BenefactorType == 'Cash') {
    //   this.ItemForm.get('beneFactorTypeId').setValue(this.BeneFactorTypeId);
    //   this.ItemForm.get('parentId').setValue(this.BeneFactorValueId);
    //   if (this.TotalValue == 0) {
    //     this.toaster.warning('لا يمكن اضافة تبرع جديد لقد نفذ مبلغ التبرع');
    //     return;
    //   }

    //   if (num > this.TotalValue) {
    //     this.toaster.warning('لا يمكن اضافة قيمة اكبر من باق مبلغ التبرع');
    //     return;
    //   }
    // }
    // this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    // let isValid = this.validateForm();
    // if (!isValid)
    //   return;


    // if (isFinalSubscribe == 0)
    //   this.ItemForm.get('isFinalSubscribe').setValue(true);

    // this.ItemForm.get('beneFactorId').setValue(this.BeneFactorId);

    // const formData = new FormData();
    // this.formService.buildFormData(formData, this.ItemForm.value);
    // this.showLoader = true;
    // this.benefactorService.AddNewBeneFactorDetails(formData).subscribe(data => {
    //   if (data.isSuccess) {
    //     this.toaster.success(data.message);
    //     this.modalService.dismissAll();
    //   }
    //   else
    //     this.toaster.error(data.message);
    //   this.showLoader = false;
    // });
  }

  NumbersOnly(key: any) {
    return this.formService.NumbersOnly(key);
  }

  DeleteItem() {
    // this.showLoader = true;
    // this.benefactorService.DeleteBeneFactorDetails(this.DetailsId).subscribe(data => {
    //   if (data.isSuccess) {
    //     this.toaster.success(data.message);
    //     this.modalService.dismissAll();
    //   }
    //   else
    //     this.toaster.error(data.message);
    //   this.showLoader = false;
    // })
  }
}
