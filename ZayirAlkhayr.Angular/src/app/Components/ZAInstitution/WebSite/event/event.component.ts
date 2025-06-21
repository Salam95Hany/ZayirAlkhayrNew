import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { ZaBreadcrumbComponent } from '../../../../Shared/za-breadcrumb/za-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../Shared/za-empty-data/za-empty-data.component';
import { FilterModel } from '../../../../Models/shared/FilterModel';
import { FileSortingModel, UploadFileModel } from '../../../../Models/shared/FileModel';
import { PagingFilterModel } from '../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../Models/shared/PagedResponseModel';
import { ValidationFormService } from '../../../../Services/shared/validation-form.service';
import { ZaWebsiteService } from '../../../../Services/zainstitution/za-website.service';

@Component({
  selector: 'app-event',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule],
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
  isFilter = false;
  isFileExist = false;
  EventId: any;
  UserModel: any;
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

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder,
    private formService: ValidationFormService, private websiteService: ZaWebsiteService
  ) { }

  ngOnInit(): void {
    this.UserModel = JSON.parse(localStorage.getItem('UserModel'));;
    this.FormInit();
    this.GetAllEvents();
    this.GetWebsiteAdminFilters();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      title: ['', [Validators.required, this.formService.noSpaceValidator]],
      description: ['', [Validators.required, this.formService.noSpaceValidator]],
      fromDate: ['', Validators.required],
      toDate: ['', Validators.required],
      isVisible: true,
      insertUser: null
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
      insertUser: this.UserModel?.userId ?? null
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.EventId = '';
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('isVisible').setValue(true);
    this.ItemForm.get('insertUser').setValue(this.UserModel?.userId);
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
      this.multiFileURL = data;
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
    this.websiteService.GetAllEvents(this.pagingFilterModel).subscribe(data => {
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetAllEvents();
  }

  GetWebsiteAdminFilters() {
    this.websiteService.GetAllWebPagesFilters('Event').subscribe(data => {
      this.filterList = data;
    });
  }

  filterChecked(filterItems: FilterModel[]) {
    this.pagingFilterModel.filterList = filterItems;
    this.GetAllEvents();
  }

  onMultiFileChange(event: any) {
    let fileSizeValidate = false;
    [...event.target.files].forEach(element => {
      let fileSize = this.formService.getFileSize(element);
      if (fileSize > 1) {
        this.toaster.warning(`هذا الملف ${element.name} حجمه أكبر من 1 ميجا`);
        fileSizeValidate = true;
      }
    });

    if (fileSizeValidate)
      return;

    this.formService.onSelectedMultiFile([...event.target.files]).then(data => {
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
      if (data.done) {
        this.modalService.dismissAll();
        this.toaster.success(data.message);
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  AddNewEvent() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.ItemForm.valid;

    if (!isValid) {
      this.formService.validateAllFormFields(this.ItemForm);
      return;
    }
    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.websiteService.AddNewEvent(this.ItemForm.value).subscribe(data => {
        if (data.done) {
          this.toaster.success(data.message);
          this.GetAllEvents();
          this.GetWebsiteAdminFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.websiteService.UpdateEvent(this.ItemForm.value).subscribe(data => {
        if (data.done) {
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
      if (data.done) {
        this.toaster.success(data.message);
        this.GetAllEvents();
        this.GetWebsiteAdminFilters();
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
      if (data.done) {
        this.modalService.dismissAll();
        this.toaster.success(data.message);
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
