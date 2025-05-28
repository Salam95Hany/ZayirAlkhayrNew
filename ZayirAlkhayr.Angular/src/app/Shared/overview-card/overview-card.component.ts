import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-overview-card',
  standalone: true,
  imports: [],
  templateUrl: './overview-card.component.html',
  styleUrl: './overview-card.component.css'
})
export class OverviewCardComponent {
  @Input() data!:
    {
      title: string,
      number: number,
      statusIcon: string,
      statusBgClass: string
    };
}
