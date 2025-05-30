import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FilterModel } from '../../../Models/shared/FilterModel';
import { FileSortingModel, UploadFileModel } from '../../../Models/shared/FileModel';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PagingFilterModel } from '../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ValidationFormService } from '../../../Services/shared/validation-form.service';
import { ZaWebsiteService } from '../../../Services/zainstitution/za-website.service';
import { CommonModule } from '@angular/common';
import { ZaBreadcrumbComponent } from '../../../Shared/za-breadcrumb/za-breadcrumb.component';
import { ZaPaginationComponent } from '../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../Shared/za-empty-data/za-empty-data.component';

@Component({
  selector: 'app-project',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent,
      ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule],
  templateUrl: './project.component.html',
  styleUrl: './project.component.css'
})
export class ProjectComponent implements OnInit {
 @ViewChild('InputMultiFile') InputMultiFile: ElementRef;
  TitleList = ['مؤسسة زائر الخير', 'موقع زائر الخير', 'المشاريع'];
  filterList: FilterModel[] = [];
  fileURL: any[] = [];
  multiFileURL: any[] = [];
  multiImagesFile: any[] = [];
  FileSotingModel: FileSortingModel[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  isFilter = false;
  isFileExist = false;
  ProjectId: any;
  UserModel: any;
  FileModel: UploadFileModel = {
    files: [],
    deletedFiles: []
  } as UploadFileModel;
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
    this.GetAllProjects();
    this.GetWebsiteAdminFilters();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      title: ['', [Validators.required, this.formService.noSpaceValidator]],
      description: ['', [Validators.required, this.formService.noSpaceValidator]],
      totalDonationAmount: ['', [Validators.required, this.formService.noSpaceValidator,Validators.pattern("[0-9]+")]],
      benefactorCount: ['', [Validators.required, this.formService.noSpaceValidator,Validators.pattern("[0-9]+")]],
      totalAmount: ['', [Validators.required, this.formService.noSpaceValidator,Validators.pattern("[0-9]+")]],
      remainingAmount: ['', [Validators.required, this.formService.noSpaceValidator,Validators.pattern("[0-9]+")]],
      isVisible: true,
      insertUser: null
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      title: item.title,
      description: item?.description,
      totalDonationAmount: item?.totalDonationAmount,
      benefactorCount: item?.benefactorCount,
      totalAmount: item?.totalAmount,
      remainingAmount: item?.remainingAmount,
      isVisible: item?.isVisible,
      insertUser: this.UserModel?.userId ?? null
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ProjectId = '';
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
    this.ProjectId = item.id;
    this.InputMultiFile.nativeElement.value = '';
    this.websiteService.GetProjectsSliderImagesById(item.id).subscribe(data => {
      this.multiFileURL = data;
      this.modalService.open(content, {
        size: 'xl',
        scrollable: true,
        centered: true
      });
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.ProjectId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllProjects() {
    this.websiteService.GetAllProjects(this.pagingFilterModel).subscribe(data => {
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
     this.GetAllProjects();
  }

 GetWebsiteAdminFilters() {
    this.websiteService.GetAllWebPagesFilters('Project').subscribe(data => {
      this.filterList = data;

    });
  }

  filterChecked(filterItems: FilterModel[]) {
    this.pagingFilterModel.filterList = filterItems;
    this.GetAllProjects();
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

    this.FileModel.id = this.ProjectId;
    this.FileModel.files = this.multiImagesFile.map(i => i.file);
    const formData = new FormData();
    this.formService.buildFormData(formData, this.FileModel);
    this.showLoader = true;
    this.websiteService.AddProjectsSliderImage(formData).subscribe(data => {
      if (data.done) {
        this.modalService.dismissAll();
        this.toaster.success(data.message);
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  AddNewProject() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.ItemForm.valid;

    if (!isValid) {
      this.formService.validateAllFormFields(this.ItemForm);
      return;
    }
    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.websiteService.AddNewProjects(this.ItemForm.value).subscribe(data => {
        if (data.done) {
          this.toaster.success(data.message);
          this.GetAllProjects();
          this.GetWebsiteAdminFilters();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.websiteService.UpdateProjects(this.ItemForm.value).subscribe(data => {
        if (data.done) {
          this.toaster.success(data.message);
          this.GetAllProjects();
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
    this.websiteService.DeleteProjects(this.ProjectId).subscribe(data => {
      if (data.done) {
        this.toaster.success(data.message);
        this.GetAllProjects();
        this.GetWebsiteAdminFilters();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
        this.showLoader = false;
    });
  }
}
