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
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaDropDownFormControlComponent } from '../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { AuthService } from '../../../../../Auth/auth.service';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { FormService } from '../../../../../Services/shared/form.service';
import { SharedService } from '../../../../../Services/shared/shared.service';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { RoleCheckerDirective } from '../../../../../Directives/role-checker.directive';
import { FormDropdownModel } from '../../../../../Models/shared/FormDropdownModel';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-general-tasks',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent,
    CommonModule, FormsModule, ReactiveFormsModule, NgbModule, ZaInputWithLabelComponent, RoleCheckerDirective,
    NgIf, NgFor, ZaDropDownFormControlComponent, NgxLoadingModule],
  templateUrl: './general-tasks.component.html',
  styleUrl: './general-tasks.component.css',
  providers: [DatePipe]
})
export class GeneralTasksComponent {
  TitleList = ['مؤسسة زائر الخير', 'إدارة المهام', 'المهام العامة'];
  PriorityList: FormDropdownModel[] = [
    { value: 'HighPriority', name: 'أولوية عالية' },
    { value: 'MediumPriority', name: 'أولوية متوسطة' },
    { value: 'LowPriority', name: 'أولوية منخفضة' },
  ];
  TasksData: any[] = [];
  UsersData: FormDropdownModel[] = [];
  filterList: FilterModel[] = [];
  CurrentUserId: any;
  showLoader = false;
  TotalCount = 0;
  UserId: any;
  TaskId: any;
  currentDate: string = '';
  currentTime: string = '';
  StatusFilterActive = 'All';
  CompletedCount = 0;
  InProgressCount = 0;
  FinishedCount = 0;
  ItemForm: FormGroup;
  PagingFilter: PagingFilterModel = {
    currentPage: 1,
    pageSize: 10,
    filterList: []
  }
  formErrors = {
    title: '',
    description: '',
    taskAddedDate: '',
    priority: '',
    assignTo: ''
  };

  constructor(private taskService: TaskService, private toaster: ToastrService, private authService: AuthService, private modalService: NgbModal,
    private fb: FormBuilder, private formService: FormService, private sharedService: SharedService, private datePipe: DatePipe
  ) { }

  ngOnInit(): void {
    this.CurrentUserId = this.authService.userId;
    this.PagingFilter.userId = this.CurrentUserId;
    this.FormInit();
    this.GetAllGeneralTasksData();
    this.GetAllGeneralTasksFilter();
    this.GetAllGeneralTaskStatistics();
    this.GetAllUsers();
    this.updateDateTime();
    setInterval(() => this.updateDateTime(), 1000);
  }

  updateDateTime() {
    const now = new Date();
    const dateOptions: Intl.DateTimeFormatOptions = {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    };

    this.currentDate = now.toLocaleDateString('ar-EG', dateOptions);
    this.currentTime = now.toLocaleTimeString('ar-EG', { hour12: true });
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      title: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      description: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      taskAddedDate: ['', [Validators.required]],
      dueDate: null,
      priority: ['', [Validators.required]],
      assignTo: ['', [Validators.required]],
      insertUser: null
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.patchValue({
      id: item.id,
      title: item?.title ?? '',
      description: item?.description ?? '',
      taskAddedDate: this.datePipe.transform(item?.taskAddedDate, 'yyyy-MM-dd') ?? null,
      dueDate: this.datePipe.transform(item?.dueDate, 'yyyy-MM-dd') ?? null,
      priority: item?.priority?.toString() ?? '',
      assignTo: item?.assignToId?.toString() ?? '',
      insertUser: item?.insertUser
    });
  }

  OpenItemModal(content: any, item: any) {
    this.ItemForm.reset();
    this.TaskId = item?.id;
    if (item)
      this.FillEditForm(item);
    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  GetAllGeneralTasksData() {
    this.showLoader = true;
    this.taskService.GetAllGeneralTasksData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      debugger;
      this.TasksData = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllGeneralTasksFilter() {
    this.taskService.GetAllGeneralTasksFilter(this.PagingFilter).subscribe(data => {
      this.filterList = data.results;
    });
  }

  GetAllGeneralTaskStatistics() {
    this.taskService.GetAllGeneralTaskStatistics().subscribe(data => {
      this.CompletedCount = data.results[0].completedCount ?? 0;
      this.InProgressCount = data.results[0].inProgressCount ?? 0;
      this.FinishedCount = data.results[0].finishedCount ?? 0;
    });
  }

  OnPageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllGeneralTasksData();
  }

  OnFilterClick(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.PagingFilter.currentPage = 1;
    this.GetAllGeneralTasksData();
  }

  GetAllUsers() {
    this.sharedService.GetAllUsersSelector().subscribe(data => {
      this.UsersData = data.results;
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

  AddItem() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();

    if (!isValid)
      return;

    this.ItemForm.patchValue({ insertUser: this.CurrentUserId });

    if (!this.TaskId) {
      this.showLoader = true;
      this.taskService.AddNewGeneralTask(this.ItemForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.toaster.success(data.message);
          this.GetAllGeneralTasksData();
          this.GetAllGeneralTasksFilter();
          this.GetAllGeneralTaskStatistics();
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
          this.GetAllGeneralTaskStatistics();
          this.modalService.dismissAll();
        }
        else
          this.toaster.error(data.message);
        this.showLoader = false;
      });
    }

  }

  DeleteItem(taskId: any) {
    this.showLoader = true;
    this.taskService.DeleteGeneralTask(taskId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllGeneralTasksData();
        this.GetAllGeneralTasksFilter();
        this.GetAllGeneralTaskStatistics();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }
}
