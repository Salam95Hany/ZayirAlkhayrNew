import { Component, forwardRef, Input } from '@angular/core';
import { FormService } from '../../Services/shared/form.service';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-za-input-with-label',
  standalone: true,
  imports: [NgIf],
  templateUrl: './za-input-with-label.component.html',
  styleUrl: './za-input-with-label.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ZaInputWithLabelComponent),
      multi: true
    }
  ]
})
export class ZaInputWithLabelComponent {
  @Input() type: 'text' | 'number' | 'date' | 'month' = 'text';
  @Input() placeholder: string = '';
  @Input() error: string | null = null;
  @Input() allowNumbersOnly: boolean = false;
  @Input() allowPaste: boolean = true;
  @Input() allowCopy: boolean = true;
  @Input() allowCut: boolean = true;
  @Input() disabled: boolean = false;
  value: any = '';

  constructor(private formService: FormService) {
  }

  writeValue(obj: any): void {
    this.value = obj;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onChange = (value: any) => { };
  onTouched = () => { };

  onInput(event: any) {
    this.value = event.target.value;
    this.onChange(this.value);
  }

  NumbersOnly(key: any) {
    if (this.allowNumbersOnly)
      return this.formService.NumbersOnly(key);

    return true;
  }

}
