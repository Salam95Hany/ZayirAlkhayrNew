export interface SchoolPaymentReceiptModel {
  studentName: string;
  academicStage: string;
  studentCode: string;
  parentName: string;
  parentPhone: string;
  studentReceipt: StudentReceiptModel;
  studentPayment: StudentPaymentModel[];
  totalAmount: string;
  totalPaid: string;
  totalRemaining: string;
  totalPaidTxt: string;
}

export interface StudentReceiptModel {
  receiptNumber: string;
  receiptDate: string;
  receiptTime: string;
  paymentMethod: string;
  paymentType: string;
  paymentStatus: string;
}

export interface StudentPaymentModel {
  feeName: string;
  totalAmount: string;
  paidAmount: string;
  remainingAmount: string;
  feeIcon: string;
}


