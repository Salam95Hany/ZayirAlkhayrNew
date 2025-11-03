export interface FilterModel {
    categoryDisplayName?: string;
    categoryName?: string;
    itemId?: string;
    itemKey?: string;
    itemValue?: string;
    from?: string;
    to?: string;
    filterType?: string;
    isChecked?: boolean;
    filterItems?: FilterModel[];
}

