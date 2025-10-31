import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../Shared/za-pagination/za-pagination.component";
import { ZaEmptyDataComponent } from "../../../../../Shared/za-empty-data/za-empty-data.component";
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { ZaDropDownFormControlComponent } from "../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component";
import { BenefactorService } from '../../../../../Services/zainstitution/benefactor.service';
import { FormService } from '../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { FileService } from '../../../../../Services/shared/file.service';
import { SharedService } from '../../../../../Services/shared/shared.service';
import { AuthService } from '../../../../../Auth/auth.service';
import { RoleCheckerDirective } from '../../../../../Directives/role-checker.directive';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-benefactor-details',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent, RoleCheckerDirective,
    ZaEmptyDataComponent, NgbModule, ReactiveFormsModule, ZaDropDownFormControlComponent, NgxLoadingModule],
  templateUrl: './benefactor-details.component.html',
  styleUrl: './benefactor-details.component.css'
})
export class BenefactorDetailsComponent implements OnInit {
  @ViewChild('InputFile') InputFile: ElementRef;
  TitleList = ['مؤسسة زائر الخير', 'إدارة المتبرعين', 'تفاصيل المتبرعين'];
  showLoader = false;
  BenefactorType = 'All';
  BeneFactorData: any[] = [];
  fileURL: any[] = [];
  BeneFactorValuesData: any[] = [];
  BeneFactorDetailsData: any[] = [];
  BeneFactorTypesData: any[] = [];
  BeneFactorId: any;
  BeneFactorValueId: any;
  DefaultImage = 'logo-2.png';
  BeneFactorTypeId: any;
  UserId: any;
  Code: any;
  SearchText = '';
  BeneFactorTypeSearchText = '';
  TotalValue = 0;
  TotalCount = 0;
  BeneFactorTotalValue = 0;
  DetailsId: any;
  isFileExist = false;
  BeneFactorTypeValidation = false;
  TypeSwitcher = false;
  isFilter = true;
  ImageFile: any;
  ItemForm: FormGroup;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 10
  }
  pagedResponseModel: PagedResponseModel<any[]> = {
    totalCount: 0,
    results: [],
  };

  formErrors = {
    totalValue: '',
    paymentDate: '',
    details: '',
    beneFactorTypeId: ''
  };

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder, private sharedService: SharedService,
    private formService: FormService, private benefactorService: BenefactorService, private fileService: FileService, private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllBeneFactorsSelector();
    this.GetAllBeneFactorTypesSelector();
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
    this.isFileExist = false;
    this.fileURL = [];
    this.ImageFile = null;
    this.BeneFactorTypeValidation = false;
    this.InputFile.nativeElement.value = '';
    this.BeneFactorTypeId = this.BenefactorType == 'Cash' ? 1 : null;;
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('isFinalSubscribe').setValue(false);
    this.ItemForm.get('isParent').setValue(false);
    this.ItemForm.get('insertUser').setValue(this.UserId);
  }

  openItemModal(content: any, item: any) {
    if (!this.BeneFactorId) {
      this.toaster.warning('برجاء اختيار متبرع');
      return;
    }

    if (this.BenefactorType == 'Cash') {
      if (!this.BeneFactorValueId) {
        this.toaster.warning('برجاء اختيار قيمة التبرع');
        return;
      }
    }
    this.ResetForm();
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, detailsId: any) {
    debugger;
    this.DetailsId = detailsId;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  OnChangeBeneFactor(beneFactorId: any) {
    let obj = this.BeneFactorData.find(i => i.value == beneFactorId);
    if (obj)
      this.Code = obj.extraData['code'] ?? '';
    this.BeneFactorValueId = null;
    this.BeneFactorTypeId = null;
    this.BenefactorType = 'All';
    this.TypeSwitcher = false;
    this.TotalValue = 0;
    this.GetAllBeneFactorParentSelectorById();
    this.GetAllBeneFactorDetails();
  }

  OnChangeBeneFactorValue(beneFactorValueId: number) {
    this.TotalValue = 0;
    this.BeneFactorValueId = beneFactorValueId;
    let obj = this.BeneFactorValuesData.find(i => i.value == beneFactorValueId);
    if (obj) {
      this.TotalValue = obj.extraData['totalValue'] ?? 0;
      this.BeneFactorTotalValue = obj.extraData['totalValue'] ?? 0;
    }

    this.GetAllBeneFactorDetailsByValueId();
  }

  GetBenefactorType(isSelected: boolean) {
    this.BenefactorType = isSelected ? 'Cash' : 'All';
    if (this.BenefactorType == 'Cash') {
      this.formService.updateFieldsRequiredValidation(this.ItemForm, 'beneFactorTypeId', false);
      this.formService.updateFieldsRequiredValidation(this.ItemForm, 'totalValue', true);
      this.BeneFactorTypeId = 1;
    } else {
      this.formService.updateFieldsRequiredValidation(this.ItemForm, 'beneFactorTypeId', true);
      this.formService.updateFieldsRequiredValidation(this.ItemForm, 'totalValue', false);
      this.BeneFactorTypeId = null;
      this.BeneFactorValueId = null;
      this.TotalValue = 0;
      this.PagingFilter.currentPage = 1;
      this.GetAllBeneFactorDetails();
    }
  }

  onFileChange(event: any) {
    let fileSize = this.fileService.getFileSize(event.target.files[0]);
    if (fileSize > 1) {
      this.toaster.warning(`هذا الملف ${event.target.files[0].name} حجمه أكبر من 1 ميجا`);
      return;
    }

    this.fileURL = [];
    this.ImageFile = null;
    this.fileService.onSelectedFile(event.target.files).then(data => {
      this.fileURL.push(data[0]);
      this.ImageFile = data[1][0];
      this.isFileExist = false;
    });
  }

  DeleteSelectedFile() {
    this.ImageFile = null;
    this.fileURL = [];
    this.InputFile.nativeElement.value = '';
  }

  GetAllBeneFactorsSelector() {
    this.sharedService.GetAllBeneFactorsSelector().subscribe(data => {
      this.BeneFactorData = data.results;
    });
  }

  GetAllBeneFactorParentSelectorById() {
    this.sharedService.GetAllBeneFactorParentSelectorById(this.BeneFactorId).subscribe(data => {
      this.BeneFactorValuesData = data.results;
    });
  }

  GetAllBeneFactorDetails() {
    this.benefactorService.GetAllBeneFactorDetails(this.PagingFilter, this.BeneFactorId).subscribe(data => {
      this.BeneFactorDetailsData = data.results;
      this.TotalCount = data.totalCount
    });
  }

  pageChanged(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllBeneFactorDetails();
  }

  GetAllBeneFactorDetailsByValueId() {
    this.benefactorService.GetAllBeneFactorCashDetails(this.BeneFactorId, this.BeneFactorValueId).subscribe(data => {
      this.BeneFactorDetailsData = data.results;
      let detailsTotalValue = 0;
      this.BeneFactorDetailsData.filter(i => i.beneFactorTypeId == 1).forEach(i => {
        detailsTotalValue += Number(i.totalValue);
      })
      this.TotalValue = this.BeneFactorTotalValue - detailsTotalValue;
    });
  }

  GetAllBeneFactorTypesSelector() {
    this.sharedService.GetAllBeneFactorTypesSelector().subscribe(data => {
      this.BeneFactorTypesData = data.results;
    });
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
    let num = Number(this.ItemForm.controls['totalValue'].value);
    let isFinalSubscribe = this.TotalValue - num;
    if (this.BenefactorType == 'Cash') {
      this.ItemForm.get('beneFactorTypeId').setValue(this.BeneFactorTypeId);
      this.ItemForm.get('parentId').setValue(this.BeneFactorValueId);
      if (this.TotalValue == 0) {
        this.toaster.warning('لا يمكن اضافة تبرع جديد لقد نفذ مبلغ التبرع');
        return;
      }

      if (num > this.TotalValue) {
        this.toaster.warning('لا يمكن اضافة قيمة اكبر من باق مبلغ التبرع');
        return;
      }
    }
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();
    if (!isValid)
      return;


    if (isFinalSubscribe == 0)
      this.ItemForm.get('isFinalSubscribe').setValue(true);

    this.ItemForm.get('beneFactorId').setValue(this.BeneFactorId);
    this.ItemForm.get('file').setValue(this.ImageFile);

    const formData = new FormData();
    this.formService.buildFormData(formData, this.ItemForm.value);
    this.showLoader = true;
    this.benefactorService.AddNewBeneFactorDetails(formData).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        if (this.BenefactorType == 'Cash')
          this.GetAllBeneFactorDetailsByValueId();
        else
          this.GetAllBeneFactorDetails();

        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  NumbersOnly(key: any) {
    return this.formService.NumbersOnly(key);
  }

  DeleteItem() {
    debugger;
    this.showLoader = true;
    this.benefactorService.DeleteBeneFactorDetails(this.DetailsId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        if (this.BenefactorType == 'Cash')
          this.GetAllBeneFactorDetailsByValueId();
        else
          this.GetAllBeneFactorDetails();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    })
  }
}
