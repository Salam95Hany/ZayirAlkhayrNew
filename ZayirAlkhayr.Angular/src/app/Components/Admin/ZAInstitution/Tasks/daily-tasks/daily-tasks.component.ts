import { Component, OnDestroy } from '@angular/core';
import { AdminBreadcrumbComponent } from '../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../Auth/auth.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZaPaginationComponent } from "../../../../../Shared/za-pagination/za-pagination.component";
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { CommonModule, NgClass, NgFor, NgIf } from '@angular/common';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { FormService } from '../../../../../Services/shared/form.service';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-daily-tasks',
  standalone: true,
  imports: [FormsModule, NgClass, NgIf, AdminBreadcrumbComponent, ZaPaginationComponent, NgFor, CommonModule,
    ZaInputWithLabelComponent, ReactiveFormsModule, NgbModule, NgxLoadingModule],
  templateUrl: './daily-tasks.component.html',
  styleUrl: './daily-tasks.component.css'
})
export class DailyTasksComponent implements OnDestroy {
  TitleList = ['مؤسسة زائر الخير', 'إدارة المهام', 'المهام اليومية'];
  TasksData: any[] = [];
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
    comment: '',
  };
  private clockTimer?: ReturnType<typeof setInterval>;

  constructor(private taskService: TaskService, private toaster: ToastrService, private authService: AuthService, private modalService: NgbModal,
    private fb: FormBuilder, private formService: FormService
  ) { }

  ngOnInit(): void {
    this.CurrentUserId = this.authService.userId;
    this.PagingFilter.userId = this.CurrentUserId;
    this.FormInit();
    this.GetAllUserTasks();
    this.updateDateTime();
    this.clockTimer = setInterval(() => this.updateDateTime(), 1000);
  }

  ngOnDestroy(): void {
    if (this.clockTimer)
      clearInterval(this.clockTimer);
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
      comment: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.patchValue({
      comment: item?.comment ?? '',
    });
  }

  OpenItemModal(content: any, item: any) {
    this.ItemForm.reset();
    this.TaskId = item.id;
    this.FillEditForm(item);
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetAllUserTasks() {
    this.showLoader = true;
    this.taskService.GetAllUserTasks(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.TasksData = data.results.item1;
      this.TotalCount = data.totalCount;
      this.CompletedCount = this.TasksData.filter(i => i.statusId == 1)?.length ?? 0;
      this.InProgressCount = this.TasksData.filter(i => i.statusId == 2)?.length ?? 0;
      if (data.results.item2 > 0) {
        this.FinishedCount = data.results.item2;
      } else {
        this.FinishedCount = 0;
      }
    });
  }

  OnPageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllUserTasks();
  }

  OnFilterClick(status: string) {
    this.PagingFilter.filterList = [];
    this.PagingFilter.currentPage = 1;
    if (status != 'SearchText') {
      this.StatusFilterActive = status;
      this.PagingFilter.searchText = '';
      this.PagingFilter.filterList.push({
        categoryName: 'TaskStatus',
        itemId: status
      });
      this.GetAllUserTasks();
    }
    else {
      const searchText = this.PagingFilter.searchText?.trim() ?? '';
      this.PagingFilter.searchText = searchText;
      if (searchText.length > 2 || !searchText) {
        this.StatusFilterActive = 'All';
        this.PagingFilter.filterList.push({
          categoryName: 'SearchText',
          itemId: this.PagingFilter.searchText
        });
        this.GetAllUserTasks();
      }
    }
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

  AddEditTaskComment() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();

    if (!isValid)
      return;

    this.showLoader = true;
    this.taskService.AddEditTaskComment(this.TaskId, this.ItemForm.value?.comment).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllUserTasks();
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
        this.GetAllUserTasks();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  trackByTask(_: number, item: any): number {
    return item.id;
  }
}
