import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FamilyStatus } from '../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { FamilyCategories, FamilyNationalities, FamilyStatusTypes } from '../../../../../../Models/zainstitution/GeneralStatus/FamilyStatusLookups';
import { DatePipe, NgIf } from '@angular/common';
import { AuthService } from '../../../../../../Auth/auth.service';
import { ZaInputWithLabelComponent } from "../../../../../../Shared/za-input-with-label/za-input-with-label.component";
import { ZaDropDownFormControlComponent } from "../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormService } from '../../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../../Services/shared/custom-validators';
import { FormDropdownModel } from '../../../../../../Models/shared/FormDropdownModel';

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
      name: [this.FamilyStatus.name, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      fname: [this.FamilyStatus.fname, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      phone: [this.FamilyStatus.phone, Validators.required],
      phone1: [this.FamilyStatus.phone1],
      address: [this.FamilyStatus.address, CustomValidators.regexPattern(RegexType.noSpace)],
      relevance: [this.FamilyStatus.relevance, CustomValidators.regexPattern(RegexType.noSpace)],
      education: [this.FamilyStatus.education, CustomValidators.regexPattern(RegexType.noSpace)],
      jop: [this.FamilyStatus.jop, CustomValidators.regexPattern(RegexType.noSpace)],
      age: [this.FamilyStatus.age],
      nationalId: [this.FamilyStatus.nationalId, CustomValidators.regexPattern(RegexType.noSpace)],
      village: [this.FamilyStatus.village, CustomValidators.regexPattern(RegexType.noSpace)],
      center: [this.FamilyStatus.center, CustomValidators.regexPattern(RegexType.noSpace)],
      governorate: [this.FamilyStatus.governorate, CustomValidators.regexPattern(RegexType.noSpace)],
      maritalStatus: [this.FamilyStatus.maritalStatus, CustomValidators.regexPattern(RegexType.noSpace)],
      statusTypeId: [this.FamilyStatus.statusTypeId, Validators.required],
      nationalityId: [this.FamilyStatus.nationalityId, Validators.required],
      categoryId: [this.FamilyStatus.categoryId, Validators.required],
      addedDate: [this.FamilyStatus.addedDate, Validators.required]
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

    return this.ItemForm.value;
  }
}
