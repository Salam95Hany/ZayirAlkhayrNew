import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { FamilyPatientTypes } from '../../../../../../../Models/zainstitution/GeneralStatus/FamilyStatusLookups';
import { FamilyDetails } from '../../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { FamilyPatientGroup, FamilyPatientTypeNames } from '../../../../../../../Models/zainstitution/GeneralStatus/UpdateFamilyStatusLookups';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormService } from '../../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators, RegexType } from '../../../../../../../Services/shared/custom-validators';
import { FormDropdownModel } from '../../../../../../../Models/shared/FormDropdownModel';
import { ZaDropDownFormControlComponent } from '../../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { ZaInputWithLabelComponent } from '../../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { NgFor, NgIf } from '@angular/common';
import { ZaEmptyDataComponent } from '../../../../../../../Shared/za-empty-data/za-empty-data.component';

@Component({
  selector: 'app-family-medical',
  standalone: true,
  imports: [ReactiveFormsModule, ZaDropDownFormControlComponent, ZaInputWithLabelComponent, NgIf, NgFor,ZaEmptyDataComponent],
  templateUrl: './family-medical.component.html',
  styleUrls: ['../family-step-shared.css', './family-medical.component.css']
})
export class FamilyMedicalComponent implements OnInit, OnChanges {
  @Input() PatientTypes: FormDropdownModel[] = [];
  @Input() FamilyDetails: FamilyDetails[] = [];
  @Input() FamilyStatusName: string;
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  @Input() FamilyPatients: FamilyPatientGroup[] = [];
  SelectedPatientTypes: FamilyPatientTypeNames[] = [];
  FamilyNames: FormDropdownModel[] = [];
  SelectedPatientTypeIds: string[] = [];
  ItemForm: FormGroup;
  FamilyPatientId: any;
  addMode = true;
  showMore = false;
  formErrors = {
    familyName: '',
    patientType: '',
    specialization: '',
    patientDate: ''
  };

  constructor(private modalService: NgbModal, private fb: FormBuilder, private formService: FormService,
    private toaster: ToastrService
  ) { }

  ngOnInit(): void {
    this.FormInit();
    if (this.UpdateMode && this.FamilyPatients.length > 0) {
      this.InetialData();
      this.mergeFamilyPatientUpdateMode();
    }

    if (this.DetailsMode)
      this.mergeFamilyPatientUpdateMode();
  }

  ngOnChanges(changes: SimpleChanges): void {
    let name = changes['FamilyStatusName'];
    let familyDetails = changes['FamilyDetails']
    if (this.UpdateMode) {
      if (name) {
        if (name.previousValue) {
          let obj = this.FamilyPatients.find(i => i.name == name.previousValue);
          if (obj)
            obj.name = name.currentValue;
        }
        this.InetialData();
      }

      if (familyDetails) {
        this.InetialData();
      }
    }
  }

  mergeFamilyPatientUpdateMode() {
    this.FamilyPatients.forEach(item => {
      let arry = this.PatientTypes.filter(i => item.patientTypeIds.includes(Number(i.value)));
      if (arry.length > 0) {
        item.patientTypeNames = arry.map(i => i.name).join(' ,');
        item.patientTypeList = arry.map<FamilyPatientTypeNames>(i => { return { id: i.value, name: i.name } });
      }
    });
  }

  InetialData() {
    if (this.FamilyDetails.length == 0) {
      const objectToKeep = this.FamilyPatients.find(i => i.name == this.FamilyStatusName);
      this.FamilyPatients = objectToKeep ? [objectToKeep] : [];
    } else {
      this.FamilyDetails.forEach(item => {
        let obj = this.FamilyPatients.find(i => i.name == item.oldName);
        if (obj)
          obj.name = item.name;
      });
      this.FamilyPatients.forEach((item, index) => {
        let obj = this.FamilyDetails.find(i => i.name == item.name);
        if (!obj && item.name != this.FamilyStatusName)
          this.FamilyPatients.splice(index, 1);
      });
    }

    this.FamilyNames = [];
    this.FamilyNames.push({ value: this.FamilyStatusName, name: this.FamilyStatusName });
    if (this.FamilyDetails && this.FamilyDetails.length > 0)
      this.FamilyNames.push(...this.FamilyDetails.map<FormDropdownModel>(i => { return { value: i.name, name: i.name } }));
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      patientDate: ['', [Validators.required]],
      specialization: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      isMedicalReport: false,
      isNeedProcess: false,
      familyName: ['', [Validators.required]],
      patientType: ['', [Validators.required]]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.SelectedPatientTypes = item.patientTypeList;
    this.SelectedPatientTypeIds = [...item.patientTypeIds];
    this.ItemForm.patchValue({
      id: item.id,
      patientDate: item.patientDate,
      specialization: item.specialization,
      isMedicalReport: item.isMedicalReport,
      isNeedProcess: item.isNeedProcess,
      familyName: item?.name?.toString() ?? '',
      patientType: this.SelectedPatientTypeIds.join(',')
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.SelectedPatientTypeIds = [];
    this.SelectedPatientTypes = [];
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
    this.FamilyPatientId = itemId;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  PatientTypeChange(items: string[]) {
    items = items.map(i => i.toString());
    this.SelectedPatientTypes = [];
    let arry = this.PatientTypes.filter(i => items.includes(i.value));
    this.SelectedPatientTypes = arry.map<FamilyPatientTypeNames>(i => { return { id: i.value, name: i.name } });
  }

  RemoveSelectedPatientTypes(id: any) {
    this.SelectedPatientTypes = this.SelectedPatientTypes.filter(i => i.id != id);
    this.SelectedPatientTypeIds = [...this.SelectedPatientTypes.map(i => i.id.toString())];
    if (this.SelectedPatientTypeIds.length == 0) {
      this.ItemForm.patchValue({ patientType: '' });
    }
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
      let checked = this.FamilyPatients.find(i => i.name == this.ItemForm?.value?.familyName);
      if (checked) {
        this.toaster.warning('هذا العنصر موجود');
        return;
      }
    }

    const formData = this.ItemForm.value;
    let arryNum = this.FamilyPatients.map(i => i.id);
    let id = arryNum.length > 0 ? Math.max(...arryNum) : 0;
    if (this.addMode) {
      this.FamilyPatients.push({
        id: id + 1,
        name: this.ItemForm?.value?.familyName,
        patientTypeIds: this.SelectedPatientTypes.map(i => i.id),
        patientTypeNames: this.SelectedPatientTypes.map(i => i.name).join(' ,'),
        patientTypeList: this.SelectedPatientTypes,
        patientDate: formData.patientDate,
        specialization: formData.specialization,
        isMedicalReport: formData.isMedicalReport,
        isNeedProcess: formData.isNeedProcess,
        familyName: formData.familyName
      });
    } else {
      let obj = this.FamilyPatients.find(i => i.id == formData.id);
      if (obj) {
        obj.name = this.ItemForm?.value?.familyName;
        obj.patientTypeIds = this.SelectedPatientTypes.map(i => i.id);
        obj.patientTypeNames = this.SelectedPatientTypes.map(i => i.name).join(' ,');
        obj.patientTypeList = this.SelectedPatientTypes;
        obj.patientDate = formData.patientDate;
        obj.specialization = formData.specialization;
        obj.isMedicalReport = formData.isMedicalReport;
        obj.isNeedProcess = formData.isNeedProcess;
        obj.familyName = formData.familyName;
      }
    }

    this.modalService.dismissAll();
  }

  DeleteItem() {
    this.FamilyPatients = this.FamilyPatients.filter(i => i.id != this.FamilyPatientId);
    this.modalService.dismissAll();
  }

  GetOutputData() {
    return this.FamilyPatients;
  }
}
