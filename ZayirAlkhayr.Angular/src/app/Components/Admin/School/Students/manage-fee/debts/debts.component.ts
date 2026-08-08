import { Component, ViewChild, ViewContainerRef } from '@angular/core';
import { QzPrintService } from '../../../../../../Services/shared/qz-print.service';
import { StudentPaymentReceiptComponent } from "../../../../Printers/student-payment-receipt/student-payment-receipt.component";
import { SchoolPaymentReceiptModel } from '../../../../../../Models/school/payment/SchoolPaymentReceiptModel';

@Component({
  selector: 'app-debts',
  standalone: true,
  imports: [StudentPaymentReceiptComponent],
  templateUrl: './debts.component.html',
  styleUrl: './debts.component.css'
})
export class DebtsComponent {
  @ViewChild('host', { read: ViewContainerRef }) host!: ViewContainerRef;
  receiptData: SchoolPaymentReceiptModel = {
    studentName: 'محمد أحمد علي',
    academicStage: 'ثالث اعدادي',
    studentCode: 'STD-2026-00125',
    parentName: 'أحمد علي حسن',
    parentPhone: '01012345678',
    studentReceipt: {
      receiptNumber: 'REC-000154',
      receiptDate: '06/08/2026',
      receiptTime: '02:35 PM',
      paymentMethod: 'نقداً',
      paymentType: 'رسوم دراسية',
      paymentStatus: 'مدفوع'
    },
    studentPayment: [
      {
        feeName: 'القسط الأول',
        totalAmount: '5000',
        paidAmount: '5000',
        remainingAmount: '0',
        feeIcon: 'icon1.png'
      },
      {
        feeName: 'رسوم الكتب',
        totalAmount: '1200',
        paidAmount: '1200',
        remainingAmount: '0',
        feeIcon: 'icon2.png'
      },
      {
        feeName: 'رسوم الباص',
        totalAmount: '3000',
        paidAmount: '1500',
        remainingAmount: '1500',
        feeIcon: 'icon1.png'
      },
      {
        feeName: 'رسوم الأنشطة',
        totalAmount: '800',
        paidAmount: '800',
        remainingAmount: '0',
        feeIcon: 'icon1.png'
      }
    ],
    totalAmount: '10000',
    totalPaid: '8500',
    totalRemaining: '1500',
    totalPaidTxt: 'ثمانية آلاف وخمسمائة جنيه مصري فقط لا غير'
  };

  constructor(private printService: QzPrintService) { }

  async Print() {
    const componentRef =this.host.createComponent(StudentPaymentReceiptComponent);
    componentRef.instance.Data = this.receiptData;
    componentRef.changeDetectorRef.detectChanges();
    await new Promise<void>(resolve => {
      requestAnimationFrame(() => {
        requestAnimationFrame(() => resolve());
      });
    });

    const element = componentRef.instance.printArea.nativeElement;
    try {
      await this.printService.Print(element);
    } finally {
      componentRef.destroy();
    }
  }


}
