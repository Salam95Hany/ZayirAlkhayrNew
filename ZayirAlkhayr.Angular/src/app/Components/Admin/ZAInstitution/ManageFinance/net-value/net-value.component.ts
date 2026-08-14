import { AfterViewInit, Component, DestroyRef, ElementRef, NgZone, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { AdminBreadcrumbComponent } from '../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { TaskService } from '../../../../../Services/zainstitution/task.service';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { CommonModule } from '@angular/common';
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { NgxLoadingModule } from 'ngx-loading';
import {
  ArcElement,
  BarController,
  BarElement,
  CategoryScale,
  Chart,
  ChartConfiguration,
  DoughnutController,
  Legend,
  LinearScale,
  LineController,
  LineElement,
  PointElement,
  Tooltip
} from 'chart.js';
import { ExpenseCategoryModel, FinancialTrendModel } from '../../../../../Models/zainstitution/FinancialDashboardModel';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';

Chart.register(
  ArcElement,
  BarController,
  BarElement,
  CategoryScale,
  DoughnutController,
  Legend,
  LinearScale,
  LineController,
  LineElement,
  PointElement,
  Tooltip
);

@Component({
  selector: 'app-net-value',
  standalone: true,
  imports: [ZaFiltersComponent, AdminBreadcrumbComponent, CommonModule, NgxLoadingModule],
  templateUrl: './net-value.component.html',
  styleUrl: './net-value.component.css'
})
export class NetValueComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('financialTrendChart') financialTrendChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('expenseCategoryChart') expenseCategoryChartRef!: ElementRef<HTMLCanvasElement>;

  TitleList = ['مؤسسة زائر الخير', 'إدارة الحسابات', 'الباقي'];
  filterList: FilterModel[] = [];
  TotalExportValue = 0;
  TotalImportValue = 0;
  NetValue = 0;
  TotalIncomePercentage = 0;
  TotalExpensesPercentage = 0;
  TotalNetValuePercentage = 0;
  selectedPercentageValue: string | null = null;
  showLoader = false;
  private viewInitialized = false;
  private chartsDataLoaded = false;
  private financialTrendData: FinancialTrendModel[] = [];
  private expenseCategoryData: ExpenseCategoryModel[] = [];
  private financialTrendChart?: Chart<'bar' | 'line'>;
  private expenseCategoryChart?: Chart<'doughnut'>;
  private chartRenderFrame?: number;
  private isDestroyed = false;
  private readonly destroyRef = inject(DestroyRef);
  PagingFilter: PagingFilterModel = {
    currentPage: 1,
    pageSize: 20,
    filterList: []
  };

  get expenseRatio(): number {
    if (!this.TotalImportValue)
      return this.TotalExportValue ? 100 : 0;

    return Math.min((this.TotalExportValue / this.TotalImportValue) * 100, 100);
  }

  get netRate(): number {
    if (!this.TotalImportValue)
      return 0;

    return (this.NetValue / this.TotalImportValue) * 100;
  }

  get activeFiltersCount(): number {
    return this.PagingFilter.filterList?.filter((filter: any) =>
      filter?.isChecked || filter?.checked || filter?.selected
    ).length ?? 0;
  }

  get hasSelectedPercentage(): boolean {
    return this.selectedPercentageValue !== null;
  }

  constructor(private taskService: TaskService, private ngZone: NgZone) {

  }

  ngOnInit(): void {
    this.GetFinancialTransactionStatisticsNetValue();
    this.GetFinancialTransactionStatisticFilter();
    this.GetFinancialNetValueChartsData();

  }

  ngAfterViewInit(): void {
    this.viewInitialized = true;
    this.scheduleChartRender();
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
    if (this.chartRenderFrame !== undefined)
      cancelAnimationFrame(this.chartRenderFrame);

    this.destroyCharts();
  }

  GetFinancialNetValueChartsData() {
    this.taskService.GetFinancialNetValueChartsData()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: data => {
          this.financialTrendData = this.normalizeFinancialTrendData(data.results?.table1);
          this.expenseCategoryData = this.normalizeExpenseCategoryData(data.results?.table);
          this.chartsDataLoaded = true;
          this.scheduleChartRender();
        },
        error: () => {
          this.financialTrendData = [];
          this.expenseCategoryData = [];
          this.chartsDataLoaded = true;
        }
      });
  }

  GetFinancialTransactionStatisticsNetValue() {
    this.showLoader = true;
    this.taskService.GetFinancialTransactionStatisticsNetValue(this.PagingFilter)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.showLoader = false)
      )
      .subscribe(data => {
        this.TotalExportValue = this.toFiniteNumber(data.results.totalExpenses);
        this.TotalImportValue = this.toFiniteNumber(data.results.totalIncome);
        this.NetValue = this.toFiniteNumber(data.results.netValue);
        this.TotalIncomePercentage = this.toFiniteNumber(data.results.totalIncomePercentage);
        this.TotalExpensesPercentage = this.toFiniteNumber(data.results.totalExpensesPercentage);
        this.TotalNetValuePercentage = this.toFiniteNumber(data.results.totalNetValuePercentage);
      });
  }

  GetFinancialTransactionStatisticFilter() {
    this.taskService.GetFinancialTransactionStatisticFilter()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(data => {
        this.filterList = data.results;
      });
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    const percentageFilter = this.findSelectedPercentageFilter(filterList);
    this.selectedPercentageValue = percentageFilter
      ? String(percentageFilter.itemKey ?? percentageFilter.itemId ?? '').replace('%', '').trim()
      : null;
    this.GetFinancialTransactionStatisticsNetValue();
  }

  private findSelectedPercentageFilter(filterList: FilterModel[] = []): FilterModel | undefined {
    for (const filter of filterList) {
      if (filter.categoryName?.toLowerCase() === 'percentage' && filter.isChecked) {
        return filter;
      }

      const selectedChild = this.findSelectedPercentageFilter(filter.filterItems ?? []);
      if (selectedChild) {
        return selectedChild;
      }
    }

    return undefined;
  }

  private scheduleChartRender(): void {
    if (!this.viewInitialized || !this.chartsDataLoaded || this.isDestroyed || this.chartRenderFrame !== undefined)
      return;

    this.ngZone.runOutsideAngular(() => {
      this.chartRenderFrame = requestAnimationFrame(() => {
        this.chartRenderFrame = undefined;
        if (this.isDestroyed)
          return;

        this.destroyCharts();
        this.createFinancialTrendChart();
        this.createExpenseCategoryChart();
      });
    });
  }

  private destroyCharts(): void {
    this.financialTrendChart?.destroy();
    this.expenseCategoryChart?.destroy();
    this.financialTrendChart = undefined;
    this.expenseCategoryChart = undefined;
  }

  private normalizeFinancialTrendData(data: FinancialTrendModel[] | null | undefined): FinancialTrendModel[] {
    if (!Array.isArray(data))
      return [];

    return data.map(item => ({
      month: String(item?.month ?? ''),
      revenue: this.toFiniteNumber(item?.revenue),
      expenses: this.toFiniteNumber(item?.expenses),
      balance: this.toFiniteNumber(item?.balance)
    }));
  }

  private normalizeExpenseCategoryData(data: ExpenseCategoryModel[] | null | undefined): ExpenseCategoryModel[] {
    if (!Array.isArray(data))
      return [];

    return data.map(item => ({
      category: String(item?.category ?? ''),
      amount: this.toFiniteNumber(item?.amount)
    }));
  }

  private toFiniteNumber(value: unknown): number {
    const numericValue = Number(value);
    return Number.isFinite(numericValue) ? numericValue : 0;
  }

  private createFinancialTrendChart(): void {
    const config: ChartConfiguration<'bar' | 'line'> = {
      type: 'bar',
      data: {
        labels: this.financialTrendData.map(item => item.month),
        datasets: [
          {
            type: 'bar',
            label: 'الإيرادات',
            data: this.financialTrendData.map(item => item.revenue),
            backgroundColor: '#1a9a78',
            borderRadius: 6,
            borderSkipped: false,
            maxBarThickness: 30
          },
          {
            type: 'bar',
            label: 'المصروفات',
            data: this.financialTrendData.map(item => item.expenses),
            backgroundColor: '#d85c68',
            borderRadius: 6,
            borderSkipped: false,
            maxBarThickness: 30
          },
          {
            type: 'line',
            label: 'صافي الرصيد',
            data: this.financialTrendData.map(item => item.balance),
            borderColor: '#24658f',
            backgroundColor: '#24658f',
            borderWidth: 2.5,
            pointBackgroundColor: '#ffffff',
            pointBorderColor: '#24658f',
            pointBorderWidth: 2,
            pointRadius: 4,
            pointHoverRadius: 6,
            tension: .35
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        animation: false,
        resizeDelay: 150,
        interaction: { mode: 'index', intersect: false },
        plugins: {
          legend: {
            position: 'bottom',
            rtl: true,
            textDirection: 'rtl',
            labels: { usePointStyle: true, boxWidth: 8, boxHeight: 8, padding: 18, color: '#52687a', font: { size: 11 } }
          },
          tooltip: {
            rtl: true,
            textDirection: 'rtl',
            titleAlign: 'right',
            bodyAlign: 'right',
            padding: 12,
            displayColors: true,
            callbacks: {
              label: context => `${context.dataset.label}: ${this.formatCurrency(Number(context.raw))}`
            }
          }
        },
        scales: {
          x: { grid: { display: false }, ticks: { color: '#718294', font: { size: 10 } }, border: { display: false } },
          y: {
            beginAtZero: true,
            grid: { color: '#edf1f4' },
            border: { display: false },
            ticks: { color: '#82909e', font: { size: 10 }, callback: value => this.formatCompactNumber(Number(value)) }
          }
        }
      }
    };

    this.financialTrendChart = new Chart(this.financialTrendChartRef.nativeElement, config);
  }

  private createExpenseCategoryChart(): void {
    const totalExpenses = this.expenseCategoryData.reduce((total, item) => total + item.amount, 0);
    const config: ChartConfiguration<'doughnut'> = {
      type: 'doughnut',
      data: {
        labels: this.expenseCategoryData.map(item => item.category),
        datasets: [{
          data: this.expenseCategoryData.map(item => item.amount),
          backgroundColor: ['#24658f', '#13a082', '#e5a52d', '#7a6bb2', '#d85c68', '#8b9aa7'],
          borderColor: '#ffffff',
          borderWidth: 3,
          hoverOffset: 7
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        animation: false,
        resizeDelay: 150,
        cutout: '66%',
        plugins: {
          legend: {
            position: 'bottom',
            rtl: true,
            textDirection: 'rtl',
            labels: { usePointStyle: true, boxWidth: 8, boxHeight: 8, padding: 13, color: '#52687a', font: { size: 10 } }
          },
          tooltip: {
            rtl: true,
            textDirection: 'rtl',
            titleAlign: 'right',
            bodyAlign: 'right',
            padding: 12,
            callbacks: {
              label: context => {
                const amount = Number(context.raw);
                const percentage = totalExpenses ? (amount / totalExpenses) * 100 : 0;
                return [`${this.formatCurrency(amount)}`, `${percentage.toFixed(1)}% من إجمالي المصروفات`];
              }
            }
          }
        }
      }
    };

    this.expenseCategoryChart = new Chart(this.expenseCategoryChartRef.nativeElement, config);
  }

  private formatCurrency(value: number): string {
    return `${new Intl.NumberFormat('en-US', { maximumFractionDigits: 0 }).format(value)} ج.م`;
  }

  private formatCompactNumber(value: number): string {
    return new Intl.NumberFormat('en-US', { notation: 'compact', maximumFractionDigits: 1 }).format(value);
  }

}
