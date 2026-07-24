import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'arabicDateWithTime',
  standalone: true
})
export class ArabicDateWithTimePipe implements PipeTransform {

  transform(value: Date | string | null | undefined): string {
    if (!value)
      return '-';

    const date = new Date(value);

    if (isNaN(date.getTime()))
      return '-';

    return new Intl.DateTimeFormat('ar-AE', {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric'
    }).format(date);
  }

}
