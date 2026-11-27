import { CommonModule } from '@angular/common';
import { Component, DestroyRef, ElementRef, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { firstValueFrom, forkJoin, Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../../Shared/za-pagination/za-pagination.component';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { AnalysisParams, FinancialBalances, FinancialMovement, FinancialSummary } from '../../../../../../Models/school/FinancialAnalysis';
import { SchoolAccountService } from '../../../../../../Services/school/school-account.service';
import { StudentSettingService } from '../../../../../../Services/school/student-setting.service';
import { StudentWithdrawalModalComponent } from '../../manage-student/student-withdrawal-modal/student-withdrawal-modal.component';
import { settlementStatuses } from '../../../../../../Models/school/student/StudentWithdrawal';
import { AuthService } from '../../../../../../Auth/auth.service';
import { PosPrinterService } from '../../../../../../Services/school/pos-printer.service';

type Tab = 'ledger' | 'fees' | 'settlements';
@Component({
  selector: 'app-financial-analysis',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, AdminBreadcrumbComponent, ZaPaginationComponent, RoleCheckerDirective],
  templateUrl: './financial-analysis.component.html',
  styleUrl: './financial-analysis.component.css'
})
export class FinancialAnalysisComponent implements OnInit, OnDestroy {
  @ViewChild('detailPanel') detailPanel: ElementRef<HTMLElement>;
  readonly titles = ['مركز بشائر القرآن', 'إدارة الحسابات', 'التحليل المالي'];
  readonly pageKey = 'School_ManageAccounts_FinancialAnalysis';
  readonly methods = ['غير محددة', 'نقدًا', 'إنستاباي', 'فودافون كاش'];
  readonly types = ['', 'دفعة رسوم', 'تحصيل تسوية', 'مصروف', 'رد مبلغ'];
  readonly groups = ['كل الحركات', 'المقبوضات', 'المصروفات', 'المبالغ المردودة'];
  readonly statuses = settlementStatuses;
  readonly tabs: {key: Tab; label: string}[] = [{key:'ledger',label:'كشف الحركة'}, {key:'fees',label:'الرسوم والمديونيات'}, {key:'settlements',label:'تسويات الانسحاب'}];
  readonly pageSize = 25;
  readonly destroyRef = inject(DestroyRef);
  period = 'month';
  from = ''; to = '';
  year = ''; stage = '';
  years: {id: number; name: string; isCurrent: boolean}[] = [];
  stages: {value: string; name: string}[] = [];
  private scope: AnalysisParams = {};
  private dates: AnalysisParams = {};
  tab: Tab = 'ledger';
  summary?: FinancialSummary;
  balances?: FinancialBalances;
  movements: FinancialMovement[] = [];
  current?: { fees: number; school: number; students: number };
  maxBar = 1;
  group = 0;
  movementType = 0;
  method = 0;
  expenseType: number | null = null;
  feeType: number | null = null;
  payer = '';
  status = 0;
  cancelled = false;
  outstanding = false;
  search = '';
  private appliedSearch = '';
  page = 1; total = 0;
  busy = false; exporting = false;
  error = ''; summaryError = ''; balanceError = '';
  private summaryRequest?: Subscription;
  private currentRequest?: Subscription;
  private rowsRequest?: Subscription;

  constructor(private service: SchoolAccountService, private settings: StudentSettingService, private modal: NgbModal,
    private toast: ToastrService, public auth: AuthService, private printer: PosPrinterService) {}

  ngOnInit() {
    this.settings.GetAllAcademicYearData({currentPage:1,pageSize:10000,filterList:[]}).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: r => { if (r.isSuccess) { this.years = r.results; this.year = String(this.years.find(y => y.isCurrent)?.id ?? this.years[0]?.id ?? ''); } else this.toast.error(r.message); },
      error: () => this.toast.error('تعذر تحميل السنوات الدراسية.')
    });
    this.settings.GetAcademicStages().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: r => this.stages = r, error: () => this.toast.error('تعذر تحميل المراحل الدراسية.')
    });
    this.apply();
  }

  ngOnDestroy() {
    this.summaryRequest?.unsubscribe();
    this.currentRequest?.unsubscribe();
    this.rowsRequest?.unsubscribe();
  }

  apply() {
    if (this.period === 'custom' && (!this.from || !this.to || this.to < this.from)) {
      this.toast.warning('اختر فترة صحيحة من تاريخ إلى تاريخ.'); return;
    }
    if (this.period === 'year' && !this.year) { this.toast.warning('اختر السنة الدراسية.'); return; }
    this.scope = this.stage ? {academicStageId: this.stage} : {};
    this.dates = {};
    if (this.period === 'year') this.scope['academicYearId'] = this.year;
    if (this.period === 'today') {
      const today = new Intl.DateTimeFormat('en-CA', {timeZone:'Africa/Cairo',year:'numeric',month:'2-digit',day:'2-digit'}).format(new Date());
      this.dates = {from:today,to:today};
    }
    if (this.period === 'custom') this.dates = {from:this.from,to:this.to};
    this.refresh();
  }

  refresh() {
    this.summaryRequest?.unsubscribe();
    this.summary = undefined; this.summaryError = '';
    this.summaryRequest = this.service.GetFinancialSummary({...this.scope,...this.dates}).subscribe({
      next: r => {
        if (!r.isSuccess) { this.summaryError = r.message; return; }
        this.summary = r.results;
        this.maxBar = this.summary.monthly.reduce((max,m) => Math.max(max,m.totals.receipts,m.totals.expenses+m.totals.refunds),1);
      }, error: () => this.summaryError = 'تعذر تحميل ملخص الحركة المالية.'
    });
    this.currentRequest?.unsubscribe();
    this.current = undefined; this.balanceError = '';
    const params = {...this.scope,outstandingOnly:true,pageSize:1};
    this.currentRequest = forkJoin([this.service.GetFinancialBalances({...params,group:0}),this.service.GetFinancialBalances({...params,group:1})]).subscribe({
      next: ([fees, settlements]) => {
        if (!fees.isSuccess || !settlements.isSuccess) { this.balanceError = 'تعذر تحميل الأرصدة الحالية.'; return; }
        const pending = settlements.results.bySettlementStatus.find(s => s.status === 1);
        this.current = { fees:fees.results.byFeeType.reduce((n,f)=>n+f.remaining,0), school:pending?.schoolOwes ?? 0, students:pending?.studentOwes ?? 0 };
      }, error: () => this.balanceError = 'تعذر تحميل الأرصدة الحالية.'
    });
    this.switchTab(this.tab);
  }

  private reset(tab: Tab) {
    this.tab = tab; this.search = ''; this.appliedSearch = ''; this.group = 0; this.movementType = 0; this.method = 0; this.expenseType = null;
    this.feeType = null; this.payer = ''; this.status = 0; this.cancelled = false; this.outstanding = false;
  }
  switchTab(tab: Tab) { this.reset(tab); this.load(); }
  ledger(group: number, method = 0, expense: number | null = null) {
    this.reset('ledger'); this.group = group; this.method = method; this.expenseType = expense;
    this.load(); this.detailPanel.nativeElement.scrollIntoView({block:'start'});
  }
  debt(tab: Tab, payer = '') {
    this.reset(tab); this.outstanding = true; this.payer = payer;
    this.load(); this.detailPanel.nativeElement.scrollIntoView({block:'start'});
  }

  private ledgerParams(): AnalysisParams {
    const p: AnalysisParams = {...this.scope,...this.dates,group:this.group,cancelledOnly:this.cancelled,search:this.appliedSearch};
    if (this.movementType) p['type'] = this.movementType;
    if (this.method) p['method'] = this.method;
    if (this.group === 2 && this.expenseType != null) p['expenseTypeId'] = this.expenseType;
    return p;
  }

  find() { this.appliedSearch = this.search.trim(); this.load(); }

  load(page = 1) {
    this.rowsRequest?.unsubscribe();
    this.page = page; this.busy = true; this.error = ''; this.total = 0;
    this.movements = []; this.balances = undefined;
    const onError = () => { this.busy = false; this.error = 'تعذر تحميل التفاصيل.'; };
    const paging = {page,pageSize:this.pageSize};
    if (this.tab === 'ledger') {
      this.rowsRequest = this.service.GetFinancialTransactions({...this.ledgerParams(),...paging}).subscribe({
        next: r => { this.busy = false; if (r.isSuccess) { this.movements = r.results; this.total = r.totalCount; } else this.error = r.message; }, error:onError
      });
    } else {
      const p: AnalysisParams = {...this.scope,...paging,group:this.tab === 'fees'?0:1,outstandingOnly:this.outstanding,search:this.appliedSearch};
      if (this.tab === 'fees' && this.feeType != null) p['feeTypeId'] = this.feeType;
      if (this.tab === 'settlements' && this.payer) p['schoolOwes'] = this.payer === 'school';
      if (this.tab === 'settlements' && this.status) p['status'] = this.status;
      this.rowsRequest = this.service.GetFinancialBalances(p).subscribe({
        next: r => { this.busy = false; if (r.isSuccess) { this.balances = r.results; this.total = r.totalCount; } else this.error = r.message; }, error:onError
      });
    }
  }

  openSettlement(enrollmentId: number) {
    const ref = this.modal.open(StudentWithdrawalModalComponent, {size:'xl',centered:true,scrollable:true,backdrop:'static',keyboard:false,ariaLabelledBy:'student-withdrawal-title'});
    ref.componentInstance.enrollmentId = enrollmentId;
    ref.closed.subscribe(changed => { if (changed) this.refresh(); });
  }
  receipt(row: FinancialMovement) {
    this.printer.GetStudentReceiptData(row.enrollmentId,row.id,this.auth.userNameAr).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: file => this.printer.downloadPdf(file,'إيصال-' + (row.reference || row.id) + '.pdf'),
      error: () => this.toast.error('تعذر تحميل الإيصال.')
    });
  }

  async exportLedger() {
    // if (this.exporting || !this.total || this.tab !== 'ledger') return;
    // this.exporting = true;
    // const params = this.ledgerParams();
    // try {
    //   const {default:writeExcel} = await import('write-excel-file');
    //   const headers = ['التاريخ','الحركة','الاسم أو البند','كود الطالب','المرجع','طريقة الدفع','المبلغ','وارد','صادر','الحالة'];
    //   const rows: (string | number)[][] = [];
    //   const seen = new Set<string>();
    //   let page = 1;
    //   let expected: number;
    //   do {
    //     const r = await firstValueFrom(this.service.GetFinancialTransactions({...params,page,pageSize:200}).pipe(takeUntilDestroyed(this.destroyRef)));
    //     if (!r.isSuccess) throw new Error(r.message);
    //     expected ??= r.totalCount;
    //     if (expected !== r.totalCount || (!r.results.length && rows.length < expected)) throw new Error('تغير الكشف أثناء التصدير. أعد المحاولة.');
    //     for (const m of r.results) {
    //       const key = m.type + ':' + m.id;
    //       if (seen.has(key)) throw new Error('تغير الكشف أثناء التصدير. أعد المحاولة.');
    //       seen.add(key);
    //       rows.push([m.date.slice(0,10),this.types[m.type],m.name,m.studentCode || '',m.reference || String(m.id),
    //       this.methods[m.method ?? 0],m.amount,m.incoming,m.outgoing,m.isCancelled?'ملغاة':'مسجلة']);
    //     }
    //     page++;
    //   } while (rows.length < expected);
    //   await writeExcel([headers.map(value=>({value,fontWeight:'bold' as const})),...rows.map(row=>row.map(value=>
    //     typeof value === 'number' ? {type:Number,value,format:'#,##0.00'} : {type:String,value}))],
    //     {sheet:'كشف الحركة',fileName:'كشف الحركة المالية.xlsx',columns:headers.map(()=>({width:20})),stickyRowsCount:1});
    // } catch (e) { this.toast.error(e instanceof Error && /[ء-ي]/.test(e.message) ? e.message : 'تعذر تصدير الكشف.'); }
    // finally { this.exporting = false; }
  }
}
