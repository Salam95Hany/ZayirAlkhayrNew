import { Component, Input, SimpleChanges } from '@angular/core';
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { FormService } from '../../../../../Services/shared/form.service';
import { AuthService } from '../../../../../Auth/auth.service';
import { StudentDiscount } from '../../../../../Models/school/student/AddStudentModel';
import { FormDropdownModel } from '../../../../../Models/shared/FormDropdownModel';
import { NgFor, NgIf } from '@angular/common';
import { ZaDropDownFormControlComponent } from '../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';

@Component({
  selector: 'app-discount-data',
  standalone: true,
  imports: [ZaInputWithLabelComponent, ReactiveFormsModule, FormsModule, NgFor, ZaDropDownFormControlComponent, NgIf],
  templateUrl: './discount-data.component.html',
  styleUrl: './discount-data.component.css'
})
export class DiscountDataComponent {
  @Input() StudentDiscount: StudentDiscount[] = [];
  @Input() DiscountTypes: FormDropdownModel[] = [];
  @Input() StudentNames: string[] = [];
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  ItemForm!: FormGroup;
  formErrors: {
    discountReason: string;
    discountAmount: string;
  }[] = [];

  constructor(private fb: FormBuilder, private authService: AuthService, private formService: FormService) { }

  ngOnInit(): void {
    this.ItemForm = this.fb.group({
      discounts: this.fb.array([])
    });

    this.StudentDiscount.forEach((item, i) => {
      item.studentName = this.StudentNames[i];
    });

    this.ItemForm.valueChanges.subscribe(() => {
      this.discounts.controls.forEach((group, index) => {
        this.formErrors[index] = this.formService.validateForm(group as FormGroup, this.formErrors[index], true);
      });
    });

    this.loadData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    let studentNames = changes['StudentNames'];
    if (studentNames && studentNames?.currentValue?.length > 0) {
      this.StudentNames.forEach((item, i) => {
        let checked = this.StudentDiscount.some(x => x.studentName == item);
        if (!checked && studentNames?.currentValue?.length > studentNames?.previousValue?.length) {
          this.StudentDiscount.push({
            studentName: item,
            discountTypeId: null,
            discountAmount: null,
            discountReason: ''
          });
        }
      });

      this.StudentDiscount.forEach((item, i) => {
        item.studentName = this.StudentNames[i];
      });

      this.ItemForm = this.fb.group({
        discounts: this.fb.array([])
      });

      this.StudentDiscount.forEach((item, i) => {
        item.studentName = this.StudentNames[i];
      });

      this.ItemForm.valueChanges.subscribe(() => {
        this.discounts.controls.forEach((group, index) => {
          this.formErrors[index] = this.formService.validateForm(group as FormGroup, this.formErrors[index], true);
        });
      });
      this.syncFormToModel();
      this.loadData();
    }
  }

  InetialData(studentData: any[]) {
    if (studentData && studentData.length > 0) {
      if (this.StudentDiscount.length == 0) {
        studentData.forEach((item, i) => {
          this.StudentDiscount.push({
            studentName: studentData[i]?.studentName,
            discountTypeId: null,
            discountAmount: null,
            discountReason: ''
          });
        });
      } else {
        this.StudentDiscount.forEach((item, i) => {
          item.studentName = studentData[i]?.studentName;
        });
      }

      this.syncFormToModel();
      this.loadData();
    }
  }

  private syncFormToModel(): void {
    const values = this.ItemForm.getRawValue().discounts;
    this.StudentDiscount = this.StudentDiscount.map((item, index) => ({
      ...item,
      ...values[index]
    }));
  }

  get discounts(): FormArray {
    return this.ItemForm.get('discounts') as FormArray;
  }

  private loadData(): void {
    this.formErrors = this.StudentDiscount.map(() => ({ discountReason: '', discountAmount: '' }));
    this.discounts.clear();
    this.StudentDiscount.forEach(item => {
      const group = this.fb.group({
        studentName: [{ value: item.studentName, disabled: true }],
        discountTypeId: [item.discountTypeId ?? ''],
        discountReason: [item.discountReason ?? ''],
        discountAmount: [item.discountAmount ?? '']
      });

      group.get('discountTypeId')?.valueChanges.subscribe(value => {
        if (value) {
          this.formService.updateFieldValidators(group, 'discountReason', true, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]);
          this.formService.updateFieldValidators(group, 'discountAmount', true, [Validators.required]);
        }
        else {
          this.formService.updateFieldValidators(group, 'discountReason', false, []);
          this.formService.updateFieldValidators(group, 'discountAmount', false, []);
        }

      });

      if (this.DetailsMode) {
        group.disable({ emitEvent: false });
      }

      this.discounts.push(group);
    });
  }

  validateForm(): boolean {
    this.formService.markControlsAsTouched(this.ItemForm);
    let valid = true;
    this.discounts.controls.forEach((group, index) => {
      if (group.invalid) {
        valid = false;
        this.formErrors[index] = this.formService.validateForm(
          group as FormGroup,
          this.formErrors[index],
          false
        );
      }
    });

    return valid;
  }

  GetOutputData(): StudentDiscount[] | null {
    let isValid = this.validateForm();
    if (!isValid) {
      return null;
    }

    const values = this.ItemForm.getRawValue().discounts;
    return this.StudentDiscount.map((x, index) => ({
      ...x,
      ...values[index]
    }));
  }
}
