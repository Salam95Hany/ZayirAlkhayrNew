import { Component } from '@angular/core';
import { TemplateModalComponent } from "../template-modal/template-modal.component";

@Component({
  selector: 'app-parent-templates',
  standalone: true,
  imports: [TemplateModalComponent],
  templateUrl: './parent-templates.component.html',
  styleUrl: './parent-templates.component.css'
})
export class ParentTemplatesComponent {

}
