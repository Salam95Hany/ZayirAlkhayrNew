import { NgIf } from '@angular/common';
import { Component, ElementRef, Injector, Input, ViewChild } from '@angular/core';
import { NgxLoadingModule } from 'ngx-loading';
import { SchoolStudentService } from '../../../../Services/school/school-student.service';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { AddStudentModel, ParentStudent, StudentDetails, StudentDiscount } from '../../../../Models/school/student/AddStudentModel';
import { ParentDataComponent } from '../addStudent/parent-data/parent-data.component';
import { StudentDataComponent } from '../addStudent/student-data/student-data.component';
import { DiscountDataComponent } from '../addStudent/discount-data/discount-data.component';
import { AuthService } from '../../../../Auth/auth.service';
import { FormDropdownModel } from '../../../../Models/shared/FormDropdownModel';

@Component({
  selector: 'app-student-sidepanel',
  standalone: true,
  imports: [NgIf, NgxLoadingModule, ParentDataComponent, DiscountDataComponent, StudentDataComponent],
  templateUrl: './student-sidepanel.component.html',
  styleUrl: './student-sidepanel.component.css'
})
export class StudentSidepanelComponent {
  @ViewChild('sidepanelEditStatus') sidepanelEditStatus: ElementRef;
  @ViewChild('ParentData') ParentData: ParentDataComponent;
  @ViewChild('StudentData') StudentData: StudentDataComponent;
  @ViewChild('DiscountData') DiscountData: DiscountDataComponent;
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
  StudentNames: string[] = [];
  StepList: any[] = [
    { stepId: 'ParentData', number: 0, scrollTop: 0 },
    { stepId: 'StudentData', number: 1 },
    { stepId: 'DiscountData', number: 2, scrollTop: 900 }
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
      let viewChilds = [this.ParentData, this.StudentData, this.DiscountData]
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


  BindStudentModel() {
    let academicStageName = this.StudentInfo.lookups.academicStages.find(i => i.value == this.StudentInfo.student.academicStageId)?.name;
    let nationalityName = this.StudentInfo.lookups.nationalities.find(i => i.value == this.StudentInfo.student.nationalityId)?.name;
    let studyPeriodName = this.StudyPeriods.find(i => i.value == this.StudentInfo.student.studyPeriod)?.name;
    let genderName = this.Genders.find(i => i.value == this.StudentInfo.student.gender)?.name;
    
    let parentStudent: ParentStudent = {
      parentId: this.StudentInfo.parent.id,
      address: this.StudentInfo.parent.address,
      phone: this.StudentInfo.parent.phone,
      parentName: this.StudentInfo.parent.name,
      childrenCount: this.StudentInfo.student.childrenCount,
      insertUser: this.authService.userId
    };

    let student: StudentDetails = {
      id: 1,
      studentId: +this.StudentInfo.student.id,
      studentName: this.StudentInfo.student.studentName,
      academicStageId: this.StudentInfo.student.academicStageId?.toString(),
      academicStageName: academicStageName,
      birthDay: this.StudentInfo.student.birthDay,
      governmentSchool: this.StudentInfo.student.governmentSchool,
      studyPeriod: +this.StudentInfo.student.studyPeriod,
      studyPeriodName: studyPeriodName,
      nationalityId: this.StudentInfo.student.nationalityId?.toString(),
      nationalityName: nationalityName,
      isHaveHealthCondition: this.StudentInfo.student.isHaveHealthCondition,
      healthConditionNote: this.StudentInfo.student.healthConditionNote,
      gender: +this.StudentInfo.student.gender,
      genderName: genderName,
      academicYear: this.StudentInfo.student.academicYear,
      studyAmount: this.StudentInfo.student.studyAmount,
      studentStatusId: +this.StudentInfo.student.studentStatusId,
      studentStatusReason: this.StudentInfo.student.studentStatusReason,
      orderAmongChildren: this.StudentInfo.student.orderAmongChildren
    };

    let studentDiscount: StudentDiscount = {
      studentName: this.StudentInfo.student.studentName,
      discountTypeId: this.StudentInfo.student.discountTypeId?.toString(),
      discountReason: this.StudentInfo.student.discountReason,
      discountAmount: this.StudentInfo.student.discountAmount,
    };

    this.AddStudentModel.parentStudent = parentStudent;
    this.AddStudentModel.student = [student];
    this.AddStudentModel.discount = [studentDiscount];
  }

  StudendChange(item: any[]) {
    this.StudentNames = item;
  }
}
