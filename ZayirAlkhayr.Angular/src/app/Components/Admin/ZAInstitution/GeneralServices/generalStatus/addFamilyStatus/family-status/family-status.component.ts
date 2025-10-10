import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FamilyStatus } from '../../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { DatePipe, NgIf } from '@angular/common';
import { AuthService } from '../../../../../../../Auth/auth.service';
import { ZaInputWithLabelComponent } from "../../../../../../../Shared/za-input-with-label/za-input-with-label.component";
import { ZaDropDownFormControlComponent } from "../../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormService } from '../../../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../../../Services/shared/custom-validators';
import { FormDropdownModel } from '../../../../../../../Models/shared/FormDropdownModel';

@Component({
  selector: 'app-family-status',
  standalone: true,
  imports: [ZaInputWithLabelComponent, ZaDropDownFormControlComponent, ReactiveFormsModule, NgIf],
  templateUrl: './family-status.component.html',
  styleUrl: './family-status.component.css',
  providers: [DatePipe]
})
export class FamilyStatusComponent implements OnInit {
  @Output() StatusNameChanged = new EventEmitter<string>();
  @Input() FamilyStatus: FamilyStatus = {} as FamilyStatus;
  @Input() Categories: FormDropdownModel[] = [];
  @Input() Nationalities: FormDropdownModel[] = [];
  @Input() StatusTypes: FormDropdownModel[] = [];
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;

  UserId: any;
  isDate = false;
  ItemForm: FormGroup;

  MaritalStatus: any[] = [
    { value: 'أعزب', name: 'أعزب' },
    { value: 'متزوج', name: 'متزوج' },
    { value: 'مطلقة', name: 'مطلقة' },
    { value: 'أرمل', name: 'أرمل' }
  ];

  formErrors = {
    name: '',
    fname: '',
    phone: '',
    address: '',
    relevance: '',
    statusTypeId: '',
    nationalityId: '',
    categoryId: '',
    addedDate: '',
    education: '',
    jop: '',
    nationalId: '',
    village: '',
    center: '',
    governorate: '',
    maritalStatus: ''
  };

  constructor(private datePipe: DatePipe, private fb: FormBuilder, private authService: AuthService, private formService: FormService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;

    if (!this.UpdateMode && !this.DetailsMode) {
      this.FamilyStatus.nationalityId = null;
      this.FamilyStatus.categoryId = null;
      this.FamilyStatus.statusTypeId = null;
      this.FamilyStatus.maritalStatus = null;
    } else {
      this.isDate = !!this.FamilyStatus.addedDate;
      this.FamilyStatus.addedDate = this.datePipe.transform(this.FamilyStatus.addedDate, 'yyyy-MM-dd');
    }

    this.FamilyStatus.insertUser = this.UserId;

    this.ItemForm = this.fb.group({
      name: [{ value: this.FamilyStatus.name, disabled: this.DetailsMode ? true : false }, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      fname: [{ value: this.FamilyStatus.fname, disabled: this.DetailsMode ? true : false }, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      phone: [{ value: this.FamilyStatus.phone, disabled: this.DetailsMode ? true : false }, Validators.required],
      phone1: [{ value: this.FamilyStatus.phone1 ?? '', disabled: this.DetailsMode ? true : false }],
      address: [{ value: this.FamilyStatus.address ?? '', disabled: this.DetailsMode ? true : false }, CustomValidators.regexPattern(RegexType.noSpace)],
      relevance: [{ value: this.FamilyStatus.relevance ?? '', disabled: this.DetailsMode ? true : false }, CustomValidators.regexPattern(RegexType.noSpace)],
      education: [{ value: this.FamilyStatus.education ?? '', disabled: this.DetailsMode ? true : false }, CustomValidators.regexPattern(RegexType.noSpace)],
      jop: [{ value: this.FamilyStatus.jop ?? '', disabled: this.DetailsMode ? true : false }, CustomValidators.regexPattern(RegexType.noSpace)],
      age: [{ value: this.FamilyStatus.age ?? '', disabled: this.DetailsMode ? true : false }],
      nationalId: [{ value: this.FamilyStatus.nationalId?.toString() ?? '', disabled: this.DetailsMode ? true : false }, CustomValidators.regexPattern(RegexType.noSpace)],
      village: [{ value: this.FamilyStatus.village ?? '', disabled: this.DetailsMode ? true : false }, CustomValidators.regexPattern(RegexType.noSpace)],
      center: [{ value: this.FamilyStatus.center ?? '', disabled: this.DetailsMode ? true : false }, CustomValidators.regexPattern(RegexType.noSpace)],
      governorate: [{ value: this.FamilyStatus.governorate ?? '', disabled: this.DetailsMode ? true : false }, CustomValidators.regexPattern(RegexType.noSpace)],
      maritalStatus: [this.FamilyStatus.maritalStatus, CustomValidators.regexPattern(RegexType.noSpace)],
      statusTypeId: [this.FamilyStatus.statusTypeId?.toString(), Validators.required],
      nationalityId: [this.FamilyStatus.nationalityId?.toString(), Validators.required],
      categoryId: [this.FamilyStatus.categoryId?.toString(), Validators.required],
      addedDate: [{ value: this.FamilyStatus.addedDate, disabled: this.DetailsMode || this.UpdateMode ? true : false }, Validators.required]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });

    this.ItemForm.get('name')?.valueChanges.subscribe(value => {
      if (this.UpdateMode) {
        this.StatusNameChanged.emit(value);
      }
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

    this.FamilyStatus = { ...this.FamilyStatus, ...this.ItemForm.value };

    return this.FamilyStatus;
  }
}
