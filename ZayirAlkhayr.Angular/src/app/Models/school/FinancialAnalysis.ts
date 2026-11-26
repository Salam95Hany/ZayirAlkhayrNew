export type AnalysisParams = Record<string, string | number | boolean>;

export interface CashTotals {
  receipts: number;
  expenses: number;
  refunds: number;
  net: number;
}

export interface FinancialSummary {
  from: string;
  to: string;
  totals: CashTotals;
  monthly: { year: number; month: number; totals: CashTotals }[];
  byMethod: { method: number | null; amount: number }[];
  byExpenseType: { expenseTypeId: number; name: string; amount: number }[];
}

export interface FinancialMovement {
  id: number;
  type: number;
  date: string;
  enrollmentId: number | null;
  name: string;
  studentCode: string | null;
  reference: string | null;
  method: number | null;
  isCancelled: boolean;
  amount: number;
  incoming: number;
  outgoing: number;
}

interface FeeAmounts {
  feeType: string;
  original: number;
  discount: number;
  net: number;
  paid: number;
  remaining: number;
}

interface StudentRow {
  enrollmentId: number;
  student: string;
  code: string;
  stage: string;
  year: string;
}

export interface FinancialBalances {
  byFeeType: (FeeAmounts & { feeTypeId: number })[];
  fees: (FeeAmounts & StudentRow & { nextInstallmentDate: string | null; nextAmount: number | null })[];
  bySettlementStatus: { status: number; schoolOwes: number; studentOwes: number }[];
  settlements: (StudentRow & { withdrawalDate: string; status: number; balance: number; paidOn: string | null })[];
}
