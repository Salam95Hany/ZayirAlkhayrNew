import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { SearchArryPipe } from "../../Pipes/search-arry.pipe";
import { FilterModel } from '../../Models/shared/FilterModel';
import { NgxDaterangepickerMd, LocaleService, LOCALE_CONFIG } from 'ngx-daterangepicker-material';

@Component({
  selector: 'app-za-filters',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbDropdownModule, SearchArryPipe,NgxDaterangepickerMd],
  templateUrl: './za-filters.component.html',
  styleUrl: './za-filters.component.css',
  providers: [
    LocaleService,
    {
      provide: LOCALE_CONFIG,
      useValue: {
        format: 'YYYY-MM-DD',
        applyLabel: 'Apply',
        daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
        monthNames: [
          'January', 'February', 'March', 'April', 'May', 'June',
          'July', 'August', 'September', 'October', 'November', 'December'
        ],
      },
    },
  ],
})
export class ZaFiltersComponent {
   @Input() FilterList: FilterModel[] = [];
  @Output() filterChanged = new EventEmitter<FilterModel[]>();
  SelectedFilter: FilterModel[] = [];

  constructor() { }

  updateFilters(filter?: FilterModel, range?: any) {
    let updatedFilters = [...this.SelectedFilter];
    
    if (filter?.filterType === 'Checkbox') {
      updatedFilters = updatedFilters.filter(f => f.filterType !== 'Checkbox');

      const checkedItems = this.FilterList
        .filter(f => f.filterType === 'Checkbox')
        .flatMap(f => f.filterItems!.filter(x => x.isChecked)
          .map(x => ({
            ...x,
            categoryName: f.categoryName,
            categoryDisplayName: f.categoryDisplayName,
            filterType: 'Checkbox'
          }))
        );

      updatedFilters.push(...checkedItems);
    }

    else if (filter) {
      updatedFilters = updatedFilters.filter(f => f.categoryName !== filter.categoryName);

      switch (filter.filterType) {
        case 'SearchText':
        case 'Day':
        case 'Month':
          if (filter.itemId && filter.itemId.trim() !== '') {
            updatedFilters.push({
              categoryName: filter.categoryName,
              categoryDisplayName: filter.categoryDisplayName,
              itemKey: filter.itemKey,
              itemId: filter.itemId,
              filterType: filter.filterType
            });
          }
          break;

        case 'DateRange':
          if (!range || !range.startDate || !range.endDate) return;
          if (range && range.endDate) {
            updatedFilters.push({
              categoryName: filter.categoryName,
              categoryDisplayName: filter.categoryDisplayName,
              filterType: 'DateRange',
              from: range.startDate.format('YYYY-MM-DD'),
              to: range.endDate.format('YYYY-MM-DD'),
            });
          }
          break;
      }
    }

    this.SelectedFilter = updatedFilters;
    this.filterChanged.emit(this.SelectedFilter);
  }

  removeSelectedFilter(filter: FilterModel, index: number) {
    this.SelectedFilter.splice(index, 1);

    this.FilterList.forEach(f => {
      if (f.filterType === 'Checkbox' && f.filterItems) {
        f.filterItems.forEach(item => {
          if (item.itemId === filter.itemId) item.isChecked = false;
        });
      }
      if (f.categoryName === filter.categoryName) {
        f.itemKey = '';
        f.itemId = '';
      }
    });

    this.filterChanged.emit(this.SelectedFilter);
  }

  removeAllFilters() {
    this.SelectedFilter = [];
    this.FilterList.forEach(f => {
      f.itemKey = '';
      f.itemId = '';
      if (f.filterItems) f.filterItems.forEach(i => i.isChecked = false);
    });
    this.filterChanged.emit(this.SelectedFilter);
  }
}
