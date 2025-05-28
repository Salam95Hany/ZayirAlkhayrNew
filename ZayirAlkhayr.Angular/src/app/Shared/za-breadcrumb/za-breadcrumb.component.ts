import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-za-breadcrumb',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './za-breadcrumb.component.html',
  styleUrl: './za-breadcrumb.component.css'
})
export class ZaBreadcrumbComponent {
  @Input() TitleList: any[]
}
