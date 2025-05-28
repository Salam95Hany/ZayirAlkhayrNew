import { CommonModule } from '@angular/common';
import { Component, EventEmitter, forwardRef, Input, Output, Renderer2 } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { NgbDropdownConfig, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { FormDropdownModel } from '../../Models/shared/FormDropdownModel';
import { SearchArryPipe } from "../../Pipes/search-arry.pipe";

@Component({
  selector: 'app-za-drop-down-form-control',
  standalone: true,
  imports: [CommonModule, NgbDropdownModule, SearchArryPipe],
  templateUrl: './za-drop-down-form-control.component.html',
  styleUrl: './za-drop-down-form-control.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ZaDropDownFormControlComponent),
      multi: true,
    },
  ],
})
export class ZaDropDownFormControlComponent {
  @Input() data: FormDropdownModel[] = [];
  @Input() placeholder: string = '';
  @Input() style: string = 'w-50';
  @Input() disabled: boolean = false;
  @Input() showSearch: boolean = false;
  @Input() selectMulti: boolean = false;
  @Input() isCustomDropdown: boolean = false;
  @Output() valueChanged = new EventEmitter<any | any[]>();
  selectedItems: string[] = [];
  selectedValues: string[] = [];
  filteredData: any[] = [];
  searchText: string = '';
  selectedValue: any = '';
  selectedName: string = '';
  selectedCode: string = '';
  searchFields: string[] = ['name', 'code'];

  private onChange: any = () => { };
  private onTouched: any = () => { };
  constructor(private dropdownConfig: NgbDropdownConfig, private renderer: Renderer2) { }

  ngOnInit(): void {
    if (this.isCustomDropdown)
      this.dropdownConfig.container = 'body';
    else
      this.dropdownConfig.container = null;
  }

  ngOnChanges(changes: any): void {
    if (changes.data) {
      if (this.selectMulti)
        this.writeValue(this.selectedValues);
      else
        this.writeValue(this.selectedValue);
    }
  }

  writeValue(value: any): void {
    if (this.selectMulti) {
      if (value)
        this.selectedValues = value;

      var items = this.data?.filter(x => value?.includes(x.value));
      if (items && items.length > 0) {
        this.selectedItems = items.map(x => x.name);
        items.map(x => x.isSelected = true);
      } else {
        this.selectedItems = [];
      }
      this.valueChanged.emit(this.selectedValues);
    }
    else {
      var sName = this.data?.find(x => x.value === value)?.name;

      if (value)
        this.selectedValue = value;
      if (sName)
        this.selectedName = sName;
      else
        this.selectedName = '';
    }
  }


  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    // Implement if needed
  }

  onInputChange(event: any) {
    const inputValue = event.target.value.toLowerCase();
    this.searchText = inputValue;
  }

  selectOption(option: any): void {
    this.selectedValue = option.value;
    this.filteredData = [];
    this.onChange(this.selectedValue);
    this.onTouched();
    this.selectedName = option.name;
    this.selectedCode = option.code;
    this.valueChanged.emit(option.value);
  }

  selectMultiOption(item: any) {
    const index = this.selectedValues.indexOf(item.value);
    var obj = this.data?.find(x => x.value == item.value);
    if (index > -1) {
      this.selectedItems.splice(index, 1);
      this.selectedValues.splice(index, 1);
      if (obj)
        obj.isSelected = false;

    } else {
      this.selectedItems.push(item.name);
      this.selectedValues.push(item.value);
      if (obj)
        obj.isSelected = true;
    }

    this.onChange(this.selectedValues);
    this.onTouched();
    this.valueChanged.emit(this.selectedValues);
  }

  removeSelected() {
    this.data?.map(x => x.isSelected = false);
    this.selectedItems = [];
    this.selectedValues = [];
    this.onChange([]);

  }
}

