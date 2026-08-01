import { Component, OnInit } from '@angular/core';
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { AdminBreadcrumbComponent } from '../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { CommonModule } from '@angular/common';
import { FilterModel } from '../../../../../Models/shared/FilterModel';

@Component({
  selector: 'app-net-value',
  standalone: true,
  imports: [ZaFiltersComponent, AdminBreadcrumbComponent, CommonModule],
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
    this.GetFinancialTransactionStatisticsNetValue();
    this.GetFinancialTransactionStatisticFilter();

  }

  GetFinancialTransactionStatisticsNetValue() {
    this.taskService.GetFinancialTransactionStatisticsNetValue(this.PagingFilter).subscribe(data => {
      this.TotalExportValue = data.results.totalExpenses ?? 0;
      this.TotalImportValue = data.results.totalIncome ?? 0;
      this.NetValue = data.results.netValue ?? 0;
    });
  }

  GetFinancialTransactionStatisticFilter() {
    this.taskService.GetFinancialTransactionStatisticFilter().subscribe(data => {
      this.filterList = data.results
    });
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetFinancialTransactionStatisticsNetValue();
  }

}
