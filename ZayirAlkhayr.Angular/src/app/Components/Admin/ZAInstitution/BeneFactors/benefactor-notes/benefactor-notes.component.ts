import { Component, OnInit } from '@angular/core';
import { ZaBreadcrumbComponent } from "../../../../../Shared/za-breadcrumb/za-breadcrumb.component";
import { ZaPaginationComponent } from "../../../../../Shared/za-pagination/za-pagination.component";
import { ZaFiltersComponent } from "../../../../../Shared/za-filters/za-filters.component";
import { CommonModule } from '@angular/common';
import { ZaEmptyDataComponent } from '../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PagingFilterModel } from '../../../../../Models/shared/PagingFilterModel ';
import { PagedResponseModel } from '../../../../../Models/shared/PagedResponseModel';
import { FilterModel } from '../../../../../Models/shared/FilterModel';
import { BenefactorService } from '../../../../../Services/zainstitution/benefactor.service';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-benefactor-notes',
  standalone: true,
  imports: [CommonModule, ZaBreadcrumbComponent, ZaPaginationComponent,
    ZaFiltersComponent, ZaEmptyDataComponent, NgbModule, NgxLoadingModule],
  templateUrl: './benefactor-notes.component.html',
  styleUrl: './benefactor-notes.component.css'
})
export class BenefactorNotesComponent implements OnInit {
  TitleList = ['مؤسسة زائر الخير', 'إدارة المتبرعين', 'ملاحظات المتبرعين'];
  filterList: FilterModel[] = [
    {
      categoryDisplayName:'بالكود, الاسم',
      categoryName:'SearchText',
      filterType:'SearchText'
    }
  ];
  isFilter = true;
  showLoader = false;
  pagingFilterModel: PagingFilterModel = {
    currentPage: 1,
    pageSize: 20,
    filterList: []
  };
  pagedResponseModel: PagedResponseModel<any[]> = {
    totalCount: 0,
    results: [],
  };

  constructor(private benefactorService: BenefactorService) { }

  ngOnInit(): void {
    this.GetBeneFactorNotes();
  }

  GetBeneFactorNotes() {
    this.showLoader = true;
    this.benefactorService.GetBeneFactorNotes(this.pagingFilterModel).subscribe(data => {
      this.showLoader = false;
      this.pagedResponseModel.results = data.results;
      this.pagedResponseModel.totalCount = data.totalCount;
    });
  }

  pageChanged(obj: any) {
    this.pagingFilterModel.currentPage = obj.page;
    this.GetBeneFactorNotes();
  }

  filterChecked(filterItems: FilterModel[]) {
    this.pagingFilterModel.filterList = filterItems;
    this.pagingFilterModel.currentPage = 1;
    this.GetBeneFactorNotes();
  }
}
