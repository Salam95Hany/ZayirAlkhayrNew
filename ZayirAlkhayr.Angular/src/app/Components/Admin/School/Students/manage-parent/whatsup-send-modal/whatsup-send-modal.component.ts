import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ParentService } from '../../../../../../Services/school/parent.service';
import { ZaDropDownFormControlComponent } from "../../../../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component";
import { FormsModule } from '@angular/forms';
import { FormDropdownModel } from '../../../../../../Models/shared/FormDropdownModel';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-whatsup-send-modal',
  standalone: true,
  imports: [CommonModule, ZaDropDownFormControlComponent, FormsModule],
  templateUrl: './whatsup-send-modal.component.html',
  styleUrl: './whatsup-send-modal.component.css'
})
export class WhatsupSendModalComponent implements OnInit {
  @Input() Templates: any[] = [];
  @Input() ParentId: number;
  @Input() WhatsupPhone: string;
  StudentId: number;
  TemplateId: number;
  ParentStudents: FormDropdownModel[] = [];
  ParentName = '';
  MessageHtml = '';
  MessageTxt = '';
  constructor(private parentService: ParentService, private toaster: ToastrService) { }

  ngOnInit(): void {
    this.GetParentStudents();
  }

  GetParentStudents() {
    this.parentService.GetParentStudents(this.ParentId).subscribe(data => {
      this.ParentStudents = data.map(i => { return { value: i.studentId, name: i.studentName } });
      this.ParentName = data[0]?.parentName;
    });
  }

  GetStudentTempMessage() {
    this.parentService.GetStudentTempMessage(this.TemplateId, this.ParentId, this.StudentId).subscribe(data => {
      this.MessageTxt = data.results;
      this.MessageHtml = data.results?.replace(/\n/g, '<br>');
      console.log(this.MessageHtml);
      
    });
  }

  TempChanged(item: any) {
    if (this.TemplateId && this.StudentId)
      this.GetStudentTempMessage();
  }

  StudentChanged(item: any) {
    if (this.TemplateId && this.StudentId)
      this.GetStudentTempMessage();
  }

  SendWhatsup() {
    if (!this.MessageTxt) {
      this.toaster.warning('لا يوجد قالب للارسال');
      return;
    }

    if (!this.TemplateId) {
      this.toaster.warning('برجاء اختيار قالب');
      return;
    }

    if (!this.StudentId) {
      this.toaster.warning('برجاء اختيار طالب');
      return;
    }

    const message = encodeURIComponent(this.MessageTxt);

    window.open(`https://wa.me/${this.WhatsupPhone}?text=${message}`, '_blank');
  }


}
