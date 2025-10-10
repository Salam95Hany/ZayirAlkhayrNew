import { Component, Input, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ZaInputWithLabelComponent } from '../../../../../../../Shared/za-input-with-label/za-input-with-label.component';
import { FamilyExtraDetails } from '../../../../../../../Models/zainstitution/GeneralStatus/AddFamilyStatusModel';
import { DatePipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-reviewers',
  standalone: true,
  imports: [FormsModule, ZaInputWithLabelComponent,NgFor],
  templateUrl: './reviewers.component.html',
  styleUrl: './reviewers.component.css',
  providers: [DatePipe]
})
export class ReviewersComponent {
  @Input() ExtraDetails: FamilyExtraDetails = {} as FamilyExtraDetails;
  @Input() UpdateMode = false;
  @Input() DetailsMode = false;
  isDate = false;
  PersonalPearpers: any[] = [
    { id: 1, name: 'صور بطاقات الرقم القومي لأفراد الاسرة (جواز سفر لغير المصري)', isSelected: false },
    { id: 2, name: 'صور شهادات الميلاد للقصر', isSelected: false },
    { id: 3, name: 'صورة من عقد الشقة (ايجار - تمليك)', isSelected: false },
    { id: 4, name: 'صورة من قسيمة الزواج', isSelected: false },
    { id: 5, name: 'صورة من الشهادات (الوفاة - الطلاق)', isSelected: false },
    { id: 6, name: 'صور الروشتات والتقارير الطبية', isSelected: false },
    { id: 7, name: 'ايصالات (مياه - كهرباء - غاز)', isSelected: false },
    { id: 8, name: 'صورة من (التأمينات - الشؤون)', isSelected: false },
    { id: 9, name: 'بيان طالب للاولاد بالمدرسة او الحضانة', isSelected: false },
  ];

  constructor(private datePipe: DatePipe) {

  }

  ngOnInit(): void { }

  ngOnChanges(changes: SimpleChanges): void {
    if ((this.UpdateMode && this.ExtraDetails) || (this.DetailsMode && this.ExtraDetails)) {
      if (this.ExtraDetails.lastVisitDate) {
        this.isDate = true;
        this.ExtraDetails.lastVisitDate = this.datePipe.transform(this.ExtraDetails.lastVisitDate, 'yyyy-MM-dd');
      }
      this.ExtraDetails.personalPapers?.split(';;;').forEach(item => {
        let obj = this.PersonalPearpers.find(i => i.id == item);
        if (obj)
          obj.isSelected = true;
      });
    } else {
      this.ExtraDetails = {} as FamilyExtraDetails;
      this.isDate = false;
      this.PersonalPearpers.forEach(i => i.isSelected = false);
    }
  }

  GetOutputData() {
    this.ExtraDetails.personalPapers = this.PersonalPearpers.filter(i => i.isSelected).map(i => i.id).join(';;;');
    let isObjEmpty = Object.keys(this.ExtraDetails).every(key => !this.ExtraDetails[key]);
    if (isObjEmpty)
      return {};
    else
      return this.ExtraDetails;
  }
}
