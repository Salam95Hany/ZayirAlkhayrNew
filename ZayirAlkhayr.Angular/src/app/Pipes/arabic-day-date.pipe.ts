import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'arabicDayDate',
  standalone: true
})
export class ArabicDayDatePipe implements PipeTransform {
 private arabicDays = ['الأحد', 'الاثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'];

  transform(value: Date | string): string {
    const date = new Date(value);
    const day = date.getDate();
    const month = date.getMonth() + 1;
    const year = date.getFullYear();
    const dayName = this.arabicDays[date.getDay()];
    
    return `${day}/${month}/${year} . ${dayName}`;
  }

}
