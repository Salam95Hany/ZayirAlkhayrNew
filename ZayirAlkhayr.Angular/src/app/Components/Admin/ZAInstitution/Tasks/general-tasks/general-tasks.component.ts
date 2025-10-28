import { Component } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaLoaderComponent } from '../../../../../Shared/za-loader/za-loader.component';
import { ZaDropDownFormControlComponent } from '../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { AuthService } from '../../../../../Auth/auth.service';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { FormService } from '../../../../../Services/shared/form.service';
import { SharedService } from '../../../../../Services/shared/shared.service';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { RoleCheckerDirective } from '../../../../../Directives/role-checker.directive';

@Component({
  selector: 'app-general-tasks',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent,
    CommonModule, FormsModule, ReactiveFormsModule, NgbModule, ZaInputWithLabelComponent,RoleCheckerDirective,
    NgIf, NgFor, ZaDropDownFormControlComponent, ZaLoaderComponent],
  templateUrl: './general-tasks.component.html',
  styleUrl: './general-tasks.component.css',
  providers: [DatePipe]
})
export class GeneralTasksComponent {
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
    private formService: FormService, private taskService: TaskService, private datepipe: DatePipe, private sharedService: SharedService,
    private datePipe: DatePipe
  ) { }

  ngOnInit(): void {
    this.CurrentUserId = this.authService.userId;
    this.FormInit();
    this.GetAllUsers();
    this.GetAllGeneralTasksData();
    this.GetAllGeneralTasksFilter();
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
      taskAddedDate: this.datePipe.transform(item?.taskAddedDate, 'yyyy-MM-dd'),
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

  GetAllGeneralTasksData() {
    this.taskService.GetAllGeneralTasksData(this.PagingFilter).subscribe(data => {
      this.TasksData = data.results;
      this.TasksData.forEach(i => {
        i.showButtonStatus = this.CurrentUserId == i.assignToId;
      })
      this.TotalCount = data.totalCount;
    });
  }

  GetAllGeneralTasksFilter() {
    this.taskService.GetAllGeneralTasksFilter(this.PagingFilter).subscribe(data => {
      this.filterList = data.results;
    });
  }

  GetAllUsers() {
    this.sharedService.GetAllUsersSelector().subscribe(data => {
      this.UsersData = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllGeneralTasksData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.PagingFilter.currentPage = 1;
    this.GetAllGeneralTasksData();
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

    if (this.ItemForm.controls['id'].value == 0) {
      this.showLoader = true;
      this.taskService.AddNewGeneralTask(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllGeneralTasksData();
          this.GetAllGeneralTasksFilter();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    } else {
      this.showLoader = true;
      this.taskService.UpdateGeneralTask(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllGeneralTasksData();
          this.GetAllGeneralTasksFilter();
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
    this.taskService.DeleteGeneralTask(this.TaskId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllGeneralTasksData();
        this.GetAllGeneralTasksFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  ConvertTaskStatus(taskId: any, statusId: any) {
    this.showLoader = true;
    this.taskService.ConvertTaskStatus(taskId, statusId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllGeneralTasksData();
        this.GetAllGeneralTasksFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
