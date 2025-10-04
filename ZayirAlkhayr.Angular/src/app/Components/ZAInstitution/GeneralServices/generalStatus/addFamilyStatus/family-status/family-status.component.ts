import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FamilyStatus } from '../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { FamilyCategories, FamilyNationalities, FamilyStatusTypes } from '../../../../../../Models/zainstitution/GeneralStatus/FamilyStatusLookups';
import { DatePipe } from '@angular/common';
import { AuthService } from '../../../../../../Auth/auth.service';
import { ZaInputWithLabelComponent } from "../../../../../../Shared/za-input-with-label/za-input-with-label.component";
import { ZaDropDownFormControlComponent } from "../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-family-status',
  standalone: true,
  imports: [ZaInputWithLabelComponent, ZaDropDownFormControlComponent, ReactiveFormsModule],
  templateUrl: './family-status.component.html',
  styleUrl: './family-status.component.css',
  providers: [DatePipe]
})
export class FamilyStatusComponent implements OnInit {
  @Output() StatusNameChanged = new EventEmitter<string>();
  @Input() FamilyStatus: FamilyStatus = {} as FamilyStatus;
  @Input() Categories: FamilyCategories[] = [];
  @Input() Nationalities: FamilyNationalities[] = [];
  @Input() StatusTypes: FamilyStatusTypes[] = [];
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;

  UserId: any;
  isDate = false;
  ItemForm: FormGroup;

  MaritalStatus: any[] = [
    { id: 1, name: 'أعزب' },
    { id: 2, name: 'متزوج' },
    { id: 3, name: 'مطلقة' },
    { id: 4, name: 'أرمل' }
  ];

   formErrors = {
    name: '',
    fname: '',
    phone: '',
    address: '',
    statusTypeId: '',
    nationalityId: '',
    categoryId: '',
    addedDate: ''
  };

  constructor(private datePipe: DatePipe, private fb: FormBuilder, private authService: AuthService) { }

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
      name: [this.FamilyStatus.name, Validators.required],
      fname: [this.FamilyStatus.fname, Validators.required],
      phone: [this.FamilyStatus.phone, [Validators.required]],
      phone1: [this.FamilyStatus.phone1],
      address: [this.FamilyStatus.address, Validators.required],
      relevance: [this.FamilyStatus.relevance],
      education: [this.FamilyStatus.education],
      jop: [this.FamilyStatus.jop],
      age: [this.FamilyStatus.age],
      nationalId: [this.FamilyStatus.nationalId],
      village: [this.FamilyStatus.village],
      center: [this.FamilyStatus.center],
      governorate: [this.FamilyStatus.governorate],
      maritalStatus: [this.FamilyStatus.maritalStatus],
      statusTypeId: [this.FamilyStatus.statusTypeId, Validators.required],
      nationalityId: [this.FamilyStatus.nationalityId, Validators.required],
      categoryId: [this.FamilyStatus.categoryId, Validators.required],
      addedDate: [this.FamilyStatus.addedDate, Validators.required]
    });

    this.ItemForm.get('name')?.valueChanges.subscribe(value => {
      if (this.UpdateMode) {
        this.StatusNameChanged.emit(value);
      }
    });
  }

  onfocus() {
    this.isDate = true;
  }

  GetOutputData() {
    this.ItemForm.markAllAsTouched();
    if (!this.ItemForm.valid) {
      return null;
    }
    return this.ItemForm.value;
  }
}
