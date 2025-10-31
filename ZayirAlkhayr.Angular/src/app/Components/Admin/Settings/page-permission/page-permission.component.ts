import { CommonModule } from '@angular/common';
import { Component, ElementRef, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { SettingService } from '../../../../Services/settings/setting.service';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import { NgxLoadingModule } from "ngx-loading";

@Component({
  selector: 'app-page-permission',
  standalone: true,
  imports: [CommonModule, FormsModule, NgxLoadingModule],
  templateUrl: './page-permission.component.html',
  styleUrl: './page-permission.component.css'
})
export class PagePermissionComponent implements OnInit {
  @ViewChild('sidepanelEditStatus') sidepanelEditStatus: ElementRef;
  @Input() UserId: string;
  @Input() UserName: string;
  @Input() RoleName: string;

  Applications = [];
  SelectedApplications = [];
  flatApplications: any[] = [];
  showLoader = false;
  IsSuperAdmin = false;

  constructor(private settingService: SettingService, private toaster: ToastrService, private offcanvasService: NgbOffcanvas, private injector: Injector) { }

  ngOnInit() {
    this.UserId = this.injector.get('UserId');
    this.UserName = this.injector.get('UserName');
    this.RoleName = this.injector.get('RoleName');
    this.GetAllApplicationsWithParents();
  }

  dismissSidePanel() {
    this.offcanvasService.dismiss();
  }

  GetAllApplicationsWithParents() {
    this.showLoader = true;
    this.settingService.GetAllApplicationsWithParents().subscribe(data => {
      this.Applications = data.results;
      this.flatApplications = this.flattenApplications(this.Applications);
      this.settingService.GetApplicationsByUserId(this.UserId).subscribe(res => {
        this.showLoader = false;
        this.SelectedApplications = res.results;
        this.IsSuperAdmin = this.SelectedApplications.some(i => i.applicationId == 'c4d473a1-0820-4701-a386-bebf90f05df7');
        this.SelectedApplications.forEach(item => {
          let obj = this.flatApplications.find(i => i.applicationId == item.applicationId);
          if (obj) {
            obj.active = true;
            obj.canAdd = item.canAdd;
            obj.canEdit = item.canEdit;
            obj.canDelete = item.canDelete;
            obj.canExport = item.canExport;
          }
        })
      });
    });
  }

  flattenApplications(apps: any[]): any[] {
    let result: any[] = [];

    for (let app of apps) {
      result.push(app);

      if (app.children && app.children.length > 0) {
        result = result.concat(this.flattenApplications(app.children));
      }
    }

    return result;
  }

  toggleExpand(item: any, event: Event) {
    event.stopPropagation();
    item.expanded = !item.expanded;
  }

  toggleSwitch(item: any, event: Event) {
    event.stopPropagation();
    item.active = !item.active;

    if (item.children.length === 0) {
      item.canAdd = item.active;
      item.canEdit = item.active;
      item.canDelete = item.active;
      item.canExport = item.active;
    } else {
      this.setActiveRecursive(item.children, item.active);
    }

    if (item.active && item.parentId) {
      this.setParentActive(item, this.Applications);
    } else if (!item.active) {
      this.checkAndDeactivateParents(item, this.Applications);
    }

    this.deactivateTopLevelIfEmpty();
  }

  deactivateTopLevelIfEmpty() {
    const hasAnyActive = this.flatApplications.some(
      i => i.active || i.canAdd || i.canEdit || i.canDelete || i.canExport
    );

    if (!hasAnyActive) {
      this.Applications.forEach(topApp => {
        topApp.active = false;
        topApp.canAdd = false;
        topApp.canEdit = false;
        topApp.canDelete = false;
        topApp.canExport = false;
      });
    }
  }

  setActiveRecursive(children: any[], state: boolean) {
    if (!children) return;

    for (let child of children) {
      child.active = state;
      child.canAdd = state;
      child.canEdit = state;
      child.canDelete = state;
      child.canExport = state;
      if (child.children && child.children.length > 0) {
        this.setActiveRecursive(child.children, state);
      }
    }
  }

  setParentActive(item: any, applications: any[]) {
    const parent = this.findParent(item.parentId, applications);
    if (parent) {
      parent.active = true;
      parent.canAdd = true;
      parent.canEdit = true;
      parent.canDelete = true;
      parent.canExport = true;
      this.setParentActive(parent, applications);
    }
  }

  findParent(parentId: string, applications: any[]): any | null {
    for (let app of applications) {
      if (app.applicationId === parentId) {
        return app;
      }
      if (app.children && app.children.length > 0) {
        const found = this.findParent(parentId, app.children);
        if (found) return found;
      }
    }
    return null;
  }

  togglePermission(item: any, prop: string, applications: any[]) {
    item[prop] = !item[prop];
    item.active = true;
    this.setParentActive(item, applications);

  }

  checkAndDeactivateParents(item: any, applications: any[]) {
    const parent = this.findParent(item.parentId, applications);
    if (parent) {
      const hasActiveChild = parent.children.some(
        (child: any) =>
          child.active ||
          child.canAdd ||
          child.canEdit ||
          child.canDelete ||
          child.canExport
      );

      if (!hasActiveChild) {
        parent.active = false;
        parent.canAdd = false;
        parent.canEdit = false;
        parent.canDelete = false;
        parent.canExport = false;
        this.checkAndDeactivateParents(parent, applications);
      }
    }
  }

  AssignApplicationToUser() {
    let selected = this.flatApplications.filter(i => i.active).map(i => {
      return {
        applicationId: i.applicationId,
        applicationName: i.applicationName,
        canAdd: i.canAdd,
        canEdit: i.canEdit,
        canDelete: i.canDelete,
        canExport: i.canExport
      }
    });

    if (selected.length == 0 && !this.IsSuperAdmin) {
      this.toaster.warning('برجاء اختيار صفحة واحدة على الاقل');
      return;
    }
    
    this.showLoader = true;
    this.settingService.AssignApplicationToUser(selected, this.UserId, this.IsSuperAdmin).subscribe(data => {
      this.showLoader = false;
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.offcanvasService.dismiss();
      } else
        this.toaster.error(data.message);
    });
  }
}
