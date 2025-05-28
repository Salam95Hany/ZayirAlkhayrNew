import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-za-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './za-loader.component.html',
  styleUrl: './za-loader.component.css'
})
export class ZaLoaderComponent {
  @Input() show: boolean = false;
  @Input() inline: boolean = false;
}
