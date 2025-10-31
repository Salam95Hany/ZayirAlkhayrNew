import { Component, ElementRef, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { FamilyStatusComponent } from "../addFamilyStatus/family-status/family-status.component";
import { FamilyDataComponent } from "../addFamilyStatus/family-data/family-data.component";
import { FamilyIncomeDataComponent } from "../addFamilyStatus/family-income-data/family-income-data.component";
import { FamilyExpensesDataComponent } from "../addFamilyStatus/family-expenses-data/family-expenses-data.component";
import { FamilyMedicalComponent } from "../addFamilyStatus/family-medical/family-medical.component";
import { FamilyNeedComponent } from "../addFamilyStatus/family-need/family-need.component";
import { ReviewersComponent } from "../addFamilyStatus/reviewers/reviewers.component";
import { AddFamilyStatusModel, FamilyDetails } from '../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { GeneralStatusService } from '../../../../../../Services/zainstitution/general-status.service';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { NgIf } from '@angular/common';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-family-status-sidepanel',
  standalone: true,
  imports: [FamilyStatusComponent, FamilyDataComponent, FamilyIncomeDataComponent, FamilyExpensesDataComponent,
    FamilyMedicalComponent, FamilyNeedComponent, ReviewersComponent, NgIf, NgxLoadingModule],
  templateUrl: './family-status-sidepanel.component.html',
  styleUrl: './family-status-sidepanel.component.css'
})
export class FamilyStatusSidepanelComponent implements OnInit {
@ViewChild('sidepanelEditStatus') sidepanelEditStatus: ElementRef;
  @ViewChild('FamilyStatus') FamilyStatus: FamilyStatusComponent;
  @ViewChild('FamilyData') FamilyData: FamilyDataComponent;
  @ViewChild('FamilyIncome') FamilyIncome: FamilyIncomeDataComponent;
  @ViewChild('FamilyExpenses') FamilyExpenses: FamilyExpensesDataComponent;
  @ViewChild('FamilyMedical') FamilyMedical: FamilyMedicalComponent;
  @ViewChild('FamilyNeed') FamilyNeed: FamilyNeedComponent;
  @ViewChild('Reviewers') Reviewers: ReviewersComponent;
  @Input() FamilyStatusId: any;
  @Input() FamilyStatusCode: any;
  @Input() FamilyStatusName: any;
  @Input() UpdateMode: any;
  @Input() DetailsMode: any;
  AddFamilyStatusModel: AddFamilyStatusModel = {} as AddFamilyStatusModel;
  FamilyStatusInfo: any;
  showLoader = false;
  isLookupsLoaded = false;
  StepList: any[] = [
    { stepId: 'familyStatus', number: 0, scrollTop: 0 },
    { stepId: 'familyDetails', number: 1 },
    { stepId: 'familyIncome', number: 2, scrollTop: 900 },
    { stepId: 'familyExpenses', number: 3, scrollTop: 1500 },
    { stepId: 'familyPatient', number: 4 },
    { stepId: 'familyNeeds', number: 5 },
    { stepId: 'familyExtraDetails', number: 6 }
  ]

  constructor(private generalService: GeneralStatusService, private offcanvasService: NgbOffcanvas, private injector: Injector
    , private toaster: ToastrService) { }

  ngOnInit(): void {
    this.FamilyStatusId = this.injector.get('FamilyStatusId');
    this.FamilyStatusCode = this.injector.get('FamilyStatusCode');
    this.FamilyStatusName = this.injector.get('FamilyStatusName');
    this.UpdateMode = this.injector.get('UpdateMode');
    this.DetailsMode = this.injector.get('DetailsMode');
    this.GetUpdateFamilyStatusLookups();
  }

  GetUpdateFamilyStatusLookups() {
    this.showLoader = true;
    this.generalService.GetUpdateFamilyStatusLookups(this.FamilyStatusId).subscribe(data => {
      this.showLoader = false;
      this.FamilyStatusInfo = data.results;
      this.isLookupsLoaded = true;
      console.log(this.FamilyStatusInfo);
    });
  }

  dismissSidePanel() {
    this.offcanvasService.dismiss();
  }

  StatusNameChanged(name: string) {
    this.FamilyStatusInfo.familyStatus.name = name;
  }

  FamilyIncomeChange(totalFamilyIncome: any) {
    this.FamilyStatusInfo.familyStatus.familyIncomes.totalFamilyIncome = totalFamilyIncome;
    this.FamilyStatusInfo.familyStatus.familyIncomes = { ...this.FamilyStatusInfo.familyStatus.familyIncomes };
  }

  FamilyDetailsChange(item: FamilyDetails[]) {
    this.FamilyStatusInfo.familyStatus.familyDetails = [...item];
  }


  UpdateFamilyStatus() {
    for (const step of this.StepList) {
      let viewChilds = [this.FamilyStatus, this.FamilyData, this.FamilyIncome, this.FamilyExpenses, this.FamilyMedical, this.FamilyNeed, this.Reviewers]
      let data = viewChilds[step.number].GetOutputData();
      if (!data) {
        this.sidepanelEditStatus.nativeElement.scrollTop = step.scrollTop;
        return;
      }

      this.AddFamilyStatusModel[step.stepId] = data;
    }

    this.showLoader = true;
    this.generalService.UpdateFamilyStatus(this.AddFamilyStatusModel).subscribe(data => {
      this.showLoader = false;
      if (data.isSuccess) {
        this.offcanvasService.dismiss({ reload: 'reload' });
        this.toaster.success(data.message);
      } else
        this.toaster.error(data.message);
    });
  }
}
