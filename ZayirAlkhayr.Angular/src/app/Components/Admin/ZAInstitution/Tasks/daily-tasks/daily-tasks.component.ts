import { Component } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaLoaderComponent } from "../../../../../Shared/za-loader/za-loader.component";
import { NgFor, NgIf } from '@angular/common';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../Auth/auth.service';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';

@Component({
  selector: 'app-daily-tasks',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaLoaderComponent, NgFor, NgIf,ZaEmptyDataComponent],
  templateUrl: './daily-tasks.component.html',
  styleUrl: './daily-tasks.component.css'
})
export class DailyTasksComponent {
  TitleList = ['مؤسسة زائر الخير', 'إدارة المهام', 'المهام اليومية'];
  TasksData: any[] = [];
  CurrentUserId: any;
  showLoader = false;
  TotalCount = 0;
  UserId: any;
  TaskId: any;

  constructor(private taskService: TaskService, private toaster: ToastrService, private authService: AuthService) { }

  ngOnInit(): void {
    this.CurrentUserId = this.authService.userId;
    this.GetAllUserTasks();
  }

  GetAllUserTasks() {
    this.taskService.GetAllUserTasks(this.CurrentUserId).subscribe(data => {
      this.TasksData = data.results;
      this.TotalCount = data.totalCount;
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
}
