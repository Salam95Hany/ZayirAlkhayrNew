import { FilterModel } from "./FilterModel";

export interface PagingFilterModel {
    searchText?: string;
    currentPage: number;
    pageSize: number;
    filterList: FilterModel[];
    filterType?: string;
}