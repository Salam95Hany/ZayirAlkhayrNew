import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { SchoolPaymentReceiptModel } from '../../../../Models/school/payment/SchoolPaymentReceiptModel';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-student-payment-receipt',
  standalone: true,
  imports: [NgClass],
  templateUrl: './student-payment-receipt.component.html',
  styleUrl: './student-payment-receipt.component.css'
})
export class StudentPaymentReceiptComponent {
  @ViewChild('printArea', { static: true }) printArea!: ElementRef<HTMLElement>;
  @Input() Data: SchoolPaymentReceiptModel;

  getFeeIcon(name: string): string {

    const value = name.toLowerCase();

    if (
        value.includes('كتاب') ||
        value.includes('كتب')
    ) {
        return 'fa-book-open';
    }

    if (
        value.includes('باص') ||
        value.includes('نقل')
    ) {
        return 'fa-bus';
    }

    if (
        value.includes('زي')
    ) {
        return 'fa-shirt';
    }

    if (
        value.includes('نشاط') ||
        value.includes('أنشطة')
    ) {
        return 'fa-futbol';
    }

    return 'fa-receipt';
}

}
