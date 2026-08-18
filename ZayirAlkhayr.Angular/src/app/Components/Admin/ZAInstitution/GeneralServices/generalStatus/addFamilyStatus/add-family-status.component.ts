import { NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FamilyStatusComponent } from './family-status/family-status.component';
import { FamilyDataComponent } from './family-data/family-data.component';
import { FamilyIncomeDataComponent } from './family-income-data/family-income-data.component';
import { FamilyExpensesDataComponent } from './family-expenses-data/family-expenses-data.component';
import { FamilyMedicalComponent } from './family-medical/family-medical.component';
import { FamilyNeedComponent } from './family-need/family-need.component';
import { ReviewersComponent } from './reviewers/reviewers.component';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { FamilyStatusLookups } from '../../../../../../Models/zainstitution/GeneralStatus/FamilyStatusLookups';
import { AddFamilyStatusModel } from '../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { GeneralStatusService } from '../../../../../../Services/zainstitution/general-status.service';
import { NgxLoadingModule } from "ngx-loading";
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';

@Component({
  selector: 'app-add-family-status',
  standalone: true,
  imports: [NgFor, NgIf, NgClass, FamilyStatusComponent, FamilyDataComponent, FamilyIncomeDataComponent, FamilyExpensesDataComponent,
    FamilyMedicalComponent, FamilyNeedComponent, ReviewersComponent, NgxLoadingModule, AdminBreadcrumbComponent],
  templateUrl: './add-family-status.component.html',
  styleUrl: './add-family-status.component.css'
})
export class AddFamilyStatusComponent implements OnInit {
  @ViewChild('FamilyStatus') FamilyStatus: FamilyStatusComponent;
  @ViewChild('FamilyData') FamilyData: FamilyDataComponent;
  @ViewChild('FamilyIncome') FamilyIncome: FamilyIncomeDataComponent;
  @ViewChild('FamilyExpenses') FamilyExpenses: FamilyExpensesDataComponent;
  @ViewChild('FamilyMedical') FamilyMedical: FamilyMedicalComponent;
  @ViewChild('FamilyNeed') FamilyNeed: FamilyNeedComponent;
  @ViewChild('Reviewers') Reviewers: ReviewersComponent;
  FamilyLookups: FamilyStatusLookups = {} as FamilyStatusLookups;
  AddFamilyStatusModel: AddFamilyStatusModel = {} as AddFamilyStatusModel;
  showLoader = false;
  isLookupsLoaded = false;
  StepName: string;
  activeStep = 1;
  Counter = 0;
  viewChilds = [];
  Steps: any[] = [];
  StepList: any[] = [
    { stepName: 'بيانات الحالة', stepDescription: 'البيانات الأساسية ووسائل التواصل', stepId: 'familyStatus', number: 1, icon: 'fa-solid fa-address-card' },
    { stepName: 'أفراد الأسرة', stepDescription: 'إضافة أفراد الأسرة وبياناتهم', stepId: 'familyDetails', number: 2, icon: 'fa-solid fa-people-roof' },
    { stepName: 'دخل الأسرة', stepDescription: 'مصادر الدخل والقيمة الشهرية', stepId: 'familyIncome', number: 3, icon: 'fa-solid fa-wallet' },
    { stepName: 'مصروفات الأسرة', stepDescription: 'تفاصيل الالتزامات والمصروفات', stepId: 'familyExpenses', number: 4, icon: 'fa-solid fa-receipt' },
    { stepName: 'الجانب الطبي', stepDescription: 'الحالة الصحية لأفراد الأسرة', stepId: 'familyPatient', number: 5, icon: 'fa-solid fa-heart-pulse' },
    { stepName: 'الاحتياجات', stepDescription: 'تحديد احتياجات الحالة وأولوياتها', stepId: 'familyNeeds', number: 6, icon: 'fa-solid fa-hand-holding-heart' },
    { stepName: 'المراجعة', stepDescription: 'ملاحظات الباحث والمراجعة النهائية', stepId: 'familyExtraDetails', number: 7, icon: 'fa-solid fa-clipboard-check' }
  ]
  readonly TitleList = [
    { label: 'مؤسسة زائر الخير', route: '/admin/za-institution/home' },
    { label: 'الحالات العامة', route: '/admin/za-institution/family-status' },
    'إضافة حالة جديدة'
  ];

  get progressPercentage(): number {
    return Math.round(((this.activeStep - 1) / (this.StepList.length - 1)) * 100);
  }

  constructor(private generalStatusService: GeneralStatusService, private toaster: ToastrService,
    private router: Router
  ) {
    this.Steps = this.StepList.map(a => a.stepId);
    this.StepName = this.Steps[0];
  }

  ngOnInit(): void {
    this.GetFamilyStatusLookups();
  }

  GetFamilyStatusLookups() {
    this.showLoader = true;
    this.generalStatusService.GetFamilyStatusLookups().subscribe(data => {
      this.showLoader = false;
      this.FamilyLookups = data.results;
      this.isLookupsLoaded = true;
    });
  }

  NextStep() {
    this.viewChilds = [this.FamilyStatus, this.FamilyData, this.FamilyIncome, this.FamilyExpenses, this.FamilyMedical, this.FamilyNeed, this.Reviewers]
    let data = this.viewChilds[this.Counter].GetOutputData();
    if (this.Counter == 0) {
      if (data == null)
        return;
      this.showLoader = true;
      this.showLoader = false;
      this.AddFamilyStatusModel[this.StepName] = data;
      this.Counter++;
      this.activeStep++;
      this.StepName = this.Steps[this.Counter];
      this.AddFamilyStatusModel = { ...this.AddFamilyStatusModel };
      if (this.viewChilds[this.Counter]?.InetialData)
        this.viewChilds[this.Counter].InetialData(data);
    } else {
      if (data !== null) {
        this.AddFamilyStatusModel[this.StepName] = data;
        this.Counter++;
        this.activeStep++;
        this.StepName = this.Steps[this.Counter];
        this.AddFamilyStatusModel = { ...this.AddFamilyStatusModel };
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

  AddNewFamilyStatus() {
    let data = this.viewChilds[this.Counter].GetOutputData();
    this.AddFamilyStatusModel[this.StepName] = data;
    this.showLoader = true;
    this.generalStatusService.AddNewFamilyStatus(this.AddFamilyStatusModel).subscribe(data => {
      this.showLoader = false;
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.router.navigateByUrl('admin/za-institution/family-status');
      } else
        this.toaster.error(data.message);
    });
  }
}
