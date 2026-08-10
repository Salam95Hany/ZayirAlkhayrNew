import { Component, TemplateRef, ViewChild } from '@angular/core';
import { TemplateModalComponent } from "../template-modal/template-modal.component";
import { AdminBreadcrumbComponent } from '../../../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { ZaPaginationComponent } from '../../../../../../Shared/za-pagination/za-pagination.component';
import { ZaFiltersComponent } from '../../../../../../Shared/za-filters/za-filters.component';
import { ZaEmptyDataComponent } from '../../../../../../Shared/za-empty-data/za-empty-data.component';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgFor, NgIf } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RoleCheckerDirective } from '../../../../../../Directives/role-checker.directive';
import { NgxLoadingModule } from 'ngx-loading';
import { FilterModel } from '../../../../../../Models/shared/FilterModel';
import { PagingFilterModel } from '../../../../../../Models/shared/PagingFilterModel ';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../../../../Auth/auth.service';
import { ParentService } from '../../../../../../Services/school/parent.service';
import { WhatsupSendModalComponent } from "../whatsup-send-modal/whatsup-send-modal.component";

@Component({
  selector: 'app-parent-templates',
  standalone: true,
  imports: [AdminBreadcrumbComponent, ZaPaginationComponent, ZaFiltersComponent, ZaEmptyDataComponent, NgbModule,
    NgIf, NgFor, ReactiveFormsModule, RoleCheckerDirective, NgxLoadingModule, TemplateModalComponent],
  templateUrl: './parent-templates.component.html',
  styleUrl: './parent-templates.component.css'
})
export class ParentTemplatesComponent {
  @ViewChild('OpenItemModal') OpenItemModal!: TemplateRef<any>;
  TitleList = ['مركز بشائر القرآن', 'أولياء الأمور', 'القوالب'];
  Results: any[] = [];
  FilterList: FilterModel[] = [];
  TemplateVariables: any[] = [];
  showLoader = false;
  isFilter = true;
  TotalCount = 0;
  ItemForm: FormGroup;
  UserId: any;
  TemplateId: any;
  PagingFilter: PagingFilterModel = {
    filterList: [],
    currentPage: 1,
    pageSize: 20
  };
  formErrors = {
    name: ''
  };

  constructor(private modalService: NgbModal, private parentService: ParentService, private toaster: ToastrService, private authService: AuthService) { }

  ngOnInit(): void {
    this.UserId = this.authService.userId;
    this.GetAllTemplateData();
    this.GetAllTemplateFilter();
    this.GetTemplateVariableData();
  }

  openItemModal(item: any) {
    this.TemplateId = item?.id;
    this.modalService.open(this.OpenItemModal, {
      size:'xl',
      scrollable: true,
      centered: true
    });
  }

  openDeleteItemModal(content: any, item: any) {
    this.TemplateId = item.id;
    this.modalService.open(content, {
      size: 'md',
      scrollable: true,
      centered: true
    });
  }

  GetTemplateVariableData() {
    this.parentService.GetTemplateVariableData().subscribe(data => {
      this.TemplateVariables = data.results;
    });
  }

  GetAllTemplateData() {
    this.showLoader = true;
    this.parentService.GetAllTemplateData(this.PagingFilter).subscribe(data => {
      this.showLoader = false;
      this.Results = data.results;
      this.TotalCount = data.totalCount;
    });
  }

  GetAllTemplateFilter() {
    this.parentService.GetAllTemplateFilter().subscribe(data => {
      this.FilterList = data.results;
    });
  }

  PageChange(obj: any) {
    this.PagingFilter.currentPage = obj.page;
    this.GetAllTemplateData();
  }

  FilterChecked(filterList: FilterModel[]) {
    this.PagingFilter.filterList = filterList;
    this.GetAllTemplateData();
  }

  RefreshData(item: boolean) {
    this.GetAllTemplateData();
    this.GetAllTemplateFilter();
  }

  DeleteItem() {
    this.showLoader = true;
    this.parentService.DeleteTemplate(this.TemplateId).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.GetAllTemplateData();
        this.GetAllTemplateFilter();
        this.modalService.dismissAll();
      }
      else
        this.toaster.error(data.message);
      this.showLoader = false;
    });
  }

  PreviewHtml(template: string): string {
    let bodyStr = template;

    this.TemplateVariables.forEach(variable => {
      const regex = new RegExp(`{{${variable.displayKey}}}`, 'g');

      bodyStr = bodyStr?.replace(
        regex,
        `<span class="template-variable">${variable.displayName}</span>`
      );
    });

    return bodyStr;
  }
}
