import { DatePipe, NgFor, NgIf } from '@angular/common';
import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZaDropDownFormControlComponent } from '../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { FamilyNeeds } from '../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormService } from '../../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { FormDropdownModel } from '../../../../../../Models/shared/FormDropdownModel';
import { ZaInputWithLabelComponent } from '../../../../../../Shared/za-input-with-label/za-input-with-label.component';

@Component({
  selector: 'app-family-need',
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule, ZaDropDownFormControlComponent, ZaEmptyDataComponent,ZaInputWithLabelComponent],
  templateUrl: './family-need.component.html',
  styleUrl: './family-need.component.css',
  providers: [DatePipe]
})
export class FamilyNeedComponent implements OnInit, OnChanges {
  @Input() FamilyNeeds: FormDropdownModel[] = [];
  @Input() FamilyCategories: FormDropdownModel[] = [];
  @Input() SelectedNeeds: FamilyNeeds[] = [];
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  FamilyNeedsByCategory: FormDropdownModel[] = [];
  ItemForm: FormGroup;
  SelectedNeedId: any;
  addMode = true;
  formErrors = {
    categoryId: '',
    familyNeedId: '',
    deliveryDate: ''
  };

  constructor(private modalService: NgbModal, private fb: FormBuilder, private formService: FormService,
    private toaster: ToastrService, private datePipe: DatePipe
  ) { }

  ngOnInit(): void {
    this.FormInit();
    if ((this.UpdateMode && this.SelectedNeeds.length > 0) || this.DetailsMode)
      this.mergeFamilyNeedUpdateMode();
  }

  ngOnChanges(): void {
    if (this.UpdateMode && this.SelectedNeeds.length > 0)
      this.mergeFamilyNeedUpdateMode();
  }

  mergeFamilyNeedUpdateMode() {
    let category = '';
    this.SelectedNeeds.forEach(item => {
      let need = this.FamilyNeeds.find(i => i.value == item.needTypeId);
      if (need)
        category = this.FamilyCategories.find(i => i.value == need.value)?.name;
      if (need && category) {
        item.categoryName = category;
        item.name = need.name;
      }
      if (item.deliveryDate)
        item.deliveryDate = this.datePipe.transform(item.deliveryDate, 'yyyy-MM')
    });
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      categoryId: ['', Validators.required],
      familyNeedId: ['', Validators.required],
      deliveryDate: ['', Validators.required],
      isWaiting: false,
    });

    this.ItemForm.get('isWaiting')?.valueChanges.subscribe((item) => {
      if (item)
        this.formService.updateFieldsRequiredValidation(this.ItemForm, 'deliveryDate', true);
      else
        this.formService.updateFieldsRequiredValidation(this.ItemForm, 'deliveryDate', false);
    });

    this.ItemForm.get('categoryId')?.valueChanges.subscribe((id) => {
      if (id)
        this.FamilyNeedsByCategory = this.FamilyNeeds.filter(i => i.extraData['categoryId'] == id);
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.FamilyNeedsByCategory = this.FamilyNeeds.filter(i => i.extraData['categoryId'] == item.categoryId);
    this.ItemForm.setValue({
      id: item.id,
      categoryId: item?.categoryId,
      familyNeedId: item?.needTypeId,
      deliveryDate: item?.deliveryDate,
      isWaiting: item.isWaiting,
    });
  }

  ResetForm() {
    this.ItemForm.reset();
    this.ItemForm.get('id').setValue(0);
    this.ItemForm.get('isWaiting').setValue(false);
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
    this.SelectedNeedId = itemId;
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
      let checked = this.SelectedNeeds.find(i => i.needTypeId == this.ItemForm?.value?.familyNeedId);
      if (checked) {
        this.toaster.warning('هذا العنصر موجود');
        return;
      }
    }

    const formData = this.ItemForm.value;
    let arryNum = this.SelectedNeeds.map(i => i.id);
    let id = arryNum.length > 0 ? Math.max(...arryNum) : 0;
    if (this.addMode) {
      this.SelectedNeeds.push({
        id: id + 1,
        categoryId: formData.categoryId,
        needTypeId: formData.familyNeedId,
        categoryName: this.FamilyCategories.find(i => i.value == formData.categoryId).name,
        name: this.FamilyNeeds.find(i => i.value == formData.familyNeedId).name,
        deliveryDate: formData.deliveryDate ?? null,
        isWaiting: formData.isWaiting
      });
    } else {
      let obj = this.SelectedNeeds.find(i => i.id == formData.id);
      if (obj) {
        obj.categoryId = formData.categoryId;
        obj.needTypeId = formData.familyNeedId;
        obj.categoryName = this.FamilyCategories.find(i => i.value == formData.categoryId).name;
        obj.name = this.FamilyNeeds.find(i => i.value == formData.familyNeedId).name;
        obj.deliveryDate = formData.deliveryDate ?? null;
        obj.isWaiting = formData.isWaiting;
      }
    }

    this.modalService.dismissAll();
  }

  DeleteItem() {
    this.SelectedNeeds = this.SelectedNeeds.filter(i => i.id != this.SelectedNeedId);
    this.modalService.dismissAll();
  }

  GetOutputData() {
    return this.SelectedNeeds;
  }
}
