import { Component, OnInit } from '@angular/core';
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-net-value',
  standalone: true,
  imports: [ZaFiltersComponent, ZaBreadcrumbComponent,CommonModule],
  templateUrl: './net-value.component.html',
  styleUrl: './net-value.component.css'
})
export class NetValueComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'إدارة الحسابات', 'الباقي'];
  TotalExportValue = 0;
  TotalImportValue = 0;
  NetValue = 0;
   PagingFilter: PagingFilterModel = {
      currentPage: 1,
      pageSize: 20,
      filterList: []
    };

  constructor(private taskService: TaskService) {

  }

  ngOnInit(): void {
    this.GetAllImportExportMonyStatistics();

  }

  GetAllImportExportMonyStatistics() {
    this.taskService.GetAllImportExportMonyStatistics(this.PagingFilter).subscribe(data => {
      this.TotalExportValue = data.results[0].allExportMoney ?? 0;
      this.TotalImportValue = data.results[0].importMoney ?? 0;
      this.NetValue = this.TotalImportValue - this.TotalExportValue;
    });
  }

}
