export interface BeneFactorValues {
    id?: number;
    beneFactorId?: number;
    beneFactorTypeId?: number;
    totalValue?: number | null;
    paymentDate?: string;
    isActive?: boolean;
    fullName?: string,
    code?: string;
    insertUser?: string;
    insertDate?: string | null;
    updateUser?: string;
    updateDate?: string | null;
}
export interface BeneFactorDetails {
    id: number;
    beneFactorId: number;
    parentId: number | null;
    beneFactorTypeId: number;
    details: string;
    totalValue: number | null;
    isActive: boolean | null;
    isParent: boolean | null;
    paymentDate: string;
    insertUser: string;
    isFinalSubscribe: boolean;
    fullName?: string,
    code?: string;
}