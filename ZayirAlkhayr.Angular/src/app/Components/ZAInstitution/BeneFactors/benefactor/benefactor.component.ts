import { Component, ElementRef, TemplateRef, ViewChild } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../Shared/za-filters/za-filters.component";
import { CommonModule, DatePipe } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZaEmptyDataComponent } from '../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { FilterModel } from '../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { ValidationFormService } from '../../../../Services/shared/validation-form.service';
import { BenefactorService } from '../../../../Services/zainstitution/benefactor.service';
import { PdfDownloadService } from '../../../../Services/shared/pdf-download.service';
import { BeneFactorDetails } from '../../../../Models/zainstitution/BeneFactorModel';
import { PDFHeaderSelectedModel, PDFModel } from '../../../../Models/shared/PDFHeaderSelected';

@Component({
  selector: 'app-benefactor',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule],
  providers: [DatePipe],
  templateUrl: './benefactor.component.html',
  styleUrl: './benefactor.component.css'
})
export class BenefactorComponent {
  @ViewChild('InputFile') InputFile: ElementRef;
  @ViewChild('DetailsSidePanel', { static: true }) DetailsSidePanel: TemplateRef<any>;
  TitleList = ['مؤسسة زائر الخير', 'إدارة المتبرعين', 'المتبرعين'];
  filterList: FilterModel[] = [];
  PDFHeaderModel: PDFHeaderSelectedModel[] = [];
  BeneFactorValues: BeneFactorDetails = {} as BeneFactorDetails;
  fileURL: any[] = [];
  BeneFactorValuesData: any[] = [];
  BeneFactorHeaders: any[] = [];
  NationalityList: any[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  isFilter = false;
  isFileExist = false;
  ImageFile: any;
  UserModel: any;
  BeneFactorId: any;
  RowCount = 20;
  PDFModel: PDFModel = {
    filterList: [],
    headers: []
  };
  pagingFilterModel: PagingFilterModel = {
    currentPage: 1,
    pageSize: 20,
    filterList: []
  };
  pagedResponseModel: PagedResponseModel<any[]> = {
    totalCount: 0,
    results: [],
  };

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder,
    private formService: ValidationFormService, private benefactorService: BenefactorService, private pdfService: PdfDownloadService,
    private offcanvasService: NgbOffcanvas,private datePipe: DatePipe
  ) { }

  ngOnInit(): void {
    this.UserModel = JSON.parse(localStorage.getItem('UserModel'));
    this.FormInit();
    this.GetAllBeneFactorNationalities();
    this.GetAllBeneFactorData();
    this.GetAllBeneFactorFilters();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      fullName: ['', [Validators.required, this.formService.noSpaceValidator]],
      description: ['', this.formService.noSpaceValidator],
      phone: ['', [Validators.required, Validators.pattern("[0-9]+")]],
      phone2: ['', Validators.pattern("[0-9]+")],
      address: ['', this.formService.noSpaceValidator],
      nationalityId: null,
      faceBook: ['', this.formService.noSpaceValidator],
      welcomeMessage: ['', this.formService.noSpaceValidator],
      InsertUser: null,
      oldFileName: null,
      file: null,
    });
  }

  FillEditForm(item: any) {
    this.fileURL = [];
    this.fileURL.push(item);
    let fileName = item.image.split('/');
    this.ItemForm.setValue({
      id: item.id,
      fullName: item.fullName,
      description: item?.description,
      phone: item?.phone,
      phone2: item?.phone2,
      address: item?.address,
      nationalityId: item?.nationalityId,
      faceBook: item?.faceBook,
      welcomeMessage: item?.welcomeMessage,
      oldFileName: fileName[fileName.length - 1],
      InsertUser: this.UserModel?.userId,
      file: null,
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.BeneFactorId = '';
    this.InputFile.nativeElement.value = '';
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('InsertUser').setValue(this.UserModel?.userId);
  }

  openItemModal(content: any, item: any) {
    this.ResetForm();
    this.isFileExist = false;
    this.fileURL = [];
    this.ImageFile = null;
    if (item)
      this.FillEditForm(item);

    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.BeneFactorId = item.id;
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openCashSidePanel(content: any, item: any) {
    this.BeneFactorValues.beneFactorId = item.id;
    this.BeneFactorValues.fullName = item.fullName;
    this.BeneFactorValues.code = item.code;
    this.BeneFactorValues.totalValue = null;
    this.BeneFactorValues.paymentDate = null;
    this.GetAllBeneFactorParentById();
    this.offcanvasService.open(content, { position: 'end' });
  }

  OpenPdfFileItemModal(content: any) {
    this.PDFHeaderModel = this.pdfService.ConverHeaderToPDFModel(this.BeneFactorHeaders);
    this.RowCount = 20;
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  BeneFactorValueCollapseClick(item: any) {
    item.isCollapsed = !item.isCollapsed;
    this.BeneFactorValuesData.filter(i => i.id != item.id).forEach(item => {
      item.isCollapsed = false;
    });

    if (item.isCollapsed && (!item.values || item.values.length == 0))
      this.benefactorService.GetAllBeneFactorCashDetails(this.BeneFactorValues.beneFactorId, item.id).subscribe(data => {
        let obj = this.BeneFactorValuesData.find(i => i.id == item.id);
        if (obj)
          obj.values = data;
      });
  }

  GetAllBeneFactorParentById() {
    this.benefactorService.GetAllBeneFactorParentById(this.BeneFactorValues.beneFactorId).subscribe(data => {
      this.BeneFactorValuesData = data;
      this.BeneFactorValuesData.forEach(i => i.isCollapsed = false);
    });
  }

  GetAllBeneFactorData() {
    this.benefactorService.GetAllBeneFactorData(this.pagingFilterModel).subscribe(data => {
      this.pagedResponseModel.results = data.table;
      this.BeneFactorHeaders = data.table1;
      this.pagedResponseModel.totalCount = data.table && data.table.length > 0 ? data.table[0].totalCount : 0;
    });
  }

  GetAllBeneFactorNationalities() {
    this.benefactorService.GetAllBeneFactorNationalities(this.pagingFilterModel).subscribe(data => {
      this.NationalityList = data.results;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetAllBeneFactorData();
  }

  GetAllBeneFactorFilters() {
    this.benefactorService.GetAllBeneFactorFilters(this.pagingFilterModel).subscribe(data => {
      this.filterList = data;
    });
  }

  filterChecked(filterList: FilterModel[]) {
    this.pagingFilterModel.filterList = filterList;
    this.PDFModel.filterList = filterList;
    this.GetAllBeneFactorData();
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

  AddNewItem() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.ItemForm.valid;

    if (!isValid) {
      this.formService.validateAllFormFields(this.ItemForm);
      return;
    }

    this.ItemForm.patchValue({ file: this.ImageFile });
    const formData = new FormData();
    this.formService.buildFormData(formData, this.ItemForm.value);
    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.benefactorService.AddNewBeneFactor(formData).subscribe(data => {
        if (data.done) {
          this.toaster.success(data.message);
          this.GetAllBeneFactorData();
          this.GetAllBeneFactorFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.benefactorService.UpdateBeneFactor(formData).subscribe(data => {
        if (data.done) {
          this.toaster.success(data.message);
          this.GetAllBeneFactorData();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    }
  }

  AddNewBeneFactorValues() {
    if (!this.BeneFactorValues.totalValue) {
      this.toaster.warning('برجاء ادخال مبلغ التبرع');
      return;
    }

    if (!this.BeneFactorValues.paymentDate) {
      this.toaster.warning('برجاء ادخال تاريخ التبرع');
      return;
    }

    if (this.BeneFactorValues.totalValue <= 0) {
      this.toaster.warning('برجاء ادخال مبلغ أكبر من صفر');
      return;
    }

    this.BeneFactorValues.beneFactorTypeId = 1;
    this.BeneFactorValues.isParent = true;
    this.BeneFactorValues.isActive = false;
    this.BeneFactorValues.insertUser = this.UserModel?.userId;
    const formData = new FormData();
    this.formService.buildFormData(formData, this.BeneFactorValues);
    this.showLoader = true;
    this.benefactorService.AddNewBeneFactorDetails(formData).subscribe(data => {
      if (data.done) {
        this.toaster.success(data.message);
        this.BeneFactorValues.totalValue = null;
        this.BeneFactorValues.paymentDate = '';
        this.GetAllBeneFactorParentById();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    })
  }

  DeleteItem() {
    this.showLoader = true;
    this.benefactorService.DeleteBeneFactor(this.BeneFactorId).subscribe(data => {
      if (data.done) {
        this.toaster.success(data.message);
        this.GetAllBeneFactorData();
        this.GetAllBeneFactorFilters();
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

  DownloadPdfFile() {
    if (this.pagedResponseModel.results.length == 0) {
      this.toaster.warning('لا يوجد بيانات للتنزيل');
      return;
    }

    let checked = this.PDFHeaderModel.filter(i => i.isSelected);
    let isAllowSummation = this.PDFHeaderModel.filter(i => i.isAllowSummation);
    if (isAllowSummation.length > 1) {
      this.toaster.warning('لا يمكن اختيار جمع قيم العامود الا لعامود واحد فقط');
      return;
    }

    if (checked.length == 0) {
      this.toaster.warning('اختر عامود واحد على الاقل');
      return;
    }

    if (checked.length > 6) {
      this.toaster.warning('لا يمكن اختيار أكثر من 6 أعمدة');
      return;
    }

    if (this.RowCount == 0 || !this.RowCount) {
      this.toaster.warning('أدخل عدد الاسطر');
      return;
    }

    if (this.RowCount > 20) {
      this.toaster.warning('عدد الاسطر لا يتجاوز 20 سطر');
      return;
    }
    let today = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    let fileName = 'المتبرعين' + '_' + today;
    this.PDFModel.headers = this.PDFHeaderModel.filter(i => i.isSelected);
    this.showLoader = true;
    this.pdfService.DownloadFile(this.PDFModel, fileName + '.pdf', 'BeneFactor/ExportBeneFactorsPDFFile?RowCount=' + this.RowCount).subscribe(data => {
      this.showLoader = false;
    });
    this.modalService.dismissAll();
  }

  DownloadExcelFile() {
    if (this.pagedResponseModel.results.length == 0) {
      this.toaster.warning('لا يوجد بيانات للتنزيل');
      return;
    }

    let userName = this.UserModel?.userName;
    let today = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    let fileName = 'المتبرعين' + '_' + today;
    this.PDFModel.headers = this.pdfService.ConverHeaderToPDFModel(this.BeneFactorHeaders);
    this.showLoader = true;
    this.pdfService.DownloadFile(this.PDFModel, fileName + '.xlsx', 'BeneFactor/ExportBeneFactorsExcelFile?UserName=' + userName).subscribe(data => {
      this.showLoader = false;
    });
  }
}
