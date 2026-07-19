import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { NgFor, NgIf } from '@angular/common';
import { ZaInputWithLabelComponent } from '../../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { ZaDropDownFormControlComponent } from '../../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component';
import { ZaEmptyDataComponent } from '../../../../../../../Shared/za-empty-data/za-empty-data.component';
import { ArabicDateWithTimePipe } from '../../../../../../../Pipes/arabic-date-with-time.pipe';
import { StudentDetails } from '../../../../../../../Models/school/student/AddStudentModel';
import { FormDropdownModel } from '../../../../../../../Models/shared/FormDropdownModel';
import { FormService } from '../../../../../../../Services/shared/form.service';
import { CustomValidators, RegexType } from '../../../../../../../Services/shared/custom-validators';

@Component({
  selector: 'app-student-data',
  standalone: true,
  imports: [NgIf, NgFor, ZaInputWithLabelComponent, ZaDropDownFormControlComponent, ReactiveFormsModule, ZaEmptyDataComponent, FormsModule,
    ArabicDateWithTimePipe
  ],
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
  studentStatus: FormDropdownModel[] = [
    { value: 1, name: 'موجود' },
    { value: 2, name: 'منسحب' }
  ];
  Genders: FormDropdownModel[] = [
    { value: 1, name: 'ذكر' },
    { value: 2, name: 'أنثى' }
  ];
  StudyPeriods: FormDropdownModel[] = [
    { value: 1, name: 'صباحي' },
    { value: 2, name: 'مسائي' }
  ];
  ItemForm: FormGroup;
  FamilyChildCount = 0;
  TotalStudyAmount = 0;
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
    healthConditionNote: '',
    gender: '',
    academicYear: '',
    studentStatusId: '',
    studentStatusReason: '',
    orderAmongChildren: ''
  };

  constructor(private modalService: NgbModal, private fb: FormBuilder, private formService: FormService,
    private toaster: ToastrService
  ) { }

  ngOnInit(): void {
    this.FormInit();
    this.FamilyChildCount = this.StudentDetails.length;
    this.TotalStudyAmount = this.StudentDetails.reduce((total, student) => total + (student.studyAmount || 0), 0);
  }

  FormInit() {
    this.ItemForm = this.fb.group({
      id: 0,
      studentName: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      academicStageId: ['', [Validators.required]],
      birthDay: ['', [Validators.required, CustomValidators.ageBetween(4, 14)]],
      governmentSchool: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      studyPeriod: ['', [Validators.required]],
      nationalityId: ['', [Validators.required]],
      isHaveHealthCondition: [''],
      healthConditionNote: [''],
      gender: ['', [Validators.required]],
      academicYear: ['', [Validators.required, CustomValidators.regexPattern(RegexType.academicYear)]],
      studentStatusId: ['', [Validators.required]],
      studentStatusReason: ['', [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]],
      orderAmongChildren: ['', [Validators.required]]
    });

    this.ItemForm.valueChanges.subscribe((data) => {
      this.formErrors = this.formService.validateForm(this.ItemForm, this.formErrors, true);
    });

    const studentStatusControl = this.ItemForm.get('studentStatusId');
    studentStatusControl?.valueChanges.subscribe((value) => {
      if (value == 2) {
        this.formService.updateFieldValidators(this.ItemForm, 'studentStatusReason', true, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]);
      } else {
        this.formService.updateFieldValidators(this.ItemForm, 'studentStatusReason', false, []);
      }
    });

    const isHaveHealthConditionControl = this.ItemForm.get('isHaveHealthCondition');
    isHaveHealthConditionControl?.valueChanges.subscribe((value) => {
      if (value) {
        this.formService.updateFieldValidators(this.ItemForm, 'healthConditionNote', true, [Validators.required, CustomValidators.regexPattern(RegexType.noSpace)]);
      } else {
        this.formService.updateFieldValidators(this.ItemForm, 'healthConditionNote', false, []);
      }
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
      nationalityId: item.nationalityId,
      isHaveHealthCondition: item.isHaveHealthCondition,
      HealthConditionNote: item.HealthConditionNote,
      gender: item.gender,
      academicYear: item.academicYear,
      studentStatusId: item.studentStatusId,
      studentStatusReason: item.studentStatusReason,
      orderAmongChildren: item.orderAmongChildren,
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

    if (this.addMode && this.StudentDetails.some(x => x.orderAmongChildren == this.ItemForm.value.orderAmongChildren)) {
      this.toaster.warning('هذا الترتيب مستخدم بالفعل لأحد الأشقاء.');
      return;
    }

    if (!this.addMode && this.StudentDetails.some(x => x.orderAmongChildren == this.ItemForm.value.orderAmongChildren && x.id != this.ItemForm.value.id)) {
      this.toaster.warning('هذا الترتيب مستخدم بالفعل لأحد الأشقاء.');
      return;
    }

    const formData = this.ItemForm.value;
    let academicStageName = this.AcademicStages.find(i => i.value == formData.academicStageId)?.name;
    let nationalityName = this.Nationalities.find(i => i.value == formData.nationalityId)?.name;
    let studyPeriodName = this.StudyPeriods.find(i => i.value == formData.studyPeriod)?.name;
    let genderName = this.Genders.find(i => i.value == formData.gender)?.name;
    let arryNum = this.StudentDetails.map(i => i.id);
    let id = arryNum.length > 0 ? Math.max(...arryNum) : 0;
    if (this.addMode) {
      this.StudentDetails.push({
        id: id + 1,
        studentName: formData.studentName,
        academicStageId: formData.academicStageId,
        academicStageName: academicStageName,
        birthDay: formData.birthDay,
        governmentSchool: formData.governmentSchool,
        studyPeriod: formData.studyPeriod,
        studyPeriodName: studyPeriodName,
        studyAmount: this.AcademicStages.find(i => i.value == formData.academicStageId)?.extraData['amount'],
        nationalityId: formData.nationalityId,
        nationalityName: nationalityName,
        isHaveHealthCondition: formData.isHaveHealthCondition,
        healthConditionNote: formData.healthConditionNote,
        gender: formData.gender,
        genderName: genderName,
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
        obj.academicStageName = academicStageName;
        obj.birthDay = formData.birthDay;
        obj.governmentSchool = formData.governmentSchool;
        obj.studyPeriod = formData.studyPeriod;
        obj.studyPeriodName = studyPeriodName;
        obj.studyAmount = this.AcademicStages.find(i => i.value == formData.academicStageId)?.extraData['amount'];
        obj.nationalityId = formData.nationalityId;
        obj.nationalityName = nationalityName;
        obj.isHaveHealthCondition = formData.isHaveHealthCondition;
        obj.healthConditionNote = formData.healthConditionNote;
        obj.gender = formData.gender;
        obj.genderName = genderName;
        obj.academicYear = formData.academicYear;
        obj.studentStatusId = formData.studentStatusId;
        obj.studentStatusReason = formData.studentStatusReason;
        obj.orderAmongChildren = formData.orderAmongChildren;
      }
    }

    this.FamilyChildCount = this.StudentDetails.length;
    if (this.UpdateMode)
      this.StudentDetailsChange.emit(this.StudentDetails.map(i => i.studentName));
    this.TotalStudyAmount = this.StudentDetails.reduce((total, student) => total + (student.studyAmount || 0), 0);
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
    if (this.StudentDetails.length == 0) {
      this.toaster.warning('أدخل بيانات طالب على الأقل');
      return null;
    }


    return this.StudentDetails;
  }
}
