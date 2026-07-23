import { Component } from '@angular/core';
import { ComingSoonComponent } from "../../../../../../Shared/coming-soon/coming-soon.component";

@Component({
  selector: 'app-promotion',
  standalone: true,
  imports: [ComingSoonComponent],
  templateUrl: './promotion.component.html',
  styleUrl: './promotion.component.css'
})
export class PromotionComponent {

}
