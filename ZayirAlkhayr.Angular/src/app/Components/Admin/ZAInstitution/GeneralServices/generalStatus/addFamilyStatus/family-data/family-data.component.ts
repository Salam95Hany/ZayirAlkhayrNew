import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FamilyDetails } from '../../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormService } from '../../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators, RegexType } from '../../../../../../../Services/shared/custom-validators';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaDropDownFormControlComponent } from '../../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { ZaEmptyDataComponent } from '../../../../../../../Shared/za-empty-data/za-empty-data.component';

@Component({
  selector: 'app-family-data',
  standalone: true,
  imports: [NgIf, NgFor, ZaInputWithLabelComponent, ZaDropDownFormControlComponent, ReactiveFormsModule,ZaEmptyDataComponent],
  templateUrl: './family-data.component.html',
  styleUrl: './family-data.component.css'
})
export class FamilyDataComponent {
  @Output() FamilyDetailsChange = new EventEmitter<FamilyDetails[]>();
  @Input() FamilyDetails: FamilyDetails[] = [];
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  ItemForm: FormGroup;
  FamilyDetailsId: any;
  FamilyChild = ["ابنة", "إبنة", "ابنه", "ابن", "إبنه", "إبن"];
  MaritalStatusValidation = false;
  FamilyChildCount = 0;
  addMode = true;
  MaritalStatus: any[] = [
    { value: 'أعزب', name: 'أعزب' },
    { value: 'متزوج', name: 'متزوج' },
    { value: 'مطلقة', name: 'مطلقة' },
    { value: 'أرمل', name: 'أرمل' }
  ];
  formErrors = {
    name: '',
    relevance: '',
    age: '',
    education: '',
    jop: '',
    nationalId: '',
    maritalStatus: ''
  };

  constructor(private modalService: NgbModal, private fb: FormBuilder, private formService: FormService,
    private toaster: ToastrService
  ) { }

  ngOnInit(): void {
    this.FormInit();
    this.FamilyChildCount = this.FamilyDetails.filter(i => this.FamilyChild.includes(i.relevance)).length;
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      name: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      relevance: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      age: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      education: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      jop: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      nationalId: ['', [CustomValidators.regexPattern(RegexType.noSpace)]],
      maritalStatus:['', [Validators.required,CustomValidators.regexPattern(RegexType.noSpace)]],
    });

     this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.patchValue({
      id: item.id,
      name: item.name,
      relevance: item.relevance,
      age: item.age,
      education: item.education,
      jop: item.jop,
      nationalId: item?.nationalId ?? '',
      maritalStatus:item?.maritalStatus
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
  }

  openItemModal(content: any, item: any) {
    this.ResetForm();
    this.addMode = true;
    if (item) {
      this.addMode = false;
      this.FillEditForm(item);
    }

    this.modalService.open(content, {
      size: 'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, itemId: any) {
    this.FamilyDetailsId = itemId;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
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

  AddNewItem() {
    this.ItemForm = this.formService.TrimFormInputValue(this.ItemForm);
    let isValid = this.validateForm();

    if (!isValid) {
      return;
    }

    if (this.addMode) {
      let checked = this.FamilyDetails.find(i => i.name == this.ItemForm.value.name);
      if (checked) {
        this.toaster.warning('هذا العنصر موجود');
        return;
      }
    }

    const formData = this.ItemForm.value;
    let arryNum = this.FamilyDetails.map(i => i.id);
    let id = arryNum.length > 0 ? Math.max(...arryNum) : 0;
    if (this.addMode) {
      this.FamilyDetails.push({
        id: id + 1,
        oldName: formData.name,
        name: formData.name,
        relevance: formData.relevance,
        age: formData.age,
        maritalStatus: formData.maritalStatus,
        education: formData.education,
        jop: formData.jop,
        nationalId: formData.nationalId
      });
    } else {
      let obj = this.FamilyDetails.find(i => i.id == formData.id);
      if (obj) {
        obj.oldName = obj.name;
        obj.name = formData.name;
        obj.relevance = formData.relevance;
        obj.age = formData.age;
        obj.maritalStatus = formData.maritalStatus;
        obj.education = formData.education;
        obj.jop = formData.jop;
        obj.nationalId = formData.nationalId;
      }
    }

    this.FamilyChildCount = this.FamilyDetails.filter(i => this.FamilyChild.includes(i.relevance)).length;
    if (this.UpdateMode)
      this.FamilyDetailsChange.emit(this.FamilyDetails);
    this.modalService.dismissAll();
  }

  DeleteItem() {
    debugger;
    this.FamilyDetails = this.FamilyDetails.filter(i => i.id != this.FamilyDetailsId);
    if (this.UpdateMode)
      this.FamilyDetailsChange.emit(this.FamilyDetails);
    this.FamilyChildCount = this.FamilyDetails.filter(i => this.FamilyChild.includes(i.relevance)).length;
    this.modalService.dismissAll();
  }

  GetOutputData() {
    if (this.FamilyDetails.length > 0) {
      this.FamilyDetails.forEach(item => {
        item.childernsCount = this.FamilyDetails.length;
        item.familyMembersCount = this.FamilyDetails.length + 1;
      });

      return this.FamilyDetails;
    } else
      return [];
  }
}
