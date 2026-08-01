import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../Auth/auth.service';
import { AdminHeaderComponent } from '../shared/admin-header/admin-header.component';

interface AdminModuleCard {
  title: string;
  description: string;
  route: string;
  pageKey: string;
  icon: string;
  tone: 'navy' | 'teal' | 'gold';
  actionLabel: string;
}

@Component({
  selector: 'app-admin-home',
  standalone: true,
  imports: [CommonModule, RouterLink, AdminHeaderComponent],
  templateUrl: './admin-home.component.html',
  styleUrl: './admin-home.component.css'
})
export class AdminHomeComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private clockTimer?: ReturnType<typeof setInterval>;

  userModel: any;
  currentTime = new Date();

  readonly modules: AdminModuleCard[] = [
    {
      title: 'مؤسسة زائر الخير',
      description: 'إدارة أعمال المؤسسة والخدمات والمهام والبيانات التشغيلية.',
      route: '/admin/za-institution',
      pageKey: 'ZAInstitution',
      icon: 'fa-solid fa-hand-holding-heart',
      tone: 'navy',
      actionLabel: 'دخول النظام'
    },
    {
      title: 'مركز بشائر القرآن',
      description: 'إدارة الطلاب وأولياء الأمور والرسوم والعمليات الأكاديمية.',
      route: '/admin/school',
      pageKey: 'School',
      icon: 'fa-solid fa-graduation-cap',
      tone: 'teal',
      actionLabel: 'فتح لوحة المدرسة'
    },
    {
      title: 'إعدادات النظام',
      description: 'إدارة المستخدمين والصلاحيات والنسخ الاحتياطية وإعدادات التشغيل.',
      route: '/admin/settings',
      pageKey: 'Settings',
      icon: 'fa-solid fa-gears',
      tone: 'gold',
      actionLabel: 'إدارة الإعدادات'
    }
  ];

  get visibleModules(): AdminModuleCard[] {
    return this.modules.filter(module => this.canAccess(module.pageKey));
  }

  get userDisplayName(): string {
    return this.userModel?.userName || 'مستخدم النظام';
  }

  get userRole(): string {
    return this.userModel?.role || 'مستخدم';
  }

  get formattedDate(): string {
    return new Intl.DateTimeFormat('ar-EG-u-nu-latn', {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric'
    }).format(this.currentTime);
  }

  get formattedTime(): string {
    return new Intl.DateTimeFormat('ar-EG-u-nu-latn', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    }).format(this.currentTime);
  }

  ngOnInit(): void {
    this.userModel = this.authService.getUserInfo();
    this.clockTimer = setInterval(() => this.currentTime = new Date(), 1000);
  }

  ngOnDestroy(): void {
    if (this.clockTimer) clearInterval(this.clockTimer);
  }

  private canAccess(pageKey: string): boolean {
    return this.authService.isSupperAdmin || this.authService.hasPagePermission(pageKey);
  }
}
