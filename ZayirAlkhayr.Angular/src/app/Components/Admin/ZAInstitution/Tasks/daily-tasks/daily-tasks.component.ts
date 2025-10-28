import { Component } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaLoaderComponent } from "../../../../../Shared/za-loader/za-loader.component";
import { NgFor, NgIf } from '@angular/common';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../Auth/auth.service';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-daily-tasks',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaLoaderComponent, FormsModule,NgFor],
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
    // this.CurrentUserId = this.authService.userId;
    // this.GetAllUserTasks();
    this.updateDateTime();
    setInterval(() => this.updateDateTime(), 1000);
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

   currentDate: string = '';
  currentTime: string = '';

  tasks: any[] = [
    {
      title: 'Complete project presentation',
      description: "Finalize slides and prepare speaking notes for tomorrow's meeting",
      priority: 'High',
      dueTime: '14:00',
      category: 'Work',
      completed: false
    },
    {
      title: 'Review team progress reports',
      description: 'Check weekly updates from all team members',
      priority: 'Medium',
      dueTime: '16:00',
      category: 'Management',
      completed: false
    },
    {
      title: 'Morning workout session',
      description: '30 minutes cardio and strength training',
      priority: 'Low',
      dueTime: '07:30',
      category: 'Personal',
      completed: true
    },
    {
      title: 'Grocery shopping',
      description: 'Buy vegetables, fruits, and household items',
      priority: 'Medium',
      dueTime: '18:00',
      category: 'Shopping',
      completed: false
    },
    {
      title: 'Read 30 pages of book',
      description: 'Continue reading "Atomic Habits"',
      priority: 'Low',
      dueTime: '22:00',
      category: 'Personal',
      completed: false
    }
  ];

  searchText: string = '';
  activeFilter: string = 'all';


  updateDateTime() {
    const now = new Date();
    const dateOptions: Intl.DateTimeFormatOptions = {
      weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
    };
    this.currentDate = now.toLocaleDateString('en-US', dateOptions);
    this.currentTime = now.toLocaleTimeString('en-US');
  }

  toggleComplete(task: any) {
    task.completed = !task.completed;
  }

  get totalTasks() {
    return this.tasks.length;
  }

  get completedTasks() {
    return this.tasks.filter(t => t.completed).length;
  }

  get inProgressTasks() {
    return this.tasks.filter(t => !t.completed).length;
  }

  get completionRate() {
    return this.totalTasks ? Math.round((this.completedTasks / this.totalTasks) * 100) : 0;
  }

  setFilter(filter: string) {
    this.activeFilter = filter;
  }

  get filteredTasks(): any[] {
    return this.tasks.filter(task => {
      const matchesSearch = task.title.toLowerCase().includes(this.searchText.toLowerCase());
      if (!matchesSearch) return false;

      switch (this.activeFilter) {
        case 'completed':
          return task.completed;
        case 'active':
          return !task.completed;
        case 'high':
          return task.priority === 'High';
        default:
          return true;
      }
    });
  }

  addTask(newTask: any) {
    this.tasks.push(newTask);
  }

  deleteTask(task: any) {
    this.tasks = this.tasks.filter(t => t !== task);
  }
}
