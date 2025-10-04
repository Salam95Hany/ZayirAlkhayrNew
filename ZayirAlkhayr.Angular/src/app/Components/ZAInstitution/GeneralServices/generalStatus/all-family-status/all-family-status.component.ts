import { Component } from '@angular/core';
import { ZaPaginationComponent } from "../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaLoaderComponent } from "../../../../../Shared/za-loader/za-loader.component";
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZaDropDownFormControlComponent } from '../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../Auth/auth.service';
import { SharedService } from '../../../../../Services/shared/shared.service';
import { FormService } from '../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-all-family-status',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent,
    CommonModule, FormsModule, ReactiveFormsModule, NgbModule, ZaInputWithLabelComponent,RouterModule,
    NgIf, NgFor, ZaDropDownFormControlComponent, ZaLoaderComponent],
  templateUrl: './all-family-status.component.html',
  styleUrl: './all-family-status.component.css'
})
export class AllFamilyStatusComponent {
TitleList = ['مؤسسة زائر الخير', 'إدارة المهام', 'المهام العامة'];
  TasksData: any[] = [];
  UsersData: any[] = [];
  filterList: FilterModel[] = [];
  CurrentUserId: any;
  ItemForm: FormGroup;
  TotalCount = 0;
  isFilter = true;
  showLoader = false;
  UserName = 'تعيين ل';
  TaskId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    task: '',
    taskAddedDate: ''
  };

  constructor(private toaster: ToastrService, private modalService: NgbModal, private fb: FormBuilder, private authService: AuthService,
    private formService: FormService, private sharedService: SharedService  ) { }

  ngOnInit(): void {
    this.CurrentUserId = this.authService.userId;
    this.FormInit();
    this.GetAllUsers();
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      task: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      taskAddedDate: ['', Validators.required],
      assignTo: null,
      InsertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.setValue({
      id: item.id,
      task: item.task,
      assignTo: item?.assignToId,
      InsertUser: this.CurrentUserId,
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('InsertUser').setValue(this.CurrentUserId);
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
    this.TaskId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllUsers() {
    this.sharedService.GetAllUsersSelector().subscribe(data => {
      this.UsersData = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.PagingFilter.currentPage = 1;
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

    if (!isValid)
      return;

    
  }

  DeleteItem() {
   
  }
}
