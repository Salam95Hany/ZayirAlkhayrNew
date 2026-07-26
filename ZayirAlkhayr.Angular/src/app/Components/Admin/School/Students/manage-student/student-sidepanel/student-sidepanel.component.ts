import { NgIf } from '@angular/common';
import { Component, ElementRef, Injector, Input, ViewChild } from '@angular/core';
import { NgxLoadingModule } from 'ngx-loading';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ParentDataComponent } from '../add-student/parent-data/parent-data.component';
import { StudentDataComponent } from '../add-student/student-data/student-data.component';
import { AddStudentModel, StudentDetails } from '../../../../../../Models/school/student/AddStudentModel';
import { FormDropdownModel } from '../../../../../../Models/shared/FormDropdownModel';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';
import { AuthService } from '../../../../../../Auth/auth.service';

@Component({
  selector: 'app-student-sidepanel',
  standalone: true,
  imports: [NgIf, NgxLoadingModule, ParentDataComponent, StudentDataComponent],
  templateUrl: './student-sidepanel.component.html',
  styleUrl: './student-sidepanel.component.css'
})
export class StudentSidepanelComponent {
  @ViewChild('sidepanelEditStatus') sidepanelEditStatus: ElementRef;
  @ViewChild('ParentData') ParentData: ParentDataComponent;
  @ViewChild('StudentData') StudentData: StudentDataComponent;
  // @ViewChild('DiscountData') DiscountData: DiscountDataComponent;
  @Input() StudentId: any;
  @Input() ParentId: any;
  @Input() StudentName: any;
  @Input() AcademicStage: any;
  @Input() UpdateMode: any;
  @Input() DetailsMode: any;
  AddStudentModel: AddStudentModel = {} as AddStudentModel;
  StudentInfo: any;
  showLoader = false;
  isLookupsLoaded = false;
  StudentDetails: StudentDetails[] = [];
  StepList: any[] = [
    { stepId: 'ParentData', number: 0, scrollTop: 0 },
    { stepId: 'StudentData', number: 1 },
    // { stepId: 'DiscountData', number: 2, scrollTop: 900 }
  ];
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

  constructor(private schoolService: SchoolStudentService, private offcanvasService: NgbOffcanvas, private injector: Injector, private toaster: ToastrService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.StudentId = this.injector.get('StudentId');
    this.ParentId = this.injector.get('ParentId');
    this.StudentName = this.injector.get('StudentName');
    this.AcademicStage = this.injector.get('AcademicStage');
    this.UpdateMode = this.injector.get('UpdateMode');
    this.DetailsMode = this.injector.get('DetailsMode');
    this.GetUpdateStudentLookups();
  }

  GetUpdateStudentLookups() {
    this.showLoader = true;
    this.schoolService.GetUpdateStudentLookups(this.StudentId, this.ParentId).subscribe(data => {
      this.showLoader = false;
      this.StudentInfo = data.results;
      this.BindStudentModel();
      this.isLookupsLoaded = true;
    });
  }

  dismissSidePanel() {
    this.offcanvasService.dismiss();
  }


  UpdateStudent() {
    for (const step of this.StepList) {
      let viewChilds = [this.ParentData, this.StudentData]
      let data = viewChilds[step.number].GetOutputData();
      if (!data) {
        this.sidepanelEditStatus.nativeElement.scrollTop = step.scrollTop;
        return;
      }

      this.AddStudentModel[step.stepId] = data;
    }

    this.showLoader = true;
    this.schoolService.UpdateStudent(this.AddStudentModel).subscribe(data => {
      this.showLoader = false;
      if (data.isSuccess) {
        this.offcanvasService.dismiss({ reload: 'reload' });
        this.toaster.success(data.message);
      } else
        this.toaster.error(data.message);
    });
  }


  BindStudentModel(): void {

    const student = this.StudentInfo.student;
    const parent = this.StudentInfo.parent;
    const enrollment = student.studentEnrollments?.[0];

    if (!student || !parent || !enrollment) {
      return;
    }

    const academicStageName = this.StudentInfo.lookups.academicStages.find(x => +x.value === enrollment.academicStageId)?.name ?? '';
    const nationalityName = this.StudentInfo.lookups.nationalities.find(x => +x.value === student.nationalityId)?.name ?? '';
    const studyPeriodName = this.StudyPeriods.find(x => +x.value === enrollment.studyPeriodId)?.name ?? '';
    const genderName = this.Genders.find(x => +x.value === +student.gender)?.name ?? '';
    const studentTypeName = this.StudentInfo.lookups.studentTypes.find(x => +x.value === +student.studentTypeId)?.name ?? '';

    this.AddStudentModel.parentStudent = {
      parentId: parent.id,
      parentName: parent.name,
      address: parent.address,
      fatherPhone: parent.parentPhone,
      motherPhone: parent.motherPhone,
      whatsappNumber: parent.whatsappNumber,
      insertUser: this.authService.userId
    };

    this.AddStudentModel.student = [{
      id: 1,
      studentId: student.id,
      studentName: student.studentName,
      academicStageId: enrollment.academicStageId,
      academicStageName,
      birthDay: student.birthDay,
      governmentSchool: student.governmentSchool,
      studyPeriodId: enrollment.studyPeriodId,
      studyPeriodName,
      nationalityId: student.nationalityId,
      nationalityName,
      studentTypeId: student.studentTypeId,
      studentTypeName,
      isHaveHealthCondition: student.isHaveHealthCondition,
      healthConditionNote: student.healthConditionNote,
      gender: +student.gender,
      genderName,
      academicYear: enrollment.academicYear.name,
      academicYearId: enrollment.academicYearId,
      studentStatusId: enrollment.studentStatusId,
      studentStatusReason: enrollment.studentStatusReason,
      orderAmongChildren: student.orderAmongChildren,
      enrollmentDate: enrollment.enrollmentDate,
      notes: enrollment.notes ?? ''
    }];

    // const feeTemplate = this.StudentInfo.lookups.feeTemplates.find(x => x.academicStageId === enrollment.academicStageId);
    // this.AddStudentModel.discount = [{
    //   studentName: student.studentName,
    //   academicStageName,
    //   academicYear: enrollment.academicYear.name,
    //   studyAmount: feeTemplate?.amount ?? 0,
    //   discountTypeId: enrollment.discountTypeId,
    //   discountReason: enrollment.discountReason,
    //   discountAmount: enrollment.discountAmount,
    //   notes: enrollment.notes
    // }];

    this.StudentDetails = [...this.AddStudentModel.student];
  }

  // StudendChange(item: StudentDetails[]) {
  //   this.StudentDetails = [...item];
  // }
}
