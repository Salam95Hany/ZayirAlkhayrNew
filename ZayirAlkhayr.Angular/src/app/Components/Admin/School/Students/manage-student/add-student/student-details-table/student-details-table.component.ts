import { Component, EventEmitter, Input, Output } from '@angular/core';
import { StudentDetails } from '../../../../../../../Models/school/student/AddStudentModel';
import { NgFor, NgIf } from '@angular/common';
import { ArabicDateWithTimePipe } from '../../../../../../../Pipes/arabic-date-with-time.pipe';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-student-details-table',
  standalone: true,
  imports: [NgFor, NgIf, ArabicDateWithTimePipe, FormsModule],
  templateUrl: './student-details-table.component.html',
  styleUrl: './student-details-table.component.css'
})
export class StudentDetailsTableComponent {
  @Input() StudentDetails: StudentDetails[] = [];
  @Input() DetailsMode = false;
  @Output() ModalClicked = new EventEmitter<any>();

  openModal(key: string, item: any) {
    this.ModalClicked.emit({ key: key, item: item });
  }

}
