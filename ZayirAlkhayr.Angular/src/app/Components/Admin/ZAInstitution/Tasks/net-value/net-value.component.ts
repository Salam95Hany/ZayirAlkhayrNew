import { Component, OnInit } from '@angular/core';
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { CommonModule } from '@angular/common';
import { FilterModel } from '../../../../../Models/shared/FilterModel';

@Component({
  selector: 'app-net-value',
  standalone: true,
  imports: [ZaFiltersComponent, ZaBreadcrumbComponent, CommonModule],
  templateUrl: './net-value.component.html',
  styleUrl: './net-value.component.css'
})
export class NetValueComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'إدارة الحسابات', 'الباقي'];
  filterList: FilterModel[] = [];
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
    this.GetAllAccountsImportMonyFilters();

  }

  GetAllImportExportMonyStatistics() {
    this.taskService.GetAllImportExportMonyStatistics(this.PagingFilter).subscribe(data => {
      this.TotalExportValue = data.results[0].allExportMoney ?? 0;
      this.TotalImportValue = data.results[0].importMoney ?? 0;
      this.NetValue = this.TotalImportValue - this.TotalExportValue;
    });
  }

  GetAllAccountsImportMonyFilters() {
    this.taskService.GetAllAccountsImportMonyFilters(this.PagingFilter).subscribe(data => {
      this.filterList = data.results.filter(i => i.categoryName != 'SearchText');
    });
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllImportExportMonyStatistics();
  }

}
