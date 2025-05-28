import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-za-pagination',
  standalone: true,
  imports: [CommonModule, PaginationModule],
  templateUrl: './za-pagination.component.html',
  styleUrl: './za-pagination.component.css'
})
export class ZaPaginationComponent implements OnInit, OnChanges {
  @Input() currentPage: number;
  @Input() pageSize: number;
  @Input() totalCount: number;
  @Input() totalPages: number;
  @Input() newPagination: boolean = true;
  @Output() pageChanged = new EventEmitter<any>();
  pages: number[] = [];
  maxSize = 3;
  showingStr = '';

  constructor() { }

  ngOnInit(): void {
    this.resetShowingStr();
  }

  ngOnChanges(changes: any) {
    this.resetShowingStr();
    if (changes.totalCount?.firstChange) {
      this.totalPages = Math.ceil(this.totalCount / this.pageSize);
      this.generatePages();
    }
  }

  resetShowingStr() {
    let showingStr = '';
    const lPage = this.currentPage * this.pageSize;
    if (lPage >= this.totalCount) {
      const fNum = (this.pageSize * (this.currentPage - 1)) + 1;
      const lNum = (this.totalCount - fNum);
      showingStr = (fNum) + '-' + (lNum + fNum);

    } else {
      if (this.totalPages === this.currentPage) {
        if (this.currentPage === 1 || this.currentPage === 0) {
          showingStr = this.currentPage + '-' + this.totalCount;
        }
        const fNum = (this.pageSize * (this.currentPage - 1));
        const lNum = (this.totalCount - fNum);
        showingStr = (fNum + 1) + '-' + (lNum + fNum);
      } else {
        if (this.currentPage === 1 || this.currentPage === 0) {
          if (this.totalCount !== 0 && (this.pageSize > this.totalCount)) {
            showingStr = this.currentPage + '-' + this.totalCount;
          } else {
            showingStr = '1-' + this.pageSize;
          }
        } else {
          showingStr = (this.pageSize * (this.currentPage - 1)) + 1 + '-' + (this.currentPage * this.pageSize);
        }
      }
    }
    this.showingStr = showingStr;
  }

  pageChangeEvent(event: any): void {
    this.currentPage = event.page;
    this.pageChanged.emit(event);
    this.resetShowingStr();
  }
  generatePages() {
    const totalPages = this.totalPages;
    const current = this.currentPage;
    const maxVisible = 3;
    let startPage = Math.max(current - Math.floor(maxVisible / 2), 1);
    let endPage = startPage + maxVisible - 1;
    if (endPage > totalPages) {
      endPage = totalPages;
      startPage = Math.max(endPage - maxVisible + 1, 1);
    }

    this.pages = Array(endPage - startPage + 1)
      .fill(0)
      .map((_, i) => startPage + i);
  }

  setPage(page: number) {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.pageChanged.emit({ page: this.currentPage });
      this.generatePages();
    }
  }
}
