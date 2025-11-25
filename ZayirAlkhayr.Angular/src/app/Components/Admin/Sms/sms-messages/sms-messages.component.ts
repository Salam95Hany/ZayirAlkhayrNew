import { Component, OnInit } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../Shared/za-pagination/za-pagination.component";
import { NgxLoadingModule } from "ngx-loading";
import { PagingFilterModel } from '../../../../Models/shared/PagingFilterModel ';
import { TaskService } from '../../../../Services/zainstitution/task.service';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { NotificationsService } from '../notifications.service';

@Component({
  selector: 'app-sms-messages',
  standalone: true,
  imports: [ZaBreadcrumbComponent, ZaPaginationComponent, NgxLoadingModule, NgFor, CommonModule],
  templateUrl: './sms-messages.component.html',
  styleUrl: './sms-messages.component.css'
})
export class SmsMessagesComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'إدارة الرسائل', 'الرسائل'];
  MessageData: any[] = [];
  currentDate = '';
  currentTime = '';
  showLoader = false;
  TotalCount = 0;
  PagingFilter: PagingFilterModel = {
    currentPage: 1,
    pageSize: 10,
    filterList: []
  }

  constructor(private taskService: TaskService, private notificationService: NotificationsService) {

  }

  ngOnInit(): void {
    this.updateDateTime();
    setInterval(() => this.updateDateTime(), 1000);
    const es = this.notificationService.connect();
    es.onmessage = (event) => {
      console.log("Event received: ", event.data);

      if (event.data === 'Message_Added') {
        this.GetAllSmsMessageData();
      }
    };

    this.GetAllSmsMessageData();
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

  GetAllSmsMessageData() {
    this.showLoader = true;
    this.taskService.GetAllSmsMessageData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.MessageData = data.results;
      this.TotalCount = data.totalCount;
      console.log('this.MessageData => ', this.MessageData);
    });
  }

  OnPageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllSmsMessageData();
  }

}
