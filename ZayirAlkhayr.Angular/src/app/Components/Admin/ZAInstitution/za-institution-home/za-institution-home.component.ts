import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MenuSidebarItem } from '../../../../Models/shared/MenueSidebarItem';
import { MenueService, MenuType } from '../../../../Services/shared/menue.service';
import { AdminBreadcrumbComponent, AdminBreadcrumbEntry } from '../../shared/admin-breadcrumb/admin-breadcrumb.component';

type InstitutionTone = 'blue' | 'teal' | 'amber' | 'violet' | 'rose';

interface InstitutionNavigationCard {
  key: string;
  title: string;
  description: string;
  route: string;
  icon: string;
  tone: InstitutionTone;
  itemCount?: number;
}

interface NavigationCopy {
  title: string;
  description: string;
  icon: string;
}

interface InstitutionMetric {
  title: string;
  value: number;
  description: string;
  icon: string;
  tone: InstitutionTone;
}

@Component({
  selector: 'app-za-institution-home',
  standalone: true,
  imports: [CommonModule, RouterLink, AdminBreadcrumbComponent],
  templateUrl: './za-institution-home.component.html',
  styleUrl: './za-institution-home.component.css'
})
export class ZaInstitutionHomeComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly menuService = inject(MenueService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly sectionCopy: Record<string, NavigationCopy> = {
    '1': { title: 'إدارة الموقع الإلكتروني', description: 'إدارة محتوى الموقع والأنشطة والفعاليات والصور والمشروعات المنشورة.', icon: 'uil uil-window-grid' },
    '2': { title: 'إدارة المتبرعين', description: 'إدارة ملفات المتبرعين وتفاصيلهم وملاحظاتهم وتصنيفات التبرعات.', icon: 'uil uil-heart-medical' },
    '3': { title: 'إدارة المهام', description: 'تنظيم المهام العامة واليومية ومتابعة سير العمل داخل المؤسسة.', icon: 'uil uil-clipboard-notes' },
    '4': { title: 'الإدارة المالية', description: 'متابعة الإيرادات والمصروفات وصافي الحسابات المالية للمؤسسة.', icon: 'uil uil-chart-growth' },
    '5': { title: 'الخدمات الاجتماعية', description: 'إدارة ملفات الأسر والحالات والاحتياجات والتصنيفات الاجتماعية.', icon: 'uil uil-users-alt' }
  };

  private readonly actionCopy: Record<string, NavigationCopy> = {
    'slide-image': { title: 'شريط الصور', description: 'إدارة الصور الرئيسية المعروضة في واجهة الموقع.', icon: 'uil uil-images' },
    'activity': { title: 'الأنشطة', description: 'إضافة الأنشطة وتحديث تفاصيلها ومحتواها المنشور.', icon: 'uil uil-presentation-play' },
    'event': { title: 'الفعاليات', description: 'إدارة الفعاليات والمواعيد والمحتوى المرتبط بها.', icon: 'uil uil-calendar-alt' },
    'photo': { title: 'معرض الصور', description: 'تنظيم ألبومات وصور المؤسسة المعروضة للجمهور.', icon: 'uil uil-scenery' },
    'project': { title: 'المشروعات', description: 'إدارة مشروعات المؤسسة ومعلوماتها المنشورة.', icon: 'uil uil-briefcase-alt' },
    'benefactors': { title: 'قائمة المتبرعين', description: 'عرض ملفات المتبرعين والبحث عنها وإدارتها.', icon: 'uil uil-users-alt' },
    'benefactor-detail': { title: 'تفاصيل المتبرعين', description: 'إدارة البيانات والسجلات التفصيلية للمتبرعين.', icon: 'uil uil-user-square' },
    'benefactor-note': { title: 'ملاحظات المتبرعين', description: 'تسجيل ومراجعة الملاحظات المرتبطة بالمتبرعين.', icon: 'uil uil-notes' },
    'benefactor-nationality': { title: 'جنسيات المتبرعين', description: 'إدارة قائمة الجنسيات المستخدمة في ملفات المتبرعين.', icon: 'uil uil-globe' },
    'benefactor-type': { title: 'أنواع التبرعات', description: 'إدارة أنواع وتصنيفات التبرعات المعتمدة.', icon: 'uil uil-label-alt' },
    'general-tasks': { title: 'المهام العامة', description: 'إنشاء ومتابعة مهام فرق العمل داخل المؤسسة.', icon: 'uil uil-clipboard-alt' },
    'daily-tasks': { title: 'المهام اليومية', description: 'متابعة الأعمال اليومية وحالة إنجازها.', icon: 'uil uil-calendar-slash' },
    'account-import-money': { title: 'الإيرادات', description: 'تسجيل ومراجعة الإيرادات والمعاملات المالية الواردة.', icon: 'uil uil-money-insert' },
    'account-export-money': { title: 'المصروفات', description: 'تسجيل ومراجعة المصروفات والمعاملات الصادرة.', icon: 'uil uil-money-withdraw' },
    'net-value': { title: 'صافي الحسابات', description: 'متابعة الرصيد المتبقي بعد الإيرادات والمصروفات.', icon: 'uil uil-balance-scale' },
    'family-status': { title: 'ملفات الحالات', description: 'عرض وإدارة بيانات الأسر والحالات المستفيدة.', icon: 'uil uil-house-user' },
    'family-nationality': { title: 'الجنسيات', description: 'إدارة الجنسيات المستخدمة في ملفات الحالات.', icon: 'uil uil-globe' },
    'family-needs': { title: 'الاحتياجات', description: 'تعريف وإدارة أنواع الاحتياجات الاجتماعية.', icon: 'uil uil-heart-sign' },
    'family-categories': { title: 'الفئات', description: 'إدارة تصنيفات وفئات الحالات المستفيدة.', icon: 'uil uil-layer-group' },
    'family-patientTypes': { title: 'أنواع المرض', description: 'إدارة أنواع الحالات المرضية المسجلة.', icon: 'uil uil-medical-square' }
  };

  readonly metrics: InstitutionMetric[] = [
    { title: 'إجمالي الزائرين', value: 682, description: 'تفاعل مع محتوى المؤسسة', icon: 'uil uil-eye', tone: 'blue' },
    { title: 'إجمالي المتبرعين', value: 84, description: 'ملف متبرع مسجل', icon: 'uil uil-heart', tone: 'teal' },
    { title: 'المستخدمون النشطون', value: 7, description: 'حساب إداري نشط', icon: 'uil uil-user-check', tone: 'violet' }
  ];

  selectedTabName?: string;
  selectedSection?: MenuSidebarItem;
  institutionMenu?: MenuSidebarItem;
  sectionCards: InstitutionNavigationCard[] = [];
  actionCards: InstitutionNavigationCard[] = [];

  get isSectionView(): boolean {
    return !!this.selectedSection;
  }

  get displayedCards(): InstitutionNavigationCard[] {
    return this.isSectionView ? this.actionCards : this.sectionCards;
  }

  get pageTitle(): string {
    return this.selectedTabName
      ? this.sectionCopy[this.selectedTabName]?.title || 'إدارة المؤسسة'
      : 'لوحة إدارة المؤسسة';
  }

  get pageDescription(): string {
    return this.selectedTabName
      ? this.sectionCopy[this.selectedTabName]?.description || 'اختر الإجراء المطلوب للمتابعة.'
      : 'مساحة موحدة لإدارة المحتوى والمتبرعين والمهام والحسابات والخدمات الاجتماعية.';
  }

  get breadcrumbs(): AdminBreadcrumbEntry[] {
    const items: AdminBreadcrumbEntry[] = [
      { label: 'مؤسسة زائر الخير', route: this.isSectionView ? '/admin/za-institution/home' : undefined }
    ];
    if (this.isSectionView) items.push(this.pageTitle);
    return items;
  }

  get formattedDate(): string {
    return new Intl.DateTimeFormat('ar-EG-u-nu-latn', {
      weekday: 'long', day: 'numeric', month: 'long', year: 'numeric'
    }).format(new Date());
  }

  get quickActions(): InstitutionNavigationCard[] {
    const preferredKeys = ['benefactors', 'account-import-money', 'general-tasks', 'family-status'];
    const actions = (this.institutionMenu?.subMenus || []).flatMap(section => this.toActionCards(section.subMenus || []));
    return preferredKeys
      .map(key => actions.find(action => action.key === key))
      .filter((action): action is InstitutionNavigationCard => !!action);
  }

  ngOnInit(): void {
    const sourceMenu = this.menuService.getMenuById(MenuType.ZAInstitution);
    this.institutionMenu = sourceMenu
      ? this.menuService.filterMenusByUserPermissions(JSON.parse(JSON.stringify(sourceMenu)))
      : undefined;
    this.sectionCards = this.toSectionCards(this.institutionMenu?.subMenus || []);

    this.route.params
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => this.selectSection(params['tabName']));
  }

  private selectSection(tabName?: string): void {
    this.selectedTabName = tabName;
    this.selectedSection = tabName
      ? this.institutionMenu?.subMenus?.find(section => section.menuItem === tabName)
      : undefined;
    this.actionCards = this.toActionCards(this.selectedSection?.subMenus || []);
  }

  private toSectionCards(items: MenuSidebarItem[]): InstitutionNavigationCard[] {
    const tones: InstitutionTone[] = ['blue', 'teal', 'amber', 'violet', 'rose'];
    return items.map((item, index) => {
      const key = item.menuItem || String(index);
      const copy = this.sectionCopy[key];
      return {
        key,
        title: copy?.title || item.displayName || 'قسم المؤسسة',
        description: copy?.description || item.description || '',
        route: item.route || '/admin/za-institution/home',
        icon: copy?.icon || item.icon || 'uil uil-apps',
        itemCount: item.subMenus?.length || 0,
        tone: tones[index % tones.length]
      };
    });
  }

  private toActionCards(items: MenuSidebarItem[]): InstitutionNavigationCard[] {
    const tones: InstitutionTone[] = ['blue', 'teal', 'amber', 'violet', 'rose'];
    return items.map((item, index) => {
      const key = item.menuItem || String(index);
      const copy = this.actionCopy[key];
      return {
        key,
        title: copy?.title || item.displayName || 'خدمة المؤسسة',
        description: copy?.description || item.description || '',
        route: item.route || '/admin/za-institution/home',
        icon: copy?.icon || item.icon || 'uil uil-arrow-left',
        tone: tones[index % tones.length]
      };
    });
  }
}
