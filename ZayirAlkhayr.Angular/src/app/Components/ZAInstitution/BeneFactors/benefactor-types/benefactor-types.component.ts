import { Component, OnInit } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../Shared/za-filters/za-filters.component";
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZaEmptyDataComponent } from '../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FilterModel } from '../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../Models/shared/PagedResponseModel';
import { ToastrService } from 'ngx-toastr';
import { ValidationFormService } from '../../../../Services/shared/validation-form.service';
import { BenefactorService } from '../../../../Services/zainstitution/benefactor.service';

@Component({
  selector: 'app-benefactor-types',
  standalone: true,
  imports: [CommonModule, FormsModule, ZaBreadcrumbComponent, ZaPaginationComponent,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, ReactiveFormsModule],
  templateUrl: './benefactor-types.component.html',
  styleUrl: './benefactor-types.component.css'
})
export class BenefactorTypesComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'إدارة المتبرعين', 'أنواع التبرع'];
  filterList: FilterModel[] = [];
  ItemForm: FormGroup;
  showLoader: boolean = false;
  isFilter = false;
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

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder,
    private formService: ValidationFormService, private benefactorService: BenefactorService) {

  }

  ngOnInit(): void {
    this.UserModel = JSON.parse(localStorage.getItem('UserModel'));
    this.FormInit();
    this.GetAllBeneFactorTypes();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      name: ['', [Validators.required, this.formService.noSpaceValidator]],
      InsertUser: null
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      name: item?.name,
      InsertUser: this.UserModel?.userId,
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('InsertUser').setValue(this.UserModel?.userId);
  }

  openItemModal(content: any, item: any) {
    this.ResetForm();
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

  GetAllBeneFactorTypes() {
    this.benefactorService.GetAllBeneFactorTypes(this.pagingFilterModel).subscribe(data => {
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetAllBeneFactorTypes();
  }

  filterChecked(filterList: FilterModel[]) {
    this.pagingFilterModel.filterList = filterList;
    this.GetAllBeneFactorTypes();
  }

  AddNewItem() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.ItemForm.valid;

    if (!isValid) {
      this.formService.validateAllFormFields(this.ItemForm);
      return;
    }
    this.showLoader = true;
    this.benefactorService.AddNewBeneFactorType(this.ItemForm.value).subscribe(data => {
      if (data.done) {
        this.toaster.success(data.message);
        this.GetAllBeneFactorTypes();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
