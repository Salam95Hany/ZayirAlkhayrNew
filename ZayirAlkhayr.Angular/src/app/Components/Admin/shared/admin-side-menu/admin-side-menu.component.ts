import { CommonModule } from '@angular/common';
import { Component, DestroyRef, Input, OnDestroy, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs';
import { MenuSidebarItem } from '../../../../Models/shared/MenueSidebarItem';
import { MenuType } from '../../../../Services/shared/menue.service';

@Component({
  selector: 'app-admin-side-menu',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './admin-side-menu.component.html',
  styleUrl: './admin-side-menu.component.css'
})
export class AdminSideMenuComponent implements OnDestroy {
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  @Input({ required: true }) menuItem?: MenuSidebarItem;

  isOpen = false;
  private previousDocumentOverflow = '';

  private readonly menuLabels: Record<string, string> = {
    [`${MenuType.ZAInstitution}:1`]: 'إدارة الموقع',
    [`${MenuType.ZAInstitution}:2`]: 'إدارة المتبرعين',
    [`${MenuType.ZAInstitution}:3`]: 'إدارة المهام',
    [`${MenuType.ZAInstitution}:4`]: 'إدارة الحسابات',
    [`${MenuType.ZAInstitution}:5`]: 'الخدمات الاجتماعية',
    [`${MenuType.School}:1`]: 'إدارة الطلاب',
    [`${MenuType.School}:2`]: 'أولياء الأمور',
    [`${MenuType.School}:3`]: 'إدارة الرسوم',
    [`${MenuType.School}:5`]: 'إعدادات المدرسة',
    [`${MenuType.Settings}:user`]: 'المستخدمون',
    [`${MenuType.Settings}:backup`]: 'النسخ الاحتياطية'
  };

  constructor() {
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.close());
  }

  get items(): MenuSidebarItem[] {
    return this.menuItem?.subMenus || [];
  }

  get moduleTitle(): string {
    switch (this.menuItem?.menuItemId) {
      case MenuType.School: return 'إدارة المدرسة';
      case MenuType.Settings: return 'إعدادات النظام';
      case MenuType.ZAInstitution: return 'إدارة المؤسسة';
      default: return 'قائمة الإدارة';
    }
  }

  get moduleSubtitle(): string {
    switch (this.menuItem?.menuItemId) {
      case MenuType.School: return 'النظام الأكاديمي والمالي';
      case MenuType.Settings: return 'المستخدمون وإعدادات التشغيل';
      case MenuType.ZAInstitution: return 'العمليات والخدمات المؤسسية';
      default: return 'مساحة العمل';
    }
  }

  get moduleIcon(): string {
    switch (this.menuItem?.menuItemId) {
      case MenuType.School: return 'fa-solid fa-school';
      case MenuType.Settings: return 'fa-solid fa-sliders';
      case MenuType.ZAInstitution: return 'fa-solid fa-building-columns';
      default: return 'fa-solid fa-grid-2';
    }
  }

  get moduleHomeRoute(): string {
    switch (this.menuItem?.menuItemId) {
      case MenuType.School: return '/admin/school/home';
      case MenuType.Settings: return '/admin/settings';
      case MenuType.ZAInstitution: return '/admin/za-institution/home';
      default: return '/admin/home';
    }
  }

  itemTitle(item: MenuSidebarItem): string {
    const key = `${this.menuItem?.menuItemId}:${item.menuItem || ''}`;
    return this.menuLabels[key] || item.displayName || 'قسم الإدارة';
  }

  itemCount(item: MenuSidebarItem): number {
    return item.subMenus?.length || 0;
  }

  toggle(): void {
    this.isOpen ? this.close() : this.open();
  }

  open(): void {
    this.isOpen = true;
    this.previousDocumentOverflow = document.documentElement.style.overflow;
    document.documentElement.style.overflow = 'hidden';
  }

  close(): void {
    if (!this.isOpen) return;
    this.isOpen = false;
    document.documentElement.style.overflow = this.previousDocumentOverflow;
  }

  ngOnDestroy(): void {
    if (this.isOpen) document.documentElement.style.overflow = this.previousDocumentOverflow;
  }
}
