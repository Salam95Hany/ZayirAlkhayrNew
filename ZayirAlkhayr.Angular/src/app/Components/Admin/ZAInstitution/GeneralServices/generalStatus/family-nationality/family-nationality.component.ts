import { Component, OnInit } from '@angular/core';
import { ZaLoaderComponent } from "../../../../../../Shared/za-loader/za-loader.component";
import { ZaBreadcrumbComponent } from "../../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../../Shared/za-filters/za-filters.component";
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStatusService } from '../../../../../../Services/zainstitution/general-status.service';
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { AuthService } from '../../../../../../Auth/auth.service';

@Component({
  selector: 'app-family-nationality',
  standalone: true,
  imports: [ZaLoaderComponent, ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent,NgbModule,
    NgIf,NgFor,ZaInputWithLabelComponent,ReactiveFormsModule
  ],
  templateUrl: './family-nationality.component.html',
  styleUrl: './family-nationality.component.css'
})
export class FamilyNationalityComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'خدمات اجتماعية', 'الجنسيات'];
  FamilyNationalityData: any[] = [];
  FilterList: FilterModel[] = [];
  showLoader = false;
  isFilter = false;
  TotalCount = 0;
  ItemForm: FormGroup;
  UserId: any;
  NationalityId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    name: ''
  };

  constructor(private modalService: NgbModal, private generalStatusService: GeneralStatusService, private formService: FormService
    , private fb: FormBuilder, private toaster: ToastrService,private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.FormInit();
    this.GetAllFamilyNationalitiesData();
    this.GetAllFamilyNationalitiesFilter();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      name: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      InsertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      name: item?.name,
      InsertUser: this.UserId,
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('InsertUser').setValue(this.UserId);
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

  openDeleteItemModal(content: any, item: any) {
    this.NationalityId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllFamilyNationalitiesData() {
    this.generalStatusService.GetAllFamilyNationalitiesData(this.PagingFilter).subscribe(data => {
      this.FamilyNationalityData = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllFamilyNationalitiesFilter() {
    this.generalStatusService.GetAllFamilyNationalitiesFilter(this.PagingFilter).subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllFamilyNationalitiesData();
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
    debugger
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();

    if (!isValid) {
      return;
    }

    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.generalStatusService.AddNewFamilyNationality(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyNationalitiesData();
          this.GetAllFamilyNationalitiesFilter();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.generalStatusService.UpdateFamilyNationality(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyNationalitiesData();
          this.GetAllFamilyNationalitiesFilter();
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
    this.generalStatusService.DeleteFamilyNationality(this.NationalityId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllFamilyNationalitiesData();
        this.GetAllFamilyNationalitiesFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
