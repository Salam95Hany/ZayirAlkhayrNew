import { FilterModel } from "./FilterModel";

export interface SearchReportModel {
    reportType: string;
    isLandScape?: boolean;
    rowCount?: number;
    userName?: string;
    queryString?: QueryString[];
    filterItems?: FilterModel[];
    headers?: PDFHeaderSelected[];
}

export interface QueryString {
    key: string;
    value: string;
}

export interface PDFHeaderSelected {
    nameEn: string;
    nameAr: string;
    isAllowSummation: boolean;
    valueType: string;
    displayOrder: number;
    isSelected: boolean;
}