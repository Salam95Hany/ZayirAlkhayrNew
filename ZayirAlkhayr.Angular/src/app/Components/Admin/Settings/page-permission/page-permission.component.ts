import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { SettingService } from '../../../../Services/settings/setting.service';

@Component({
  selector: 'app-page-permission',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './page-permission.component.html',
  styleUrl: './page-permission.component.css'
})
export class PagePermissionComponent implements OnInit, OnChanges {
  @Input() UserId: string;
  @Output() ApplicationChange = new EventEmitter<any>();

  Applications = [];
  SelectedApplications = [];
  flatApplications: any[] = [];

  constructor(private settingService: SettingService, private toaster: ToastrService) { }

  ngOnInit() {
    if (this.UserId)
      this.GetAllApplicationsWithParents();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.UserId)
      this.GetAllApplicationsWithParents();
  }

  GetAllApplicationsWithParents() {
    this.settingService.GetAllApplicationsWithParents().subscribe(data => {
      this.Applications = data.results;
      this.flatApplications = this.flattenApplications(this.Applications);
      this.settingService.GetApplicationsByUserId(this.UserId).subscribe(res => {
        this.SelectedApplications = res.results;
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

    if (item[prop]) {
      item.active = true;
      this.setParentActive(item, applications);
    } else {
      const anyActive =
        item.canAdd || item.canEdit || item.canDelete || item.canExport;
      item.active = anyActive;

      if (!anyActive) {
        this.checkAndDeactivateParents(item, applications);
      }
    }
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
        userId: this.UserId,
        applicationName: i.applicationName,
        canAdd: i.canAdd,
        canEdit: i.canEdit,
        canDelete: i.canDelete,
        canExport: i.canExport
      }
    });

    if (selected.length == 0) {
      this.toaster.warning('برجاء اختيار صفحة واحدة على الاقل');
      return;
    }

    this.settingService.AssignApplicationToUser(selected).subscribe(data => {
      if (data.isSuccess) {
        this.toaster.success(data.message);
        this.ApplicationChange.emit(true);
      } else
        this.toaster.error(data.message);
    });
  }
}
