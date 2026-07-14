import { NgClass, NgFor, NgIf } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { NgxLoadingModule } from 'ngx-loading';
import { SchoolStudentService } from '../../../../Services/school/school-student.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { ParentDataComponent } from './parent-data/parent-data.component';
import { StudentDataComponent } from './student-data/student-data.component';
import { DiscountDataComponent } from './discount-data/discount-data.component';
import { StudentLookups } from '../../../../Models/school/student/StudentLookups';
import { AddStudentModel } from '../../../../Models/school/student/AddStudentModel';

@Component({
  selector: 'app-add-student',
  standalone: true,
  imports: [NgFor, NgIf, NgClass, NgxLoadingModule, ParentDataComponent, StudentDataComponent, DiscountDataComponent],
  templateUrl: './add-student.component.html',
  styleUrl: './add-student.component.css'
})
export class AddStudentComponent {
  @ViewChild('ParentData') ParentData: ParentDataComponent;
  @ViewChild('StudentData') StudentData: StudentDataComponent;
  @ViewChild('DiscountData') DiscountData: DiscountDataComponent;
  StudentLookups: StudentLookups = {} as StudentLookups;
  AddStudentModel: AddStudentModel = {} as AddStudentModel;
  showLoader = false;
  isLookupsLoaded = false;
  StepName: string;
  activeStep = 1;
  Counter = 0;
  viewChilds = [];
  Steps: any[] = [];
  StepList: any[] = [
    { stepName: 'بيانات ولي الأمر', stepId: 'ParentData', number: 1 },
    { stepName: 'بيانات الطالب', stepId: 'StudentData', number: 2 },
    { stepName: 'بيانات الخصم', stepId: 'DiscountData', number: 3 }
  ]

  constructor(private schoolStudentService: SchoolStudentService, private toaster: ToastrService, private router: Router
  ) {
    this.Steps = this.StepList.map(a => a.stepId);
    this.StepName = this.Steps[0];
  }

  ngOnInit(): void {
    this.GetStudentLookups();
  }

  GetStudentLookups() {
    this.showLoader = true;
    this.schoolStudentService.GetStudentLookups().subscribe(data => {
      this.showLoader = false;
      this.StudentLookups = data.results;
      this.isLookupsLoaded = true;
    });
  }

  NextStep() {
    this.viewChilds = [this.ParentData, this.StudentData, this.DiscountData]
    let data = this.viewChilds[this.Counter].GetOutputData();
    if (this.Counter == 0) {
      if (data == null)
        return;
      this.showLoader = true;
      this.showLoader = false;
      this.AddStudentModel[this.StepName] = data;
      this.Counter++;
      this.activeStep++;
      this.StepName = this.Steps[this.Counter];
      this.AddStudentModel = { ...this.AddStudentModel };
      if (this.viewChilds[this.Counter]?.InetialData)
        this.viewChilds[this.Counter].InetialData(data);
    } else {
      if (data !== null) {
        this.AddStudentModel[this.StepName] = data;
        this.Counter++;
        this.activeStep++;
        this.StepName = this.Steps[this.Counter];
        this.AddStudentModel = { ...this.AddStudentModel };
        if (this.viewChilds[this.Counter]?.InetialData)
          this.viewChilds[this.Counter].InetialData(data);
      }
    }
  }

  PreviousSteps() {
    if (this.Counter == 0) {
      return;
    } else {
      this.Counter--;
      this.activeStep--;
      this.StepName = this.Steps[this.Counter];
    }
  }

  AddNewStudent() {
    let data = this.viewChilds[this.Counter].GetOutputData();
    this.AddStudentModel[this.StepName] = data;
    console.log(this.AddStudentModel);
    return;
    this.showLoader = true;
    this.schoolStudentService.AddNewStudent(this.AddStudentModel).subscribe(data => {
      this.showLoader = false;
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.router.navigateByUrl('admin/za-institution/family-status');
      } else
        this.toaster.error(data.message);
    });
  }
}
