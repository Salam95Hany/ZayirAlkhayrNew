import { CommonModule, NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { StudentPrintSlot, TicketStudent } from '../../../../../../Models/school/student/StudentPrintSlot';
import { StudentTicketPrintService } from '../../../../../../Services/school/student-ticket-print.service';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { ZaPaginationComponent } from "../../../../../../Shared/za-pagination/za-pagination.component";

type PrintStatus = 'IDLE' | 'PREVIEWING' | 'GENERATING';

interface DragPayload {
  studentId: string;
  sourcePage?: number;
  sourceSlot?: number;
}

@Component({
  selector: 'app-student-ticket',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIf, ZaPaginationComponent],
  templateUrl: './student-ticket.component.html',
  styleUrl: './student-ticket.component.css'
})
export class StudentTicketComponent implements OnInit {
  readonly slotsPerPage = 8;
  readonly slotNumbers = Array.from({ length: this.slotsPerPage }, (_, index) => index + 1);
  readonly todayIsoDate = this.getLocalIsoDate();
  readonly minimumRenewalDate = this.getLocalIsoDate(1);
  private pageSlots = new Map<number, Array<TicketStudent | null>>();
  academicYears = [];
  grades = [];
  selectedAcademicYear = 0;
  selectedGrade = 0;
  selectedPayment = 0;
  selectedPrinted = 0;
  selectedStudentId: string | null = null;
  searchTerm = '';
  currentPage = 1;
  printStatus: PrintStatus = 'IDLE';
  showFilters = false;
  showClearConfirmation = false;
  showPreview = false;
  showRenewalDateModal = false;
  students: TicketStudent[] = [];
  pendingRenewalStudents: TicketStudent[] = [];
  renewalDateValues: Record<string, string> = {};
  draggingStudentId: string | null = null;
  dragOverSlot: number | null = null;
  TotalCount = 0;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 8
  }

  constructor(private printService: StudentTicketPrintService, private toaster: ToastrService) {
    this.pageSlots.set(1, this.createEmptyPage());
  }

  ngOnInit(): void {
    this.GetAllStudentTicketData();
    this.GetAcademicYear();
    this.GetAcademicStages();
  }

  get currentSlots(): Array<TicketStudent | null> {
    return this.ensurePage(this.currentPage);
  }

  get currentPageCount(): number {
    return this.currentSlots.filter(Boolean).length;
  }

  get currentPrintModel(): StudentPrintSlot[] {
    return this.currentSlots.reduce<StudentPrintSlot[]>((model, student, index) => {
      if (student) {
        const printSlot: StudentPrintSlot = {
          studentId: student.id,
          slotNumber: index + 1
        };

        if (student.paymentStatus === 'PENDING' && student.installmentRenewalDate) {
          printSlot.InstallmentRenewalDate = student.installmentRenewalDate;
        }

        model.push(printSlot);
      }
      return model;
    }, []);
  }

  get allRenewalDatesValid(): boolean {
    return this.pendingRenewalStudents.length > 0 && this.pendingRenewalStudents.every(student => {
      const selectedDate = this.renewalDateValues[student.id];
      return !!selectedDate && selectedDate > this.todayIsoDate;
    });
  }

  GetAcademicYear() {
    this.printService.GetAcademicYear().subscribe(data => {
      this.academicYears = data;
    })
  }

  GetAcademicStages() {
    this.printService.GetAcademicStages().subscribe(data => {
      this.grades = data;
    })
  }

  GetAllStudentTicketData() {
    this.printService.GetAllStudentTicketData(this.PagingFilter).subscribe(data => {
      this.students = data.results;
      this.TotalCount = data.totalCount;
    })
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllStudentTicketData();
  }

  onContextChange(): void {
    this.currentPage = 1;
    this.PagingFilter.currentPage = 1;
    if (this.selectedAcademicYear && this.selectedAcademicYear != 0) {
      this.PagingFilter.filterList = this.PagingFilter.filterList.filter(i => i.categoryName != 'AcademicYear');
      let checked = this.PagingFilter.filterList.find(i => i.categoryName == 'AcademicYear' && i.itemId == this.selectedAcademicYear.toString());
      if (!checked) {
        this.PagingFilter.filterList.push({
          categoryName: 'AcademicYear',
          itemId: this.selectedAcademicYear.toString()
        })
      }
    } else {
      this.PagingFilter.filterList = this.PagingFilter.filterList.filter(i => i.categoryName != 'AcademicYear');
    }

    if (this.selectedGrade && this.selectedGrade != 0) {
      this.PagingFilter.filterList = this.PagingFilter.filterList.filter(i => i.categoryName != 'AcademicStage');
      let checked = this.PagingFilter.filterList.find(i => i.categoryName == 'AcademicStage' && i.itemId == this.selectedGrade.toString());
      if (!checked) {
        this.PagingFilter.filterList.push({
          categoryName: 'AcademicStage',
          itemId: this.selectedGrade.toString()
        })
      }
    } else {
      this.PagingFilter.filterList = this.PagingFilter.filterList.filter(i => i.categoryName != 'AcademicStage');
    }

    if (this.selectedPayment && this.selectedPayment != 0) {
      this.PagingFilter.filterList = this.PagingFilter.filterList.filter(i => i.categoryName != 'PaymentStatus');
      let checked = this.PagingFilter.filterList.find(i => i.categoryName == 'PaymentStatus' && i.itemId == this.selectedPayment.toString());
      if (!checked) {
        this.PagingFilter.filterList.push({
          categoryName: 'PaymentStatus',
          itemId: this.selectedPayment.toString()
        })
      }
    } else {
      this.PagingFilter.filterList = this.PagingFilter.filterList.filter(i => i.categoryName != 'PaymentStatus');
    }

    if (this.selectedPrinted && this.selectedPrinted != 0) {
      this.PagingFilter.filterList = this.PagingFilter.filterList.filter(i => i.categoryName != 'PrintStatus');
      let checked = this.PagingFilter.filterList.find(i => i.categoryName == 'PrintStatus' && i.itemId == this.selectedPrinted.toString());
      if (!checked) {
        this.PagingFilter.filterList.push({
          categoryName: 'PrintStatus',
          itemId: this.selectedPrinted.toString()
        })
      }
    } else {
      this.PagingFilter.filterList = this.PagingFilter.filterList.filter(i => i.categoryName != 'PrintStatus');
    }

    this.GetAllStudentTicketData();

  }

  onSearchChange(): void {
    this.currentPage = 1;
  }

  selectStudent(student: TicketStudent): void {
    this.selectedStudentId = student.id;
  }

  startDrag(event: DragEvent, student: TicketStudent, sourceSlot?: number): void {
    const payload: DragPayload = { studentId: student.id };
    if (sourceSlot) {
      payload.sourcePage = this.currentPage;
      payload.sourceSlot = sourceSlot;
    }
    this.draggingStudentId = student.id;
    this.selectedStudentId = student.id;
    event.dataTransfer?.setData('application/json', JSON.stringify(payload));
    if (event.dataTransfer) event.dataTransfer.effectAllowed = 'move';
  }

  allowDrop(event: DragEvent, slotNumber: number): void {
    event.preventDefault();
    if (event.dataTransfer) event.dataTransfer.dropEffect = 'move';
    this.dragOverSlot = slotNumber;
  }

  leaveSlot(event: DragEvent): void {
    const target = event.currentTarget as HTMLElement;
    if (!target.contains(event.relatedTarget as Node)) this.dragOverSlot = null;
  }

  dropOnSlot(event: DragEvent, targetSlot: number): void {
    event.preventDefault();
    const payload = this.readDragPayload(event);
    this.dragOverSlot = null;
    this.draggingStudentId = null;
    if (!payload) return;

    const student = this.students.find(item => item.id === payload.studentId);
    if (!student) return;

    const targetIndex = targetSlot - 1;
    const targetStudent = this.currentSlots[targetIndex];
    const previousLocation = this.findStudentLocation(student.id);
    if (previousLocation?.page === this.currentPage && previousLocation.slot === targetSlot) {
      this.requestRenewalDateIfRequired(student);
      return;
    }

    if (previousLocation) this.ensurePage(previousLocation.page)[previousLocation.slot - 1] = null;
    if (targetStudent && payload.sourcePage && payload.sourceSlot) {
      this.ensurePage(payload.sourcePage)[payload.sourceSlot - 1] = targetStudent;
    }
    this.currentSlots[targetIndex] = student;
    this.requestRenewalDateIfRequired(student);
  }

  confirmRenewalDate(): void {
    if (!this.allRenewalDatesValid) {
      this.toaster.warning('يجب اختيار تاريخ لاحق لتاريخ اليوم لكل الطلاب');
      return;
    }

    this.pendingRenewalStudents.forEach(student => {
      student.installmentRenewalDate = this.renewalDateValues[student.id];
    });

    this.pendingRenewalStudents = [];
    this.renewalDateValues = {};
    this.showRenewalDateModal = false;
  }

  isRenewalDateInvalid(studentId: string): boolean {
    const selectedDate = this.renewalDateValues[studentId];
    return !!selectedDate && selectedDate <= this.todayIsoDate;
  }

  endDrag(): void {
    this.draggingStudentId = null;
    this.dragOverSlot = null;
  }

  removeFromSlot(slotNumber: number): void {
    this.currentSlots[slotNumber - 1] = null;
  }

  autoDistribute(): void {
    const assignedIds = this.getAssignedStudentIds();
    const queue = this.students.filter(student => !assignedIds.has(student.id));
    if (!queue.length) {
      this.requestMissingRenewalDatesForAssignedStudents();
      this.toaster.info('لا يوجد طلاب جدد لتوزيعهم');
      return;
    }

    let queueIndex = 0;
    let page = 1;
    while (queueIndex < queue.length) {
      const slots = this.ensurePage(page);
      for (let index = 0; index < slots.length && queueIndex < queue.length; index++) {
        if (!slots[index]) slots[index] = queue[queueIndex++];
      }
      page++;
    }
    this.currentPage = 1;
    this.requestMissingRenewalDatesForAssignedStudents();
  }

  confirmClearAll(): void {
    this.showClearConfirmation = true;
  }

  clearAll(): void {
    this.pageSlots.clear();
    this.pageSlots.set(1, this.createEmptyPage());
    this.currentPage = 1;
    this.showClearConfirmation = false;
    this.toaster.success('تم مسح جميع التوزيعات');
  }

  closePreview(): void {
    this.showPreview = false;
    this.printStatus = 'IDLE';
  }

  generateAndPrintPdf(slots: StudentPrintSlot[] = this.currentPrintModel): void {
    if (!slots.length) {
      this.toaster.warning('أضف طالبًا واحدًا على الأقل قبل تجهيز الطباعة');
      return;
    }

    this.printStatus = 'GENERATING';
    this.printService.generateAndPrintPdf(slots);
    this.toaster.success('تم تجهيز بيانات الطباعة للربط مع خدمة PDF');
    this.printStatus = 'IDLE';
    this.clearAll();
  }

  isStudentAssigned(studentId: string): boolean {
    return !!this.findStudentLocation(studentId);
  }

  studentPosition(studentId: string): string {
    const location = this.findStudentLocation(studentId);
    return location ? `ص ${location.page} · خانة ${location.slot}` : '';
  }

  formatDate(value: string | null): string {
    if (!value) return '';
    const date = new Date(value);

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();

    return `${year}/${month}/${day}`;
  }

  trackByStudentId(_index: number, student: TicketStudent): string {
    return student.id;
  }

  private createEmptyPage(): Array<TicketStudent | null> {
    return Array.from({ length: this.slotsPerPage }, () => null);
  }

  private ensurePage(page: number): Array<TicketStudent | null> {
    if (!this.pageSlots.has(page)) this.pageSlots.set(page, this.createEmptyPage());
    return this.pageSlots.get(page)!;
  }

  private findStudentLocation(studentId: string): { page: number; slot: number } | null {
    for (const [page, slots] of this.pageSlots.entries()) {
      const slotIndex = slots.findIndex(student => student?.id === studentId);
      if (slotIndex >= 0) return { page, slot: slotIndex + 1 };
    }
    return null;
  }

  private getAssignedStudentIds(): Set<string> {
    const ids = new Set<string>();
    this.pageSlots.forEach(slots => slots.forEach(student => {
      if (student) ids.add(student.id);
    }));
    return ids;
  }

  private requestRenewalDateIfRequired(student: TicketStudent): void {
    if (student.paymentStatus !== 'PENDING' || student.installmentRenewalDate) return;

    this.openRenewalDateModal([student]);
  }

  private requestMissingRenewalDatesForAssignedStudents(): void {
    const pendingStudents = Array.from(this.pageSlots.values())
      .flat()
      .filter((student): student is TicketStudent =>
        !!student && student.paymentStatus === 'PENDING' && !student.installmentRenewalDate
      );

    this.openRenewalDateModal(pendingStudents);
  }

  private openRenewalDateModal(students: TicketStudent[]): void {
    const uniqueStudents = Array.from(new Map(students.map(student => [student.id, student])).values());
    if (!uniqueStudents.length) return;

    this.pendingRenewalStudents = uniqueStudents;
    this.renewalDateValues = Object.fromEntries(uniqueStudents.map(student => [student.id, '']));
    this.showRenewalDateModal = true;
  }

  private getLocalIsoDate(daysToAdd = 0): string {
    const date = new Date();
    date.setHours(0, 0, 0, 0);
    date.setDate(date.getDate() + daysToAdd);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private readDragPayload(event: DragEvent): DragPayload | null {
    try {
      const value = event.dataTransfer?.getData('application/json');
      return value ? JSON.parse(value) as DragPayload : null;
    } catch {
      return null;
    }
  }
}
