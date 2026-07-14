import { NgIf } from '@angular/common';
import { Component, ElementRef, Injector, Input, ViewChild } from '@angular/core';
import { NgxLoadingModule } from 'ngx-loading';
import { SchoolStudentService } from '../../../../Services/school/school-student.service';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { AddStudentModel } from '../../../../Models/school/student/AddStudentModel';
import { ParentDataComponent } from '../addStudent/parent-data/parent-data.component';
import { StudentDataComponent } from '../addStudent/student-data/student-data.component';
import { DiscountDataComponent } from '../addStudent/discount-data/discount-data.component';

@Component({
  selector: 'app-student-sidepanel',
  standalone: true,
  imports: [NgIf, NgxLoadingModule, ParentDataComponent, DiscountDataComponent],
  templateUrl: './student-sidepanel.component.html',
  styleUrl: './student-sidepanel.component.css'
})
export class StudentSidepanelComponent {
  @ViewChild('sidepanelEditStatus') sidepanelEditStatus: ElementRef;
  @ViewChild('ParentData') ParentData: ParentDataComponent;
  @ViewChild('StudentData') StudentData: StudentDataComponent;
  @ViewChild('DiscountData') DiscountData: DiscountDataComponent;
  @Input() StudentId: any;
  @Input() StudentName: any;
  @Input() AcademicStage: any;
  @Input() UpdateMode: any;
  @Input() DetailsMode: any;
  AddStudentModel: AddStudentModel = {} as AddStudentModel;
  StudentInfo: any;
  showLoader = false;
  isLookupsLoaded = false;
  StepList: any[] = [
    { stepId: 'ParentData', number: 0, scrollTop: 0 },
    { stepId: 'StudentData', number: 1 },
    { stepId: 'DiscountData', number: 2, scrollTop: 900 }
  ]

  constructor(private schoolService: SchoolStudentService, private offcanvasService: NgbOffcanvas, private injector: Injector, private toaster: ToastrService) { }

  ngOnInit(): void {
    this.StudentId = this.injector.get('StudentId');
    this.StudentName = this.injector.get('StudentName');
    this.AcademicStage = this.injector.get('AcademicStage');
    this.UpdateMode = this.injector.get('UpdateMode');
    this.DetailsMode = this.injector.get('DetailsMode');
    this.GetUpdateStudentLookups();
  }

  GetUpdateStudentLookups() {
    this.showLoader = true;
    this.schoolService.GetUpdateStudentLookups(this.StudentId).subscribe(data => {
      this.showLoader = false;
      this.StudentInfo = data.results;
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
}
