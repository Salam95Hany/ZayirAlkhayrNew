export interface ErrorResponseModel<T> {
    isSuccess: boolean;
    message: string;
    errorCode: number;
    results: T;
}