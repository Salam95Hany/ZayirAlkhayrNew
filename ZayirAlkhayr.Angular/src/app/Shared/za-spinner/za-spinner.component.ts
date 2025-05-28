import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-za-spinner',
  standalone: true,
  imports: [],
  templateUrl: './za-spinner.component.html',
  styleUrl: './za-spinner.component.css'
})
export class ZaSpinnerComponent {
  @Input() size: string = '72px';
}
