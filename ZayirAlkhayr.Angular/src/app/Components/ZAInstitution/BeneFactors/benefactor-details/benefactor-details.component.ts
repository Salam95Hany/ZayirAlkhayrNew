import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../Shared/za-pagination/za-pagination.component";
import { ZaEmptyDataComponent } from "../../../../Shared/za-empty-data/za-empty-data.component";
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PagingFilterModel } from '../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ValidationFormService } from '../../../../Services/shared/validation-form.service';
import { CommonModule } from '@angular/common';
import { ZaDropDownFormControlComponent } from "../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component";
import { BenefactorService } from '../../../../Services/zainstitution/benefactor.service';

@Component({
  selector: 'app-benefactor-details',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent,
    ZaEmptyDataComponent, NgbModule, ReactiveFormsModule, ZaDropDownFormControlComponent],
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
  DefaultImage = '../../../../assets/logo-2.png';
  BeneFactorTypeId: any;
  UserModel: any;
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
  isFilter = false;
  ImageFile: any;
  ItemForm: FormGroup;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 10
  }
  BeneFactorPagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 200
  }
  pagedResponseModel: PagedResponseModel<any[]> = {
    totalCount: 0,
    results: [],
  };

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder,
    private formService: ValidationFormService, private benefactorService: BenefactorService
  ) { }

  ngOnInit(): void {
    this.UserModel = JSON.parse(localStorage.getItem('UserModel'));
    this.FormInit();
    this.GetAllBeneFactorData();
    this.GetAllBeneFactorTypes();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      beneFactorId: null,
      beneFactorTypeId: null,
      parentId: null,
      details: ['', this.formService.noSpaceValidator],
      totalValue: ['', Validators.required],
      paymentDate: ['', Validators.required],
      insertUser: null,
      isFinalSubscribe: false,
      isParent: false,
      oldFileName: null,
      file: null,
    });
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
    this.ItemForm.get('totalValue').setValue(0);
    this.ItemForm.get('isFinalSubscribe').setValue(false);
    this.ItemForm.get('isParent').setValue(false);
    this.ItemForm.get('insertUser').setValue(this.UserModel?.userId);
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
    this.DetailsId = detailsId;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetBenefactorType(isSelected: boolean) {
    this.BenefactorType = isSelected ? 'Cash' : 'All';
    if (this.BenefactorType == 'Cash') {
      this.BeneFactorTypeId = 1;
    } else {
      this.BeneFactorTypeId = null;
      this.BeneFactorValueId = null;
      this.TotalValue = 0;
      this.PagingFilter.currentPage = 1;
      this.GetAllBeneFactorDetails();
    }
  }

  onFileChange(event: any) {
    let fileSize = this.formService.getFileSize(event.target.files[0]);
    if (fileSize > 1) {
      this.toaster.warning(`هذا الملف ${event.target.files[0].name} حجمه أكبر من 1 ميجا`);
      return;
    }

    this.fileURL = [];
    this.ImageFile = null;
    this.formService.onSelectedFile(event.target.files).then(data => {
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

  GetAllBeneFactorData() {
    this.benefactorService.GetAllBeneFactorData(this.BeneFactorPagingFilter).subscribe(data => {
      this.BeneFactorData = data.table;
    });
  }

  GetAllBeneFactorParentById() {
    this.benefactorService.GetAllBeneFactorParentById(this.BeneFactorId).subscribe(data => {
      this.BeneFactorValuesData = data.filter(i => !i.isActive);
    });
  }

  GetAllBeneFactorDetails() {
    this.benefactorService.GetAllBeneFactorDetails(this.PagingFilter, this.BeneFactorId).subscribe(data => {
      this.BeneFactorDetailsData = data;
      this.TotalCount = data && data.length > 0 ? data[0].totalCount : 0;
    });
  }

  pageChanged(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllBeneFactorDetails();
  }

  GetAllBeneFactorDetailsByValueId() {
    this.benefactorService.GetAllBeneFactorCashDetails(this.BeneFactorId, this.BeneFactorValueId).subscribe(data => {
      this.BeneFactorDetailsData = data;
      let detailsTotalValue = 0;
      this.BeneFactorDetailsData.filter(i => i.beneFactorTypeId == 1).forEach(i => {
        detailsTotalValue += Number(i.totalValue);
      })
      this.TotalValue = this.BeneFactorTotalValue - detailsTotalValue;
    });
  }

  GetAllBeneFactorTypes() {
    this.benefactorService.GetAllBeneFactorTypes(this.BeneFactorPagingFilter).subscribe(data => {
      this.BeneFactorTypesData = data.results.filter(i => i.id != 1);
    });
  }

  AddNewItem() {
    let num = Number(this.ItemForm.controls['totalValue'].value);
    let isFinalSubscribe = this.TotalValue - num;
    if (this.BenefactorType == 'Cash') {
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
    let isValid = this.ItemForm.valid;
    if (!isValid) {
      this.formService.validateAllFormFields(this.ItemForm);
      return;
    }

    if (isFinalSubscribe == 0)
      this.ItemForm.get('isFinalSubscribe').setValue(true);

    this.ItemForm.get('beneFactorId').setValue(this.BeneFactorId);

    this.ItemForm.get('beneFactorTypeId').setValue(this.BeneFactorTypeId);
    this.ItemForm.get('file').setValue(this.ImageFile);
    const formData = new FormData();
    this.formService.buildFormData(formData, this.ItemForm.value);
    this.showLoader = true;
    this.benefactorService.AddNewBeneFactorDetails(formData).subscribe(data => {
      if (data.done) {
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
    this.showLoader = true;
    this.benefactorService.DeleteBeneFactorDetails(this.DetailsId).subscribe(data => {
      if (data.done) {
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
