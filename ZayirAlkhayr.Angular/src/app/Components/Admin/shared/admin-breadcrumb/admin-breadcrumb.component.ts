import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface AdminBreadcrumbItem {
  label: string;
  route?: string;
  icon?: string;
}

export type AdminBreadcrumbEntry = string | AdminBreadcrumbItem;

@Component({
  selector: 'app-admin-breadcrumb',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-breadcrumb.component.html',
  styleUrl: './admin-breadcrumb.component.css'
})
export class AdminBreadcrumbComponent {
  @Input() TitleList: AdminBreadcrumbEntry[] = [];

  get items(): AdminBreadcrumbItem[] {
    return (this.TitleList || [])
      .filter(Boolean)
      .map(item => typeof item === 'string' ? { label: item } : item);
  }

  isCurrent(index: number): boolean {
    return index === this.items.length - 1;
  }
}
