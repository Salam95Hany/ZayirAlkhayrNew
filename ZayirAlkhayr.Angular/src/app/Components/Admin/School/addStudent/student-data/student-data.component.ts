import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormService } from '../../../../../Services/shared/form.service';
import { ToastrService } from 'ngx-toastr';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaDropDownFormControlComponent } from '../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { CustomValidators, RegexType } from '../../../../../Services/shared/custom-validators';
import { StudentDetails } from '../../../../../Models/school/student/AddStudentModel';
import { FormDropdownModel } from '../../../../../Models/shared/FormDropdownModel';

@Component({
  selector: 'app-student-data',
  standalone: true,
  imports: [NgIf, NgFor, ZaInputWithLabelComponent, ZaDropDownFormControlComponent, ReactiveFormsModule, ZaEmptyDataComponent],
  templateUrl: './student-data.component.html',
  styleUrl: './student-data.component.css'
})
export class StudentDataComponent {
  @Input() StudentDetails: StudentDetails[] = [];
  @Input() AcademicStages: FormDropdownModel[] = [];
  @Input() Nationalities: FormDropdownModel[] = [];
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  @Output() StudentDetailsChange = new EventEmitter<string[]>();
  ItemForm: FormGroup;
  FamilyChildCount = 0;
  StudentDetailsId: number;
  addMode = true;
  formErrors = {
    studentName: '',
    academicStageId: '',
    birthDay: '',
    governmentSchool: '',
    studyPeriod: '',
    nationalityId: '',
    isHaveHealthCondition: '',
    HealthConditionNote: '',
    gender: '',
    academicYear: '',
    studentStatusId: '',
    studentStatusReason: '',
    orderAmongChildren: '',
    nationalId: ''
  };

  constructor(private modalService: NgbModal, private fb: FormBuilder, private formService: FormService,
    private toaster: ToastrService
  ) { }

  ngOnInit(): void {
    this.FormInit();
    this.FamilyChildCount = this.StudentDetails.length;
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      studentName: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      academicStageId: ['', [Validators.required]],
      birthDay: ['', [Validators.required]],
      governmentSchool: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      studyPeriod: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      NationalityId: ['', [Validators.required]],
      isHaveHealthCondition: ['', [Validators.required]],
      HealthConditionNote: [''],
      studyAmount: [''],
      gender: ['', [Validators.required]],
      academicYear: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      studentStatusId: ['', [Validators.required]],
      studentStatusReason: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      orderAmongChildren: ['', [Validators.required]],
      nationalId: ['', [Validators.required]]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });
  }

  FillEditForm(item: any) {
    this.ItemForm.patchValue({
      id: item.id,
      studentName: item.studentName,
      academicStageId: item.academicStageId,
      birthDay: item.birthDay,
      governmentSchool: item.governmentSchool,
      studyPeriod: item.studyPeriod,
      NationalityId: item.NationalityId,
      isHaveHealthCondition: item.isHaveHealthCondition,
      HealthConditionNote: item.HealthConditionNote,
      studyAmount: item.studyAmount,
      gender: item.gender,
      academicYear: item.academicYear,
      studentStatusId: item.studentStatusId,
      studentStatusReason: item.studentStatusReason,
      orderAmongChildren: item.orderAmongChildren,
      nationalId: item.nationalId
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
    this.StudentDetailsId = itemId;
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
      let checked = this.StudentDetails.find(i => i.studentName == this.ItemForm.value.studentName);
      if (checked) {
        this.toaster.warning('هذا الطالب موجود');
        return;
      }
    }

    const formData = this.ItemForm.value;
    let arryNum = this.StudentDetails.map(i => i.id);
    let id = arryNum.length > 0 ? Math.max(...arryNum) : 0;
    if (this.addMode) {
      this.StudentDetails.push({
        id: id + 1,
        studentName: formData.studentName,
        academicStageId: formData.academicStageId,
        birthDay: formData.birthDay,
        governmentSchool: formData.governmentSchool,
        studyPeriod: formData.studyPeriod,
        studyAmount: formData.studyAmount,
        nationalityId: formData.nationalityId,
        isHaveHealthCondition: formData.isHaveHealthCondition,
        healthConditionNote: formData.healthConditionNote,
        gender: formData.gender,
        academicYear: formData.academicYear,
        studentStatusId: formData.studentStatusId,
        studentStatusReason: formData.studentStatusReason,
        orderAmongChildren: formData.orderAmongChildren,
      });
    } else {
      let obj = this.StudentDetails.find(i => i.id == formData.id);
      if (obj) {
        obj.studentName = formData.studentName;
        obj.academicStageId = formData.academicStageId;
        obj.birthDay = formData.birthDay;
        obj.governmentSchool = formData.governmentSchool;
        obj.studyPeriod = formData.studyPeriod;
        obj.studyAmount = formData.studyAmount;
        obj.nationalityId = formData.nationalityId;
        obj.isHaveHealthCondition = formData.isHaveHealthCondition;
        obj.healthConditionNote = formData.healthConditionNote;
        obj.gender = formData.gender;
        obj.academicYear = formData.academicYear;
        obj.studentStatusId = formData.studentStatusId;
        obj.studentStatusReason = formData.studentStatusReason;
        obj.orderAmongChildren = formData.orderAmongChildren;
      }
    }

    this.FamilyChildCount = this.StudentDetails.length;
    if (this.UpdateMode)
      this.StudentDetailsChange.emit(this.StudentDetails.map(i => i.studentName));
    this.modalService.dismissAll();
  }

  DeleteItem() {
    this.StudentDetails = this.StudentDetails.filter(i => i.id != this.StudentDetailsId);
    if (this.UpdateMode)
      this.StudentDetailsChange.emit(this.StudentDetails.map(i => i.studentName));
    this.FamilyChildCount = this.StudentDetails.length;
    this.modalService.dismissAll();
  }

  GetOutputData() {
    if (this.StudentDetails.length > 0) {

      return this.StudentDetails;
    } else
      return [];
  }
}
