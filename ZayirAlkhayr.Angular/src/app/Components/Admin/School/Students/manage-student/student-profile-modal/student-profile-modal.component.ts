import { CommonModule, DatePipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { NgxLoadingModule } from 'ngx-loading';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ArabicDateWithTimePipe } from '../../../../../../Pipes/arabic-date-with-time.pipe';
import { SearchReportModel } from '../../../../../../Models/shared/SearchReportModel';
import { PdfDownloadService } from '../../../../../../Services/shared/pdf-download.service';

@Component({
  selector: 'app-student-profile-modal',
  standalone: true,
  imports: [CommonModule, NgxLoadingModule],
  templateUrl: './student-profile-modal.component.html',
  styleUrl: './student-profile-modal.component.css',
  providers: [ArabicDateWithTimePipe, DatePipe]
})
export class StudentProfileModalComponent implements OnInit {
  @Input() studentId: number;
  @Input() parentId: number;
  showLoader = false;
  loadError = false;
  StudentData: any;
  PaymentData: any[] = [];
  SearchReport: SearchReportModel = {
    reportType: 'StudentProfilePdf',
    headers: [],
    filterItems: [],
    queryString: []
  };

  constructor(private schoolService: SchoolStudentService, private modalService: NgbModal, private arabicDatePipe: ArabicDateWithTimePipe, private datePipe: DatePipe,
    private pdfService: PdfDownloadService
  ) { }

  ngOnInit(): void {
    this.GetStudentHistoryById();
  }

  GetStudentHistoryById() {
    if (!this.studentId) {
      return;
    }

    this.showLoader = true;
    this.schoolService.GetStudentHistoryById(this.studentId).subscribe({
      next: data => {
        this.showLoader = false;
        this.StudentData = data?.results || {};
        this.CreatePaymentDetailsArray();
      },
      error: () => {
        this.showLoader = false;
      }
    });
  }

  get studentName(): string {
    return this.StudentData?.studentName || 'بيانات الطالب';
  }

  get studentInitial(): string {
    return this.studentName.trim().charAt(0) || 'ط';
  }

  get basicFields() {
    return [
      { label: 'الاسم الكامل:', value: this.StudentData?.studentName },
      { label: 'الجنس:', value: this.StudentData?.gender },
      { label: 'تاريخ الميلاد:', value: this.arabicDatePipe.transform(this.StudentData?.birthDay) },
      { label: 'الجنسية:', value: this.StudentData?.nationality },
      { label: 'نوع الطالب:', value: this.StudentData?.studentType },
      { label: 'الترتيب بين أخوته:', value: this.StudentData?.orderAmongChildren },
      { label: 'الحالة الصحية:', value: this.StudentData?.isHaveHealthCondition ? (this.StudentData?.healthConditionNote || 'توجد حالة صحية') : 'لا توجد حالة صحية' }
    ];
  }

  get parentFields() {
    return [
      { label: 'الاسم:', value: this.StudentData?.parentName },
      { label: 'رقم الهاتف:', value: this.StudentData?.parentPhone, ltr: true, icon: 'fa-solid fa-phone' },
      { label: 'واتساب:', value: this.StudentData?.parentWhatsappNumber, ltr: true, icon: 'fa-brands fa-whatsapp' },
      { label: 'صلة القرابة:', value: this.StudentData?.phoneRelationship || 'ولي الأمر' },
      { label: 'عدد الأبناء:', value: this.StudentData?.brotherCount }
    ];
  }

  get contactFields() {
    return [
      { label: 'العنوان الكامل:', value: this.StudentData?.address },
      { label: 'المدرسة الحكومية:', value: this.StudentData?.governmentSchool }
    ];
  }

  get academicFields() {
    return [
      { label: 'العام الدراسي:', value: this.StudentData?.academicYear },
      { label: 'المرحلة / الصف:', value: this.StudentData?.academicStage },
      { label: 'الفترة الدراسية:', value: this.StudentData?.studyPeriodName },
      { label: 'تاريخ التسجيل:', value: this.arabicDatePipe.transform(this.StudentData?.enrollmentDate), ltr: true },
      { label: 'الحالة:', value: this.StudentData?.studentStatusName },
    ];
  }

  get feeSummary() {
    debugger;
    const fees = this.StudentData?.fees?.length ? this.StudentData?.fees : [];
    const total = fees.reduce((sum: number, fee: any) => sum + Number(fee?.totalAmount), 0) ?? 0;
    const normalizedTotal = total || fees.reduce((sum: number, fee: any) => sum + Number(fee?.netAmount), 0);
    const paid = fees.reduce((sum: number, fee: any) => sum + Number(fee?.paidAmount), 0);
    return { total: normalizedTotal, paid, remaining: Math.max(normalizedTotal - paid, 0) };
  }

  CreatePaymentDetailsArray() {
    this.PaymentData = [];
    const fees = this.StudentData.fees.length ? this.StudentData.fees : [];
    fees.forEach((fee: any, index: number) => {
      const payments = fee?.payments || [];
      if (payments.length > 0)
        payments.forEach((payment: any) => {
          this.PaymentData.push({
            index: index + 1,
            feeName: fee?.feeName || '-',
            amount: payment?.amount || 0,
            nextInstallmentDate: this.arabicDatePipe.transform(payment?.nextInstallmentDate),
            paymentDate: this.arabicDatePipe.transform(payment?.paymentDate),
            paymentMethod: payment?.paymentMethod,
            receiptNumber: payment?.receiptNumber,
          });
        });
    });
  }

  CloseModal(): void {
    this.modalService.dismissAll();
  }

  DownloadPdfProfile() {
    this.SearchReport.queryString = [
      { key: 'StudentId', value: this.studentId.toString() }
    ];
    let today = this.datePipe.transform(new Date(), 'dd-MM-yyyy');
    let fileName = 'ملف الطالب_' + this.StudentData?.studentName + '_' + today;
    this.showLoader = true;
    this.pdfService.DownloadFile(this.SearchReport, fileName + '.pdf').subscribe(data => {
      this.showLoader = false;
    });
  }
}
