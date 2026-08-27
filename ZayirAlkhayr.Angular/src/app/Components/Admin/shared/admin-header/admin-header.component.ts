import { CommonModule } from '@angular/common';
import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs';
import { AuthService } from '../../../../Auth/auth.service';

export interface AdminNotification {
  id: string | number;
  title: string;
  message?: string;
  createdAt?: string | Date;
  route?: string;
  icon?: string;
  isRead: boolean;
  category?: 'info' | 'success' | 'warning' | 'danger';
}

@Component({
  selector: 'app-admin-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, NgbDropdownModule],
  templateUrl: './admin-header.component.html',
  styleUrl: './admin-header.component.css'
})
export class AdminHeaderComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  @Input() notifications: AdminNotification[] = [];

  userModel: any;
  mobileMenuOpen = false;
  isLoggingOut = false;
  isSuperAdmin = false;
  UserRoleName = '';

  get unreadCount(): number {
    return this.notifications.filter(notification => !notification.isRead).length;
  }

  get unreadBadge(): string {
    return this.unreadCount > 99 ? '99+' : String(this.unreadCount);
  }

  get userName(): string {
    return this.userModel?.userName || 'مستخدم النظام';
  }

  get userRole(): string {
    return this.userModel?.role || 'مستخدم';
  }

  get userInitials(): string {
    const parts = this.userName.trim().split(/\s+/).filter(Boolean);
    return (parts[0]?.[0] || '') + (parts[1]?.[0] || '');
  }

  get currentModuleName(): string {
    const url = this.router.url;
    if (url.startsWith('/admin/school')) return 'إدارة المدرسة';
    if (url.startsWith('/admin/za-institution')) return 'إدارة المؤسسة';
    if (url.startsWith('/admin/settings')) return 'إعدادات النظام';
    return 'لوحة التحكم';
  }

  ngOnInit(): void {
    this.UserRoleName = this.authService.UserRoleName;
    this.isSuperAdmin = this.authService.isSupperAdmin;
    this.userModel = this.authService.getUserInfo();
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.mobileMenuOpen = false);
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  markAsRead(notification: AdminNotification): void {
    if (notification.isRead) return;
    this.notifications = this.notifications.map(item =>
      item.id === notification.id ? { ...item, isRead: true } : item
    );
  }

  markAllAsRead(event: Event): void {
    event.stopPropagation();
    this.notifications = this.notifications.map(notification => ({ ...notification, isRead: true }));
  }

  notificationLink(notification: AdminNotification): string | null {
    return notification.route || null;
  }

  formatNotificationTime(value?: string | Date): string {
    if (!value) return '';
    const date = value instanceof Date ? value : new Date(value);
    if (Number.isNaN(date.getTime())) return '';
    return new Intl.DateTimeFormat('ar-EG-u-nu-latn', {
      day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit'
    }).format(date);
  }

  trackNotification(_: number, notification: AdminNotification): string | number {
    return notification.id;
  }

  logout(): void {
    if (this.isLoggingOut) return;
    this.isLoggingOut = true;
    this.authService.AdminLogout(this.userModel?.userId).subscribe({
      next: data => {
        if (data) this.authService.loginRedirect();
        else this.isLoggingOut = false;
      },
      error: () => this.isLoggingOut = false
    });
  }
}
