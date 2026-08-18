import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ZaInputWithLabelComponent } from "../../../../../../../Shared/za-input-with-label/za-input-with-label.component";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FamilyIncome } from '../../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-family-income-data',
  standalone: true,
  imports: [ZaInputWithLabelComponent, FormsModule],
  templateUrl: './family-income-data.component.html',
  styleUrls: ['../family-step-shared.css', './family-income-data.component.css']
})
export class FamilyIncomeDataComponent {
  @Output() FamilyIncomeChange = new EventEmitter<any>();
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  @Input() FamilyIncome: FamilyIncome = {
    fatherJop: 0,
    motherJop: 0,
    childernsJop: 0,
    affairSpension_SocialSolidarity: 0,
    project: 0,
    liveStock_Lands: 0,
    organization_ZakatCommittee: 0,
    insurancePension: 0,
    other: 0,
    totalFamilyIncome: 0,
    comments: ''
  };

  constructor(private toaster: ToastrService) { }

  onInputChange(value: string, key: any) {
    if (!value)
      value = '0';

    this.FamilyIncome.totalFamilyIncome = 0;
    this.FamilyIncome[key] = value ? Number(value) : 0;
    if (Number(value) > 0)
      Object.entries(this.FamilyIncome).filter(([key, value]) => typeof value === 'number'
        && key != 'id' && key != 'familyStatusId').forEach(([key, value]) => {
          this.FamilyIncome.totalFamilyIncome += value;
        });

    if (this.UpdateMode)
      this.FamilyIncomeChange.emit(this.FamilyIncome.totalFamilyIncome);
  }

  GetOutputData() {
    let arry = Object.entries(this.FamilyIncome).filter(([key, value]) => typeof value === 'number'
      && key != 'id' && key != 'familyStatusId').map(([key, value]) => value)
    let checked = arry.some(value => value > 0);

    if (!checked) {
      this.toaster.warning('برجاء ادخال قيمة واحدة على الأقل');
      return null;
    } else
      return this.FamilyIncome;
  }
}
