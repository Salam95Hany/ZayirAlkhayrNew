export interface PagedResponseModel<T> {
    results?: T;
    totalCount?: number;
    errorMessage?: string;
    isSuccess?: boolean;
    errorCode?: number;
}