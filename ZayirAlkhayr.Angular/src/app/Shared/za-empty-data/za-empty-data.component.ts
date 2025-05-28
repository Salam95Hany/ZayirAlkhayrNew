import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ZaSpinnerComponent } from "../za-spinner/za-spinner.component";

@Component({
  selector: 'app-za-empty-data',
  standalone: true,
  imports: [CommonModule, ZaSpinnerComponent],
  templateUrl: './za-empty-data.component.html',
  styleUrl: './za-empty-data.component.css'
})
export class ZaEmptyDataComponent {
  @Input() showEmptyData: boolean = false;
  @Input() showLoader: boolean = false;
}
