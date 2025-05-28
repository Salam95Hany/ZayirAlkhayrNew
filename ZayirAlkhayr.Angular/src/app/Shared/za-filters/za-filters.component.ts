import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { SearchArryPipe } from "../../Pipes/search-arry.pipe";
import { FilterModel } from '../../Models/shared/FilterModel';

@Component({
  selector: 'app-za-filters',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbDropdownModule, SearchArryPipe],
  templateUrl: './za-filters.component.html',
  styleUrl: './za-filters.component.css'
})
export class ZaFiltersComponent {
  @Input() FilterList: FilterModel[] = [];
  @Input() showSearchText: boolean = false;
  @Input() searchPlaceholder: string = '';
  @Output() filterChanged = new EventEmitter<FilterModel[]>();
  SelectedFilter: FilterModel[] = [];
  SearchText: string = '';

  filterSearch: string = '';

  constructor() { }

  filterChecked() {
    this.SelectedFilter = [];
    this.FilterList.map(item => {
      let checked = item.filterItems.filter(a => a.isChecked && a.isChecked == true);
      if (checked.length > 0) {
        checked.map(obj => {
          this.SelectedFilter.push(obj);
        });
      }
    });
    if (this.SearchText) {
      const textFilter: FilterModel = {
        categoryDisplayName: 'بحث بالنص',
        categoryName: 'SearchText',
        itemKey: this.SearchText,
        itemFlag: this.SearchText,
        itemValue: this.SearchText,
        isChecked: true
      }
      this.SelectedFilter.push(textFilter);
    }
    this.filterChanged.emit(this.SelectedFilter);
  }

  RemoveSelectedFilter(filter: any, index: number) {
    this.SelectedFilter.splice(index, 1);
    this.FilterList.map(item => {
      let checked = item.filterItems.find(a => a.categoryName == filter.categoryName);
      if (checked) {
        checked.isChecked = false;
      }
    });
    if (filter.categoryName == 'SearchText')
      this.SearchText = '';

    this.filterChanged.emit(this.SelectedFilter);
  }

  RemoveAllFilters() {
    this.SelectedFilter = [];
    this.FilterList.map(item => {
      item.filterItems.map(a => a.isChecked = false);
    });
    this.SearchText = '';
    this.filterChanged.emit(this.SelectedFilter);
  }
}
