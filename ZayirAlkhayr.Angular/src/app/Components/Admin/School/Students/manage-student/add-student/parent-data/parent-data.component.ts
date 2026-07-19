import { Component, Input } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZaInputWithLabelComponent } from '../../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ParentStudent } from '../../../../../../../Models/school/student/AddStudentModel';
import { AuthService } from '../../../../../../../Auth/auth.service';
import { FormService } from '../../../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../../../Services/shared/custom-validators';

@Component({
  selector: 'app-parent-data',
  standalone: true,
  imports: [ZaInputWithLabelComponent, ReactiveFormsModule, FormsModule],
  templateUrl: './parent-data.component.html',
  styleUrl: './parent-data.component.css'
})
export class ParentDataComponent {
  @Input() ParentStudent: ParentStudent = {} as ParentStudent;
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  UserId: any;
  ItemForm: FormGroup;
  formErrors = {
    parentName: '',
    phone: '',
    address: ''
  };

  constructor(private fb: FormBuilder, private authService: AuthService, private formService: FormService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.ParentStudent.insertUser = this.UserId;
    if (!this.UpdateMode && !this.DetailsMode)
      this.ParentStudent.childrenCount = 0;
    this.ItemForm = this.fb.group({
      parentName: [{ value: this.ParentStudent.parentName ?? '', disabled: this.DetailsMode ? true : false }, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      phone: [{ value: this.ParentStudent.phone ?? '', disabled: this.DetailsMode ? true : false }, [Validators.required]],
      address: [{ value: this.ParentStudent.address ?? '', disabled: this.DetailsMode ? true : false }, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  validateForm(): boolean {
    this.formService.markFormGroupTouched(this.ItemForm);
    if (this.ItemForm.valid) {
      return true;
    } else {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, false)
      return false;
    }
  }

  GetOutputData() {
    let isValid = this.validateForm();
    if (!isValid) {
      return null;
    }

    this.ParentStudent = { ...this.ParentStudent, ...this.ItemForm.value };
    return this.ParentStudent;
  }
}
