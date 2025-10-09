import { Component } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { ZaLoaderComponent } from "../../../../../Shared/za-loader/za-loader.component";
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { GeneralStatusService } from '../../../../../Services/zainstitution/general-status.service';
import { FormService } from '../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { AuthService } from '../../../../../Auth/auth.service';

@Component({
  selector: 'app-family-patienttypes',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaLoaderComponent, ZaInputWithLabelComponent, ZaEmptyDataComponent, ReactiveFormsModule,
    NgFor, NgIf, NgbModule],
  templateUrl: './family-patienttypes.component.html',
  styleUrl: './family-patienttypes.component.css'
})
export class FamilyPatienttypesComponent {
  TitleList = ['مؤسسة زائر الخير', 'خدمات اجتماعية', 'أنواع المرض'];
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
    this.GetAllFamilyPatientData();
    this.GetAllFamilyPatientFilter();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      name: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      InsertUser: null
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      name: item?.name,
      InsertUser: this.UserId,
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
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

  GetAllFamilyPatientData() {
    this.generalStatusService.GetAllFamilyPatientData(this.PagingFilter).subscribe(data => {
      this.FamilyNationalityData = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllFamilyPatientFilter() {
    this.generalStatusService.GetAllFamilyPatientFilter().subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllFamilyPatientData();
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
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();

    if (!isValid) {
      return;
    }


    this.showLoader = true;
    if (this.ItemForm.controls['id'].value == 0) {
      this.generalStatusService.AddNewFamilyPatient(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyPatientData();
          this.GetAllFamilyPatientFilter();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.generalStatusService.UpdateFamilyPatient(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllFamilyPatientData();
          this.GetAllFamilyPatientFilter();
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
    this.generalStatusService.DeleteFamilyPatient(this.NationalityId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllFamilyPatientData();
        this.GetAllFamilyPatientFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
