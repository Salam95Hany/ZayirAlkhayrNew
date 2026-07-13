import { Component, Input } from '@angular/core';
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { FormService } from '../../../../../Services/shared/form.service';
import { AuthService } from '../../../../../Auth/auth.service';
import { StudentDiscount } from '../../../../../Models/school/student/AddStudentModel';

@Component({
  selector: 'app-discount-data',
  standalone: true,
  imports: [ZaInputWithLabelComponent, ReactiveFormsModule, FormsModule],
  templateUrl: './discount-data.component.html',
  styleUrl: './discount-data.component.css'
})
export class DiscountDataComponent {
  @Input() StudentDiscount: StudentDiscount[] = [];
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  ItemForm!: FormGroup;

  constructor(private fb: FormBuilder,private authService: AuthService,private formService: FormService) { }

  ngOnInit(): void {
    this.ItemForm = this.fb.group({
      discounts: this.fb.array([])
    });

    this.loadData();
  }

  get discounts(): FormArray {
    return this.ItemForm.get('discounts') as FormArray;
  }

  private loadData(): void {
    this.StudentDiscount.forEach(item => {
      this.discounts.push(
        this.fb.group({
          studentName: [{value: item.studentName,disabled: true}],
          discountTypeId: [
            {
              value: item.discountTypeId ?? '',
              disabled: this.DetailsMode
            },
            [
              Validators.required,
              CustomValidators.regexPattern(RegexType.noSpace)
            ]
          ],
          discountReason: [
            {
              value: item.discountReason ?? '',
              disabled: this.DetailsMode
            },
            [
              Validators.required,
              CustomValidators.regexPattern(RegexType.noSpace)
            ]
          ],
          discountAmount: [
            {
              value: item.discountAmount ?? '',
              disabled: this.DetailsMode
            },
            [
              Validators.required,
              CustomValidators.regexPattern(RegexType.numeric)
            ]
          ]

        })
      );

    });

  }

  validateForm(): boolean {
    this.formService.markFormGroupTouched(this.ItemForm);
    return this.ItemForm.valid;
  }

  GetOutputData(): StudentDiscount[] | null {
    if (!this.validateForm())
      return null;

    const values = this.ItemForm.getRawValue().discounts;
    return this.StudentDiscount.map((x, index) => ({
      ...x,
      ...values[index]
    }));
  }
}
