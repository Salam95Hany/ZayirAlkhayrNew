export interface WithdrawnStudent {
  enrollmentId: number;
  studentId: number;
  name: string;
  code: string;
  stage: string;
  year: string;
  withdrawalDate: string | null;
  balance: number | null;
  status: number | null;
  settlementAmount: number | null;
  amountOutstanding: number | null;
  settlementPayer: 'School' | 'Student' | null;
}

export interface WithdrawalPayment {
  id: number;
  receiptNumber: string;
  paymentDate: string;
  amount: number;
  paymentMethod: number;
  isCancelled: boolean;
}

export interface WithdrawalFee {
  studentFeeId: number;
  name: string;
  originalStatus: number;
  original: number;
  discount: number;
  net: number;
  paid: number;
  due: number | null;
  payments: WithdrawalPayment[];
}

export interface StudentWithdrawal {
  student: WithdrawnStudent;
  enrollmentDate: string;
  withdrawalReason: string | null;
  months: number | null;
  totalPaid: number;
  totalDue: number | null;
  paidOn: string | null;
  method: number | null;
  fees: WithdrawalFee[];
}

export const settlementStatuses: Record<number, string> = {
  1: 'قيد التنفيذ', 2: 'تم السداد', 3: 'لا يوجد مبلغ مستحق'
};
