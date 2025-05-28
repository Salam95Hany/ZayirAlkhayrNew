import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbDropdownModule, NgbModal, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { ZaBreadcrumbComponent } from "../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../Shared/za-filters/za-filters.component";
import { ZaEmptyDataComponent } from "../../../Shared/za-empty-data/za-empty-data.component";
import { FilterModel } from '../../../Models/shared/FilterModel';
import { ToastrService } from 'ngx-toastr';
import { PagedResponseModel } from '../../../Models/shared/PagedResponseModel';
import { PagingFilterModel } from '../../../Models/shared/PagingFilterModel ';
import { ZaDropDownFormControlComponent } from "../../../Shared/za-drop-down-form-control/za-drop-down-form-control.component";
import { ZaLoaderComponent } from "../../../Shared/za-loader/za-loader.component";

@Component({
  selector: 'app-slide-image',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, NgbDropdownModule, ZaBreadcrumbComponent, ZaPaginationComponent,
    ZaFiltersComponent, ZaEmptyDataComponent, ZaDropDownFormControlComponent, ZaLoaderComponent],
  templateUrl: './slide-image.component.html',
  styleUrl: './slide-image.component.css'
})
export class SlideImageComponent implements OnInit {
  @ViewChild('DetailsSidePanel', { static: true }) DetailsSidePanel: TemplateRef<any>;
  TitleList = ['مؤسسة زائر الخير', 'موقع زائر الخير', 'شريط الصور'];
  filterList: FilterModel[] = [];
  showLoader: boolean = false;
  showExportLoader: boolean = false;
  selectAll: boolean = false;
  loading = true;
  EntryId: any;
  pagingFilterModel: PagingFilterModel = {
    currentPage: 1,
    pageSize: 20,
    filterList: []
  };
  pagedResponseModel: PagedResponseModel<any[]> = {
    results: [],
  };

  constructor(private offcanvasService: NgbOffcanvas, private toaster: ToastrService, private modalService: NgbModal) { }

  ngOnInit(): void {
  }

  openSidePanel(journalEntryId: number) {
    this.EntryId = journalEntryId;
    this.offcanvasService.open(this.DetailsSidePanel, { panelClass: 'details-panel', position: 'end' });
  }

  openItemModal(content: any, item: any) {
    this.modalService.open(content, { size: 'lg', centered: true, scrollable: true });
  }

  openDeleteItemModal(content: any, id: any) {
    this.modalService.open(content, { size: 'md', centered: true, scrollable: true });
  }


  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
  }

  filterChecked(filterItems: FilterModel[]) {
    this.pagingFilterModel.filterList = filterItems;
  }
}
