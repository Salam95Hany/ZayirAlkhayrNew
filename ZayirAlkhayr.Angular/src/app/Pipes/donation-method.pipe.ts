import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'donationMethod',
  standalone: true
})
export class DonationMethodPipe implements PipeTransform {

  transform(value: number): string {
    if (value == 1)
      return 'فودافون كاش';
    else if (value == 2)
      return 'انستا باي';
    else if (value == 3)
      return 'نقداً';

    return '';
  }

}
