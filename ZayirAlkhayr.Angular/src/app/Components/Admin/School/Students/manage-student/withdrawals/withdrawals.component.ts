import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../../Shared/za-pagination/za-pagination.component';
import { SchoolStudentService } from '../../../../../../Services/school/school-student.service';
import { WithdrawnStudent, settlementStatuses } from '../../../../../../Models/school/student/StudentWithdrawal';
import { StudentWithdrawalModalComponent } from '../student-withdrawal-modal/student-withdrawal-modal.component';

@Component({
  selector: 'app-withdrawals',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminBreadcrumbComponent, ZaPaginationComponent],
  templateUrl: './withdrawals.component.html',
  styleUrl: './withdrawals.component.css'
})
export class WithdrawalsComponent implements OnInit, OnDestroy {
  readonly titles = ['مركز بشائر القرآن', 'إدارة الطلاب', 'الطلاب المنسحبون'];
  readonly statuses = settlementStatuses;
  readonly pageSize = 25;
  rows: WithdrawnStudent[] = [];
  total = 0;
  page = 1;
  search = '';
  private appliedSearch = '';
  loading = false;
  error = '';
  private request?: Subscription;

  constructor(private service: SchoolStudentService, private modal: NgbModal) {}

  ngOnInit() { this.load(1); }
  ngOnDestroy() { this.request?.unsubscribe(); }

  searchStudents() {
    this.appliedSearch = this.search.trim();
    this.load(1);
  }

  load(page: number) {
    this.request?.unsubscribe();
    this.page = page;
    this.loading = true;
    this.error = '';
    this.request = this.service.GetWithdrawnStudents(page, this.pageSize, this.appliedSearch).subscribe({
      next: result => {
        this.loading = false;
        if (result.isSuccess) {
          this.rows = result.results;
          this.total = result.totalCount;
        } else this.error = result.message;
      },
      error: () => {
        this.loading = false;
        this.error = 'تعذر تحميل قائمة الطلاب المنسحبين.';
      }
    });
  }

  openDetails(student: WithdrawnStudent) {
    const ref = this.modal.open(StudentWithdrawalModalComponent, {
      size: 'xl', centered: true, scrollable: true, backdrop: 'static', keyboard: false, ariaLabelledBy: 'student-withdrawal-title'
    });
    ref.componentInstance.enrollmentId = student.enrollmentId;
    ref.closed.subscribe(changed => { if (changed) this.load(this.page); });
  }
}
