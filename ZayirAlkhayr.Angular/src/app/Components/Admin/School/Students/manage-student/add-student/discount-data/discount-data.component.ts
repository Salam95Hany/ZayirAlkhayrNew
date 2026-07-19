import { Component, Input, SimpleChanges } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaDropDownFormControlComponent } from '../../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { StudentDetails, StudentDiscount } from '../../../../../../../Models/school/student/AddStudentModel';
import { FormDropdownModel } from '../../../../../../../Models/shared/FormDropdownModel';
import { AuthService } from '../../../../../../../Auth/auth.service';
import { FormService } from '../../../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../../../Services/shared/custom-validators';

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
  @Input() StudentDetails: StudentDetails[] = [];
  @Input() FeeTemplates: any[] = [];
  @Input() CurrentYear: FormDropdownModel;
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
      item.studentName = this.StudentDetails[i].studentName;
    });

    this.ItemForm.valueChanges.subscribe(() => {
      this.discounts.controls.forEach((group, index) => {
        this.formErrors[index] = this.formService.validateForm(group as FormGroup, this.formErrors[index], true);
      });
    });

    this.loadData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    let studentDetails = changes['StudentDetails'];
    if (studentDetails && studentDetails?.currentValue?.length > 0) {
      this.StudentDetails.forEach((item, i) => {
        let checked = this.StudentDiscount.some(x => x.studentName == item.studentName);
        if (!checked && studentDetails?.currentValue?.length > studentDetails?.previousValue?.length) {
          let amount = this.FeeTemplates.find(i => i.academicStageId == item.academicStageId)?.amount;
          this.StudentDiscount.push({
            studentName: item.studentName,
            academicStageName: item.academicStageName,
            academicYear: item.academicYear,
            studyAmount: amount,
            discountTypeId: null,
            discountAmount: null,
            discountReason: '',
            notes: ''
          });
        }
      });

      this.StudentDiscount.forEach((item, i) => {
        item.studentName = this.StudentDetails[i].studentName;
      });

      this.ItemForm = this.fb.group({
        discounts: this.fb.array([])
      });

      this.StudentDiscount.forEach((item, i) => {
        item.studentName = this.StudentDetails[i].studentName;
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

  InetialData(studentData: StudentDetails[]) {
    if (studentData && studentData.length > 0) {
      this.StudentDiscount = [];
      studentData.forEach((item, i) => {
        let amount = this.FeeTemplates.find(i => i.academicStageId == item.academicStageId)?.amount;
        this.StudentDiscount.push({
          studentName: item.studentName,
          academicStageName: item.academicStageName,
          academicYear: item.academicYear,
          studyAmount: amount,
          discountTypeId: null,
          discountAmount: null,
          discountReason: '',
          notes: ''
        });
      });

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
        academicStageName: [{ value: item.academicStageName, disabled: true }],
        academicYear: [{ value: item.academicYear, disabled: true }],
        studyAmount: [{ value: item.studyAmount, disabled: true }],
        discountTypeId: [item.discountTypeId ?? ''],
        discountReason: [item.discountReason ?? ''],
        discountAmount: [item.discountAmount ?? ''],
        notes: [item.discountAmount ?? '']
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
