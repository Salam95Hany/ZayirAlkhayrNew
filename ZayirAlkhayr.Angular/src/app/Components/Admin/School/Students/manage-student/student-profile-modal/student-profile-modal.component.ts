import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { NgxLoadingModule } from 'ngx-loading';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';

interface ProfileField {
  label: string;
  value: string | number | null | undefined;
  ltr?: boolean;
  icon?: string;
}

interface FeeSummary {
  total: number;
  paid: number;
  remaining: number;
}

@Component({
  selector: 'app-student-profile-modal',
  standalone: true,
  imports: [CommonModule, NgxLoadingModule],
  templateUrl: './student-profile-modal.component.html',
  styleUrl: './student-profile-modal.component.css'
})
export class StudentProfileModalComponent implements OnInit {
  @Input() studentId!: number;
  @Input() parentId!: number;
  @Input() listItem: any;

  showLoader = false;
  loadError = false;
  profile: any = {};
  student: any = {};
  parent: any = {};
  enrollment: any = {};
  lookups: any = {};
  feeRecords: any[] = [];

  constructor(private schoolService: SchoolStudentService) { }

  ngOnInit(): void {
    if (!this.studentId || !this.parentId) {
      this.bindProfile(this.listItem || {});
      return;
    }

    this.showLoader = true;
    this.schoolService.GetUpdateStudentLookups(this.studentId, this.parentId).subscribe({
      next: data => {
        this.bindProfile(data?.results || {});
        this.loadFees();
      },
      error: () => {
        this.bindProfile(this.listItem || {});
        this.loadError = true;
        this.showLoader = false;
      }
    });
  }

  private bindProfile(data: any): void {
    this.profile = data || {};
    this.student = data?.student || data || {};
    this.parent = data?.parent || {};
    this.enrollment = this.student?.studentEnrollments?.[0] || data?.enrollment || {};
    this.lookups = data?.lookups || {};
  }

  get studentName(): string {
    return this.student?.studentName || this.listItem?.studentName || 'بيانات الطالب';
  }

  get studentInitial(): string {
    return this.studentName.trim().charAt(0) || 'ط';
  }

  get studentCode(): string {
    return this.student?.code || this.listItem?.code || `STU-${this.studentId || '---'}`;
  }

  get imageUrl(): string {
    return this.student?.imageUrl || this.student?.photoUrl || this.listItem?.imageUrl || '';
  }

  get statusName(): string {
    return this.enrollment?.studentStatus?.name || this.enrollment?.studentStatusName || this.listItem?.studentStatusName || 'منتظم';
  }

  get academicStageName(): string {
    return this.lookupName('academicStages', this.enrollment?.academicStageId) ||
      this.enrollment?.academicStageName || this.listItem?.academicStageName || '-';
  }

  get academicYearName(): string {
    return this.enrollment?.academicYear?.name || this.enrollment?.academicYearName ||
      this.listItem?.academicYearName || this.lookups?.currentYear?.name || '-';
  }

  get studyPeriodName(): string {
    const periods: Record<number, string> = { 1: 'صباحي', 2: 'مسائي' };
    return this.enrollment?.studyPeriodName || periods[+this.enrollment?.studyPeriodId] || this.listItem?.studyPeriodName || '-';
  }

  get basicFields(): ProfileField[] {
    return [
      { label: 'الاسم الكامل', value: this.studentName },
      { label: 'الجنس', value: this.genderName },
      { label: 'تاريخ الميلاد', value: this.formatDate(this.student?.birthDay || this.listItem?.birthDay), ltr: true },
      { label: 'الجنسية', value: this.lookupName('nationalities', this.student?.nationalityId) || this.listItem?.nationalityName },
      { label: 'نوع الطالب', value: this.lookupName('studentTypes', this.student?.studentTypeId) || this.listItem?.studentTypeName },
      { label: 'الترتيب بين الأبناء', value: this.student?.orderAmongChildren },
      { label: 'الحالة الصحية', value: this.student?.isHaveHealthCondition ? (this.student?.healthConditionNote || 'توجد حالة صحية') : 'لا توجد حالة صحية' }
    ];
  }

  get parentFields(): ProfileField[] {
    return [
      { label: 'الاسم', value: this.parent?.name || this.parent?.parentName || this.listItem?.parentName },
      { label: 'رقم الهاتف', value: this.parent?.parentPhone || this.parent?.fatherPhone || this.listItem?.parentPhone, ltr: true, icon: 'fa-solid fa-phone' },
      { label: 'واتساب', value: this.parent?.whatsappNumber, ltr: true, icon: 'fa-brands fa-whatsapp' },
      { label: 'البريد الإلكتروني', value: this.parent?.email, ltr: true, icon: 'fa-regular fa-envelope' },
      { label: 'صلة القرابة', value: this.parent?.relationName || 'ولي الأمر' },
      { label: 'المهنة', value: this.parent?.jobName || this.parent?.job }
    ];
  }

  get contactFields(): ProfileField[] {
    return [
      { label: 'العنوان الكامل', value: this.parent?.address || this.student?.address || this.listItem?.address },
      { label: 'هاتف الطالب', value: this.student?.phoneNumber || this.student?.phone, ltr: true, icon: 'fa-solid fa-phone' },
      { label: 'البريد الإلكتروني', value: this.student?.email, ltr: true, icon: 'fa-regular fa-envelope' },
      { label: 'المدرسة السابقة', value: this.student?.governmentSchool }
    ];
  }

  get academicFields(): ProfileField[] {
    return [
      { label: 'العام الدراسي', value: this.academicYearName },
      { label: 'المرحلة / الصف', value: this.academicStageName },
      { label: 'الفترة الدراسية', value: this.studyPeriodName },
      { label: 'تاريخ القيد', value: this.formatDate(this.enrollment?.enrollmentDate), ltr: true },
      { label: 'الحالة', value: this.statusName },
      { label: 'رقم الجلوس', value: this.enrollment?.seatNumber || this.student?.seatNumber }
    ];
  }

  get feeSummary(): FeeSummary {
    const fees = this.feeRecords.length ? this.feeRecords : (this.enrollment?.studentFees || this.enrollment?.fees || []);
    const total = this.toNumber(this.enrollment?.totalFees) || fees.reduce((sum: number, fee: any) => sum + this.toNumber(fee?.amount), 0);
    const normalizedTotal = total || fees.reduce((sum: number, fee: any) => sum + this.toNumber(fee?.netAmount ?? fee?.totalAmount), 0);
    const paid = this.toNumber(this.enrollment?.paidAmount) || fees.reduce((sum: number, fee: any) => sum + this.toNumber(fee?.paidAmount), 0);
    return { total: normalizedTotal, paid, remaining: Math.max(normalizedTotal - paid, 0) };
  }

  get paymentDetails(): any[] {
    const fees = this.feeRecords.length ? this.feeRecords : (this.enrollment?.studentFees || this.enrollment?.fees || []);
    return fees.map((fee: any, index: number) => {
      const payments = fee?.payments || [];
      const latestPayment = payments.length ? payments[payments.length - 1] : null;
      const amount = this.toNumber(fee?.netAmount ?? fee?.totalAmount ?? fee?.amount);
      const paidAmount = this.toNumber(fee?.paidAmount) || payments.reduce((sum: number, payment: any) => sum + (payment?.isCancelled ? 0 : this.toNumber(payment?.amount)), 0);
      return {
        index: index + 1,
        feeName: fee?.feeTypeName || fee?.feeName || fee?.name || '-',
        amount,
        dueDate: fee?.dueDate || fee?.nextInstallmentDate,
        paymentDate: latestPayment?.paymentDate || fee?.paymentDate,
        paidAmount,
        remainingAmount: this.toNumber(fee?.remainingAmount),
        paymentMethod: latestPayment?.paymentMethodName || this.paymentMethodName(latestPayment?.paymentMethod ?? fee?.paymentMethod),
        receiptNumber: latestPayment?.receiptNumber || fee?.receiptNumber,
        isPaid: +fee?.statusId === 3 || (amount > 0 && paidAmount >= amount)
      };
    });
  }

  get notes(): string {
    return this.enrollment?.notes || this.student?.notes || 'لا توجد ملاحظات مسجلة على ملف الطالب.';
  }

  get genderName(): string {
    const gender = +this.student?.gender;
    return this.student?.genderName || this.listItem?.gender || (gender === 1 ? 'ذكر' : gender === 2 ? 'أنثى' : '-');
  }

  displayValue(value: unknown): string | number {
    return value === null || value === undefined || value === '' ? '-' : value as string | number;
  }

  formatMoney(value: number): string {
    return new Intl.NumberFormat('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value || 0);
  }

  // requestEdit(): void {
  //   this.activeModal.close({ action: 'edit' });
  // }

  printProfile(): void {
    window.print();
  }

  private loadFees(): void {
    const enrollmentId = this.enrollment?.id || this.enrollment?.enrollmentId;
    if (!enrollmentId) {
      this.showLoader = false;
      return;
    }

    this.schoolService.GetAllStudentFeesByEnrollmentId(enrollmentId).subscribe({
      next: data => {
        const fees = data?.results?.fees || data?.results;
        this.feeRecords = Array.isArray(fees) ? fees : [];
        this.showLoader = false;
      },
      error: () => {
        this.showLoader = false;
      }
    });
  }

  paymentMethodName(method: unknown): string {
    const methods: Record<number, string> = { 1: 'نقدي', 2: 'إنستاباي', 3: 'فودافون كاش' };
    return methods[+method!] || '-';
  }

  private lookupName(collection: string, id: unknown): string {
    if (id === null || id === undefined) return '';
    return this.lookups?.[collection]?.find((item: any) => +item.value === +id)?.name || '';
  }

  private formatDate(value: unknown): string {
    if (!value) return '-';
    const date = new Date(value as string);
    return Number.isNaN(date.getTime()) ? String(value) : new Intl.DateTimeFormat('en-GB').format(date);
  }

  private toNumber(value: unknown): number {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
  }
}
