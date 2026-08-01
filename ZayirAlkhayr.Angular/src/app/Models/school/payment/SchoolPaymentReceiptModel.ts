export interface SchoolPaymentReceiptModel {
  school: ReceiptSchool;
  receipt: ReceiptIdentity;
  payment: ReceiptPayment;
  student: ReceiptStudent;
  guardian?: ReceiptGuardian;
  cashier?: ReceiptCashier;
  allocations: ReceiptFeeAllocation[];
  summary: ReceiptFinancialSummary;
  presentation?: ReceiptPresentation;
}

export interface ReceiptSchool {
  id?: string | number;
  nameAr: string;
  nameEn?: string;
  slogan?: string;
  logoUrl?: string;
  stampUrl?: string;
  contacts?: {
    phone?: string;
    address?: string;
    website?: string;
    email?: string;
  };
}

export interface ReceiptIdentity {
  number: string;
  issuedAt: string | Date;
  title?: string;
  externalReference?: string;
}

export interface ReceiptPayment {
  type: ReceiptLookup;
  method: ReceiptLookup;
  status: ReceiptLookup;
  transactionReference?: string;
}

export interface ReceiptLookup {
  code: string;
  label: string;
}

export interface ReceiptStudent {
  id?: string | number;
  studentNumber: string;
  fullName: string;
  academicStage: string;
  grade: string;
  classroom?: string;
  academicYear?: string;
}

export interface ReceiptGuardian {
  id?: string | number;
  fullName: string;
  phone?: string;
  relationship?: string;
}

export interface ReceiptCashier {
  id?: string | number;
  fullName: string;
  employeeNumber?: string;
  branchName?: string;
}

export interface ReceiptFeeAllocation {
  id?: string | number;
  fee: {
    id?: string | number;
    code?: string;
    name: string;
  };
  academicPeriod?: string;
  assessedAmount: number;
  paidAmount: number;
  remainingAmount: number;
}

export interface ReceiptFinancialSummary {
  totalAssessed: number;
  totalPaid: number;
  totalRemaining: number;
  amountInWords?: string;
  currency: {
    code: string;
    symbol?: string;
    fractionDigits?: number;
  };
}

export interface ReceiptPresentation {
  thankYouMessage?: string;
  retentionNote?: string;
  notes?: string[];
  qrCode?: {
    imageUrl: string;
    payload?: string;
    caption?: string;
  };
  barcode?: {
    imageUrl: string;
    value: string;
    caption?: string;
  };
}
