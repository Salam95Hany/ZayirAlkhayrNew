import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MenuSidebarItem } from '../../../../Models/shared/MenueSidebarItem';
import { MenueService, MenuType } from '../../../../Services/shared/menue.service';
import { AdminBreadcrumbComponent, AdminBreadcrumbEntry } from '../../shared/admin-breadcrumb/admin-breadcrumb.component';
import { AuthService } from '../../../../Auth/auth.service';

interface SchoolNavigationCard {
  key: string;
  title: string;
  description: string;
  route: string;
  icon: string;
  pageKey?: string;
  itemCount?: number;
  tone: 'blue' | 'teal' | 'amber' | 'violet';
}

interface NavigationCopy {
  title: string;
  description: string;
}

@Component({
  selector: 'app-school-home',
  standalone: true,
  imports: [CommonModule, RouterLink, AdminBreadcrumbComponent],
  templateUrl: './school-home.component.html',
  styleUrl: './school-home.component.css'
})
export class SchoolHomeComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly menuService = inject(MenueService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly authService = inject(AuthService);

  private readonly sectionCopy: Record<string, NavigationCopy> = {
    '1': { title: 'إدارة الطلاب', description: 'إدارة ملفات الطلاب والتسجيل السنوي والترحيل والانسحابات.' },
    '2': { title: 'أولياء الأمور', description: 'إدارة بيانات أولياء الأمور وقنوات التواصل والقوالب.' },
    '3': { title: 'إدارة الرسوم', description: 'متابعة الرسوم والمدفوعات والإيصالات والمديونيات.' },
    '5': { title: 'إعدادات المدرسة', description: 'تهيئة السنوات والمراحل وأنواع الرسوم والبيانات الأساسية.' }
  };

  private readonly actionCopy: Record<string, NavigationCopy> = {
    'students': { title: 'قائمة الطلاب', description: 'عرض ملفات الطلاب والبحث عنها وإدارتها.' },
    'add-student': { title: 'تسجيل طالب جديد', description: 'إنشاء ملف طالب وإضافة بياناته الأساسية.' },
    'enrollment': { title: 'التسجيل السنوي', description: 'تسجيل الطلاب في العام الدراسي الجديد.' },
    'promotion': { title: 'ترحيل الطلاب', description: 'ترحيل الطلاب إلى الصف أو المرحلة التالية.' },
    'withdrawals': { title: 'الطلاب المنسحبون', description: 'متابعة طلبات الانسحاب وأسبابها.' },
    'parents': { title: 'قائمة أولياء الأمور', description: 'عرض بيانات أولياء الأمور وعلاقات الطلاب.' },
    'parent-template': { title: 'قوالب أولياء الأمور', description: 'إدارة قوالب الرسائل والتواصل المعتمدة.' },
    'student-fees': { title: 'رسوم الطلاب', description: 'متابعة الرسوم المستحقة لكل طالب.' },
    'receiving-payment': { title: 'استلام دفعة', description: 'تسجيل دفعة جديدة وإصدار إيصال السداد.' },
    'payment-logs': { title: 'سجل الدفعات', description: 'مراجعة عمليات السداد والإيصالات السابقة.' },
    'debts': { title: 'المديونيات', description: 'متابعة الأرصدة والمتأخرات المستحقة.' },
    'academic-years': { title: 'السنوات الدراسية', description: 'إدارة السنوات الدراسية وحالتها.' },
    'academic-stages': { title: 'المراحل الدراسية', description: 'تعريف المراحل والصفوف الدراسية.' },
    'fee-types': { title: 'أنواع الرسوم', description: 'تعريف بنود الرسوم الدراسية.' },
    'fee-templates': { title: 'قوالب الرسوم', description: 'إعداد قوالب الرسوم حسب المرحلة.' },
    'discount-types': { title: 'أنواع الخصومات', description: 'إدارة سياسات وأنواع الخصومات.' },
    'nationalities': { title: 'الجنسيات', description: 'إدارة قائمة الجنسيات المعتمدة.' },
    'student-types': { title: 'أنواع الطلاب', description: 'تعريف تصنيفات وأنواع الطلاب.' }
  };

  selectedTabName?: string;
  selectedSection?: MenuSidebarItem;
  schoolMenu?: MenuSidebarItem;
  sectionCards: SchoolNavigationCard[] = [];
  actionCards: SchoolNavigationCard[] = [];
  UserRoleName = '';

  get isSectionView(): boolean {
    return !!this.selectedSection;
  }

  get displayedCards(): SchoolNavigationCard[] {
    return this.isSectionView ? this.actionCards : this.sectionCards;
  }

  get pageTitle(): string {
    if (!this.selectedTabName) return 'لوحة إدارة المدرسة';
    return this.sectionCopy[this.selectedTabName]?.title || 'إدارة المدرسة';
  }

  get pageDescription(): string {
    if (!this.selectedTabName) {
      return 'مساحة موحدة لإدارة الطلاب وأولياء الأمور والرسوم والإعدادات الأكاديمية.';
    }
    return this.sectionCopy[this.selectedTabName]?.description || 'اختر الإجراء المطلوب للمتابعة.';
  }

  get breadcrumbs(): AdminBreadcrumbEntry[] {
    const items: AdminBreadcrumbEntry[] = [
      { label: 'إدارة المدرسة', route: this.isSectionView ? '/admin/school/home' : undefined }
    ];
    if (this.isSectionView) items.push(this.pageTitle);
    return items;
  }

  get formattedDate(): string {
    return new Intl.DateTimeFormat('ar-EG-u-nu-latn', {
      weekday: 'long', day: 'numeric', month: 'long', year: 'numeric'
    }).format(new Date());
  }

  get quickActions(): SchoolNavigationCard[] {
    const preferredKeys = ['add-student', 'receiving-payment', 'students', 'payment-logs'];
    const availableActions = this.sectionCards.flatMap(section => this.toActionCards(
      this.schoolMenu?.subMenus?.find(item => item.menuItem === section.key)?.subMenus || []
    ));
    return preferredKeys
      .map(key => availableActions.find(action => action.key === key))
      .filter((action): action is SchoolNavigationCard => !!action)
      .slice(0, 4);
  }

  ngOnInit(): void {
    this.UserRoleName = this.authService.UserRoleName;
    const sourceMenu = this.menuService.getMenuById(MenuType.School);
    this.schoolMenu = sourceMenu
      ? this.menuService.filterMenusByUserPermissions(JSON.parse(JSON.stringify(sourceMenu)))
      : undefined;
    this.sectionCards = this.toSectionCards(this.schoolMenu?.subMenus || []);

    this.route.params
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => this.selectSection(params['tabName']));
  }

  private selectSection(tabName?: string): void {
    this.selectedTabName = tabName;
    this.selectedSection = tabName
      ? this.schoolMenu?.subMenus?.find(section => section.menuItem === tabName)
      : undefined;
    this.actionCards = this.toActionCards(this.selectedSection?.subMenus || []);
  }

  private toSectionCards(items: MenuSidebarItem[]): SchoolNavigationCard[] {
    const tones: SchoolNavigationCard['tone'][] = ['blue', 'teal', 'amber', 'violet'];
    return items.map((item, index) => {
      const key = item.menuItem || String(index);
      const copy = this.sectionCopy[key];
      return {
        key,
        title: copy?.title || item.displayName || 'قسم المدرسة',
        description: copy?.description || item.description || '',
        route: item.route || '/admin/school/home',
        icon: item.icon || 'fa-solid fa-layer-group',
        pageKey: item.pageKey,
        itemCount: item.subMenus?.length || 0,
        tone: tones[index % tones.length]
      };
    });
  }

  private toActionCards(items: MenuSidebarItem[]): SchoolNavigationCard[] {
    const tones: SchoolNavigationCard['tone'][] = ['blue', 'teal', 'amber', 'violet'];
    return items.map((item, index) => {
      const key = item.menuItem || String(index);
      const copy = this.actionCopy[key];
      return {
        key,
        title: copy?.title || item.displayName || 'إجراء',
        description: copy?.description || item.description || '',
        route: item.route || '/admin/school/home',
        icon: item.icon || 'fa-solid fa-arrow-left',
        pageKey: item.pageKey,
        tone: tones[index % tones.length]
      };
    });
  }
}
