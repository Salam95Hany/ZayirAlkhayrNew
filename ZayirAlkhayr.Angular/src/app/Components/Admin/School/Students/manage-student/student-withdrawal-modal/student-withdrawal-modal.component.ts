import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { StudentWithdrawal, WithdrawalPayment, settlementStatuses } from '../../../../../../Models/school/student/StudentWithdrawal';

@Component({
  selector: 'app-student-withdrawal-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, RoleCheckerDirective],
  templateUrl: './student-withdrawal-modal.component.html',
  styleUrl: './student-withdrawal-modal.component.css'
})
export class StudentWithdrawalModalComponent implements OnInit, OnDestroy {
  @Input() enrollmentId: number;
  @Input() previewMode = false;
  readonly months = Array.from({ length: 11 }, (_, index) => 1 + index / 2);
  readonly statuses = settlementStatuses;
  readonly methods: Record<number, string> = { 1: 'نقدًا', 2: 'إنستاباي', 3: 'فودافون كاش' };
  readonly feeStatuses: Record<number, string> = { 1: 'غير مدفوع', 2: 'مدفوع جزئيًا', 3: 'مدفوع', 4: 'ملغى' };
  originalTotal = 0;
  discountTotal = 0;
  selectedMonths = 1;
  method: number | null = null;
  data: StudentWithdrawal | null = null;
  payments: WithdrawalPayment[] = [];
  loading = false;
  saving = false;
  error = '';
  changed = false;
  private readRequest?: Subscription;

  constructor(public modal: NgbActiveModal, private service: SchoolStudentService, private toast: ToastrService) {}

  ngOnInit() { this.load(); }
  ngOnDestroy() { this.readRequest?.unsubscribe(); }

  load(months = this.selectedMonths) {
    this.readRequest?.unsubscribe();
    this.selectedMonths = months;
    this.loading = true;
    this.error = '';
    const request = this.previewMode ? this.service.PreviewWithdrawal(this.enrollmentId, months)
      : this.service.GetWithdrawalDetails(this.enrollmentId);
    this.readRequest = request.subscribe({
      next: result => {
        this.loading = false;
        if (result.isSuccess && result.results) this.show(result.results);
        else this.readFailed(result.message);
      },
      error: () => this.readFailed('تعذر تحميل بيانات الانسحاب.')
    });
  }

  private readFailed(message: string) {
    this.loading = false;
    this.data = null;
    this.error = message;
  }

  private show(data: StudentWithdrawal) {
    this.data = data;
    const activeFees = data.fees.filter(fee => fee.originalStatus !== 4);
    this.originalTotal = activeFees.reduce((sum, fee) => sum + fee.original, 0);
    this.discountTotal = activeFees.reduce((sum, fee) => sum + fee.discount, 0);
    this.payments = data.fees.flatMap(fee => fee.payments)
      .sort((a, b) => b.paymentDate.localeCompare(a.paymentDate) || b.id - a.id);
  }

  confirm() {
    if (!this.data || !this.previewMode || this.saving || this.loading) return;
    this.saving = true;
    this.service.ConfirmWithdrawal(this.data).subscribe({
      next: result => {
        this.saving = false;
        if (result.isSuccess) {
          this.changed = true;
          this.previewMode = false;
          this.show(result.results);
          this.toast.success('تم تأكيد انسحاب الطالب.');
        } else {
          this.toast.error(result.message);
          this.load();
        }
      },
      error: () => {
        this.saving = false;
        this.changed = true;
        this.toast.error('تعذر التحقق من الحفظ. حدّث بيانات الطالب قبل إعادة التأكيد.');
        this.modal.close(true);
      }
    });
  }

  settle() {
    if (this.previewMode || this.data?.student.status !== 1 || !this.method || this.saving || this.loading) return;
    this.saving = true;
    this.service.PayWithdrawalSettlement(this.enrollmentId, this.method).subscribe({
      next: result => {
        this.saving = false;
        this.changed = true;
        if (result.isSuccess) this.toast.success('تم تسجيل سداد كامل المبلغ.');
        else this.toast.error(result.message);
        this.load();
      },
      error: () => {
        this.saving = false;
        this.changed = true;
        this.toast.error('تعذر التحقق من السداد. جارٍ تحديث التسوية.');
        this.load();
      }
    });
  }
}
