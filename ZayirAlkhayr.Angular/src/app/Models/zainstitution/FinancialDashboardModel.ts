export interface FinancialTrendModel {
  month: string;
  revenue: number;
  expenses: number;
  balance: number;
}

export interface ExpenseCategoryModel {
  category: string;
  amount: number;
}

export interface FinancialChartsResponseModel {
  table: ExpenseCategoryModel[];
  table1: FinancialTrendModel[];
}
