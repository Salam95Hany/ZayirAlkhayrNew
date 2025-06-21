import { FilterModel } from "./FilterModel";

export interface PDFHeaderSelectedModel {
    nameEn: string;
    nameAr: string;
    isAllowSummation: boolean;
    valueType: string;
    displayOrder: number;
    isSelected: boolean;
}

export interface PDFModel {
    headers: PDFHeaderSelectedModel[];
    filterList: FilterModel[];
}