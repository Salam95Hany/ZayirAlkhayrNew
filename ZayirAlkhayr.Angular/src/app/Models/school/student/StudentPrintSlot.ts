export interface StudentPrintSlot {
  studentId: string;
  slotNumber: number;
  InstallmentRenewalDate?: string;
}

export type StudentPaymentStatus = 'PAID' | 'INSTALLMENT' | 'PENDING';

export interface TicketStudent {
  id: string;
  name: string;
  grade: string;
  paymentStatus: StudentPaymentStatus;
  installmentRenewalDate: string | null;
  lastPrintedAt: string | null;
}
