import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ZaBreadcrumbComponent } from '../../../../Shared/za-breadcrumb/za-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../Shared/za-empty-data/za-empty-data.component';
import { FilterModel } from '../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../Models/shared/PagedResponseModel';
import { ZaWebsiteService } from '../../../../Services/zainstitution/za-website.service';
import { FileService } from '../../../../Services/shared/file.service';
import { FormService } from '../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../Services/shared/custom-validators';

@Component({
  selector: 'app-slide-image',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule],
  templateUrl: './slide-image.component.html',
  styleUrl: './slide-image.component.css'
})
export class SlideImageComponent implements OnInit {
  @ViewChild('InputFile') InputFile: ElementRef;
  // @ViewChild('DetailsSidePanel', { static: true }) DetailsSidePanel: TemplateRef<any>;
  TitleList = ['مؤسسة زائر الخير', 'موقع زائر الخير', 'شريط الصور'];
  filterList: FilterModel[] = [];
  fileURL: any[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  isFilter = true;
  isFileExist = false;
  ImageFile: any;
  UserModel: any;
  SliderId: number;
  pagingFilterModel: PagingFilterModel = {
    currentPage: 1,
    pageSize: 20,
    filterList: []
  };
  pagedResponseModel: PagedResponseModel<any[]> = {
    totalCount: 0,
    results: [],
  };

  formErrors = {
    title: ''
  };

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder,
    private fileService: FileService, private websiteService: ZaWebsiteService, private formService: FormService
  ) { }

  ngOnInit(): void {
    this.UserModel = JSON.parse(localStorage.getItem('UserModel'));
    this.FormInit();
    this.GetHomeSliderImages();
    this.GetWebsiteAdminFilters();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      title: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      isVisible: true,
      InsertUser: null,
      oldFileName: null,
      file: null,
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.fileURL = [];
    this.fileURL.push(item);
    let fileName = item.image.split('/');
    this.ItemForm.setValue({
      id: item.id,
      title: item?.title,
      isVisible: item?.isVisible,
      oldFileName: fileName[fileName.length - 1],
      InsertUser: this.UserModel?.userId,
      file: null,
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('isVisible').setValue(true);
    this.ItemForm.get('InsertUser').setValue(this.UserModel?.userId);
  }

  // openSidePanel(journalEntryId: number) {
  //   this.EntryId = journalEntryId;
  //   this.offcanvasService.open(this.DetailsSidePanel, { panelClass: 'details-panel', position: 'end' });
  // }

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
    })
  }

  openDeleteItemModal(content: any, item: any) {
    this.SliderId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    })
  }

  GetHomeSliderImages() {
    this.websiteService.GetHomeSliderImages(this.pagingFilterModel).subscribe(data => {
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetHomeSliderImages();
  }

  GetWebsiteAdminFilters() {
    this.websiteService.GetAllWebPagesFilters('Home').subscribe(data => {
      this.filterList = data.results;
    });
  }

  filterChecked(filterItems: FilterModel[]) {
    this.pagingFilterModel.filterList = filterItems;
    this.GetHomeSliderImages();
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

   validateForm(): boolean {
    this.formService.markFormGroupTouched(this.ItemForm);
    if (this.ItemForm.valid) {
      return true;
    } else {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, false)
      return false;
    }
  }

  AddNewSlider() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();
    this.isFileExist = this.fileURL.length == 0;
    if (this.isFileExist)
      return;

    if (!isValid)
      return;

    this.ItemForm.patchValue({ file: this.ImageFile });
    const formData = new FormData();
    this.formService.buildFormData(formData, this.ItemForm.value);
    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.websiteService.AddNewSliderImage(formData).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetHomeSliderImages();
          this.GetWebsiteAdminFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.websiteService.UpdateSliderImage(formData).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetHomeSliderImages();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    }
  }

  DeleteSlider() {
    this.showLoader = true;
    this.websiteService.DeleteSliderImage(this.SliderId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetHomeSliderImages();
        this.GetWebsiteAdminFilters();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
