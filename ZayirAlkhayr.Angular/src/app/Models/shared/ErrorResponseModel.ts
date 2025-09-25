export interface ApiResponseModel<T> {
    isSuccess: boolean;
    message: string;
    totalCount: number;
    results: T;
}