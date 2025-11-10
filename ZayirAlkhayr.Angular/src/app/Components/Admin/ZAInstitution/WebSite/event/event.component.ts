import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { ZaBreadcrumbComponent } from '../../../../../Shared/za-breadcrumb/za-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { FileSortingModel, UploadFileModel } from '../../../../../Models/shared/FileModel';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../../Models/shared/PagedResponseModel';
import { ZaWebsiteService } from '../../../../../Services/zainstitution/za-website.service';
import { FileService } from '../../../../../Services/shared/file.service';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { FormService } from '../../../../../Services/shared/form.service';
import { AuthService } from '../../../../../Auth/auth.service';
import { RoleCheckerDirective } from '../../../../../Directives/role-checker.directive';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-event',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent, RoleCheckerDirective,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule, NgxLoadingModule],
  templateUrl: './event.component.html',
  styleUrl: './event.component.css'
})
export class EventComponent implements OnInit {
  @ViewChild('InputMultiFile') InputMultiFile: ElementRef;
  TitleList = ['مؤسسة زائر الخير', 'موقع زائر الخير', 'الفعاليات'];
  filterList: FilterModel[] = [];
  fileURL: any[] = [];
  multiFileURL: any[] = [];
  multiImagesFile: any[] = [];
  FileSotingModel: FileSortingModel[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  isFilter = true;
  isFileExist = false;
  EventId: any;
  UserId: any;
  FileModel: UploadFileModel = {
    files: [],
    deletedFiles: []
  }
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
    title: '',
    description: '',
    fromDate: '',
    toDate: ''
  };

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder,private authService: AuthService,
    private fileService: FileService, private websiteService: ZaWebsiteService, private formService: FormService
  ) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllEvents();
    this.GetEventFilters();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      title: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      description: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      fromDate: ['', Validators.required],
      toDate: ['', Validators.required],
      isVisible: true,
      insertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      title: item.title,
      description: item?.description,
      fromDate: item?.fromDate,
      toDate: item?.toDate,
      isVisible: item?.isVisible,
      insertUser: this.UserId ?? null
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.EventId = '';
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('isVisible').setValue(true);
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

  openAddImagesModal(content: any, item: any) {
    this.multiFileURL = [];
    this.multiImagesFile = [];
    this.FileModel = { files: [], deletedFiles: [] };
    this.EventId = item.id;
    this.InputMultiFile.nativeElement.value = '';
    this.websiteService.GetEventSliderImagesById(item.id).subscribe(data => {
      this.multiFileURL = data.results;
      this.modalService.open(content, {
        size: 'xl',
        scrollable: true,
        centered: true
      });
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.EventId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllEvents() {
    this.showLoader = true;
    this.websiteService.GetAllEvents(this.pagingFilterModel).subscribe(data => {
      this.showLoader = false;
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetAllEvents();
  }

  GetEventFilters() {
    this.websiteService.GetEventFilters().subscribe(data => {
      this.filterList = data.results;
    });
  }

  filterChecked(filterItems: FilterModel[]) {
    this.pagingFilterModel.filterList = filterItems;
    this.pagingFilterModel.currentPage = 1;
    this.GetAllEvents();
  }

  onMultiFileChange(event: any) {
    let fileSizeValidate = false;
    [...event.target.files].forEach(element => {
      let fileSize = this.fileService.getFileSize(element);
      if (fileSize > 1) {
        this.toaster.warning(`هذا الملف ${element.name} حجمه أكبر من 1 ميجا`);
        fileSizeValidate = true;
      }
    });

    if (fileSizeValidate)
      return;

    this.fileService.onSelectedMultiFile([...event.target.files]).then(data => {
      this.multiFileURL.push(...data?.urls);
      this.multiImagesFile.push(...data?.fileContents);
    });
  }

  DeleteMultiImageFiles(index: number, item: any) {
    this.multiFileURL.splice(index, 1);
    this.InputMultiFile.nativeElement.value = '';

    if (item?.id) {
      let fileName = item.image.split('\\');
      this.FileModel.deletedFiles.push({ id: item.id, fileName: fileName[fileName.length - 1] });
    } else {
      this.multiImagesFile = this.multiImagesFile.filter(i => i.uniqueId != item.uniqueId);
    }
  }

  AddMultiImagesFile() {
    if (this.multiImagesFile.length == 0 && this.FileModel.deletedFiles.length == 0)
      return;

    this.FileModel.id = this.EventId;
    this.FileModel.files = this.multiImagesFile.map(i => i.file);
    const formData = new FormData();
    this.formService.buildFormData(formData, this.FileModel);
    this.showLoader = true;
    this.websiteService.AddEventSliderImage(formData).subscribe(data => {
      if (data.isSuccess) {
        this.modalService.dismissAll();
        this.toaster.success(data.message);
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
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

  AddNewEvent() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();

    if (!isValid) {
      return;
    }

    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.websiteService.AddNewEvent(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllEvents();
          this.GetEventFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.websiteService.UpdateEvent(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllEvents();
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
    this.websiteService.DeleteEvent(this.EventId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllEvents();
        this.GetEventFilters();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  ApplyFilesSorting() {
    let checked = this.multiFileURL.every(i => i.id);
    if (!checked) {
      this.toaster.warning('برجاء اضافة الصور الجديدة اولا');
      return;
    }

    let FileSotingModel = this.multiFileURL.map<FileSortingModel>(i => { return { fileId: i.id, displayOrder: i.displayOrder } });
    this.showLoader = true;
    this.websiteService.ApplyEventFilesSorting(FileSotingModel, this.EventId).subscribe(data => {
      if (data.isSuccess) {
        this.modalService.dismissAll();
        this.toaster.success(data.message);
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
