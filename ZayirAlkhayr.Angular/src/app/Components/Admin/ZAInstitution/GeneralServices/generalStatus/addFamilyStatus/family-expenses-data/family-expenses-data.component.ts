import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ZaInputWithLabelComponent } from "../../../../../../../Shared/za-input-with-label/za-input-with-label.component";
import { FamilyExpenses, FamilyIncome } from '../../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-family-expenses-data',
  standalone: true,
  imports: [ZaInputWithLabelComponent, FormsModule],
  templateUrl: './family-expenses-data.component.html',
  styleUrl: './family-expenses-data.component.css'
})
export class FamilyExpensesDataComponent implements OnInit, OnChanges {
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  @Input() FamilyIncome: FamilyIncome;
  @Input() FamilyExpenses: FamilyExpenses = {
    rent_Electricity_Water_Gas_Sewage: 0,
    medicalExamination_Treatment: 0,
    installment_debts: 0,
    schoolExpenses: 0,
    physiotherapySessions: 0,
    analysis: 0,
    satisfactoryTransfers: 0,
    medicalXRays: 0,
    isMinisterialSupply: false,
    isFoodBank: false,
    totalFamilyExpenses: 0,
    netFamilyIncome: 0,
    familyCount: 0
  };
  TotalFamilyIncome: number = 0;
  initialNetFamilyIncome = 0;


  constructor(private toaster: ToastrService) { }

  ngOnInit(): void { }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.UpdateMode) {
      this.InetialData(this.FamilyIncome);
      Object.entries(this.FamilyExpenses).filter(([key, value]) => typeof value === 'number'
        && key != 'totalFamilyExpenses' && key != 'netFamilyIncome' && key != 'familyCount'
        && key != 'id' && key != 'familyStatusId').forEach(([key, value]) => {
          this.FamilyExpenses.totalFamilyExpenses += value;
        });
      this.FamilyExpenses.netFamilyIncome -= this.FamilyExpenses.totalFamilyExpenses;
    }
  }

  InetialData(data: FamilyIncome) {
    if (this.UpdateMode)
      this.FamilyExpenses.totalFamilyExpenses = 0;
    this.FamilyExpenses.netFamilyIncome = 0;
    this.initialNetFamilyIncome = 0;
    Object.entries(data).filter(([key, value]) => typeof value === 'number'
      && key != 'id' && key != 'familyStatusId').forEach(([key, value]) => {
        this.FamilyExpenses.netFamilyIncome += value;
        this.initialNetFamilyIncome += value;
      });

    this.FamilyExpenses.netFamilyIncome = this.FamilyExpenses.netFamilyIncome - data.totalFamilyIncome;
    if (!this.UpdateMode)
      this.FamilyExpenses.netFamilyIncome -= this.FamilyExpenses.totalFamilyExpenses;
    this.initialNetFamilyIncome -= data.totalFamilyIncome;
  }

  onInputChange(value: string, key: any) {
    if (!value)
      value = '0';

    this.FamilyExpenses.totalFamilyExpenses = 0;
    let netFamily = 0;
    this.FamilyExpenses[key] = value ? Number(value) : 0;
    if (Number(value) > 0) {
      Object.entries(this.FamilyExpenses).filter(([key, value]) => typeof value === 'number'
        && key != 'totalFamilyExpenses' && key != 'netFamilyIncome' && key != 'familyCount'
        && key != 'id' && key != 'familyStatusId').forEach(([key, value]) => {
          this.FamilyExpenses.totalFamilyExpenses += value;
          netFamily += value
        });
      this.FamilyExpenses.netFamilyIncome = this.initialNetFamilyIncome - netFamily;
    }

  }

  GetOutputData() {
    let arry = Object.entries(this.FamilyExpenses).filter(([key, value]) => typeof value === 'number'
      && key != 'totalFamilyExpenses' && key != 'netFamilyIncome' && key != 'familyCount'
      && key != 'id' && key != 'familyStatusId').map(([key, value]) => value)
    let checked = arry.some(value => value > 0);

    if (!checked) {
      this.toaster.warning('برجاء ادخال قيمة واحدة على الأقل');
      return null;
    } else
      return this.FamilyExpenses;
  }
}
