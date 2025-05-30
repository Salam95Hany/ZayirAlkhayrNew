export interface UploadFileModel {
    id?: number;
    files?: File[];
    deletedFiles?: DeletedFileModel[];
}

export interface DeletedFileModel {
    id: number;
    fileName: string;
}

export interface FileSortingModel {
    fileId: string;
    displayOrder: number;
}