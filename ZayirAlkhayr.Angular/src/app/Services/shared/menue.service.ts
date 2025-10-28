import { inject, Injectable } from '@angular/core';
import { MenuSidebarItem } from '../../Models/shared/MenueSidebarItem';
import { AuthService } from '../../Auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class MenueService {
  authService = inject(AuthService);
  getMenuById(menuId: MenuType, subItemName: string = null): MenuSidebarItem {
    if (subItemName) {
      return this.menus.find(x => x.menuItemId == menuId)?.subMenus?.find(x => x.menuItem == subItemName);
    }
    return this.menus.find(x => x.menuItemId == menuId);
  }
  menus: MenuSidebarItem[] = [
    {
      menuItemId: MenuType.ZAInstitution,
      displayName: 'مؤسسة زائر الخير',
      menuItem: 'ZAInstitution',
      subMenus: [
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'موقع زائر الخير',
          menuItem: '1',
          description: 'إدارة موقع زائر الخير',
          icon: 'fa-solid fa-exchange-alt',
          route: '/admin/za-institution/home/1',
          pageKey: 'ZAInstitution_ManageWebSite',
          subMenus: [
            {
              displayName: 'شريط الصور',
              menuItem: 'slide-image',
              description: 'تتبع و إدارة شريط الصور',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/slide-image',
              pageKey: 'ZAInstitution_SlideImage',
            },
            {
              displayName: 'الأنشطة',
              menuItem: 'activity',
              description: 'تتبع و إدارة الأنشطة',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/activity',
              pageKey: 'ZAInstitution_Activity',
            },
            {
              displayName: 'الفعاليات',
              menuItem: 'event',
              description: 'تتبع و إدارة الفعاليات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/event',
              pageKey: 'ZAInstitution_Event',
            },
            {
              displayName: 'الصور',
              menuItem: 'photo',
              description: 'تتبع و إدارة الصور',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/photo',
              pageKey: 'ZAInstitution_Photo',
            },
            {
              displayName: 'المشاريع',
              menuItem: 'project',
              description: 'تتبع و إدارة المشاريع',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/project',
              pageKey: 'ZAInstitution_Project',
            }
          ]
        },
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'إدارة المتبرعين',
          menuItem: '2',
          description: 'إدارة و عرض بيانات المتبرعين',
          icon: 'fa-solid fa-file-invoice-dollar',
          route: '/admin/za-institution/home/2',
          pageKey: 'ZAInstitution_ManageBenefactors',
          subMenus: [
            {
              displayName: 'المتبرعين',
              menuItem: 'benefactors',
              description: 'إدارة و عرض بيانات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactors',
              pageKey: 'ZAInstitution_Benefactors',
            },
            {
              displayName: 'تفاصيل المتبرعين',
              menuItem: 'benefactor-detail',
              description: 'عرض تفاصيل المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactor-detail',
              pageKey: 'ZAInstitution_BenefactorDetail',
            },
            {
              displayName: 'ملاحظات المتبرعين',
              menuItem: 'benefactor-note',
              description: 'عرض ملاحظات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactor-note',
              pageKey: 'ZAInstitution_BenefactorNote',
            },
            {
              displayName: 'جنسيات المتبرعين',
              menuItem: 'benefactor-nationality',
              description: 'إدارة و عرض جنسيات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactor-nationality',
              pageKey: 'ZAInstitution_BenefactorNationality',
            },
            {
              displayName: 'أنواع التبرع',
              menuItem: 'benefactor-type',
              description: 'إدارة و عرض أنواع التبرع',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactor-type',
              pageKey: 'ZAInstitution_BenefactorType',
            },
          ]
        },
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'إدارة المهام',
          menuItem: '3',
          description: 'إدارة المهام و الحسابات',
          icon: 'uil uil-list-ul',
          route: '/admin/za-institution/home/3',
          pageKey: 'ZAInstitution_ManageTasks',
          subMenus: [
            {
              displayName: 'المهام العامة',
              menuItem: 'general-tasks',
              description: 'إدارة و عرض المهام العامة',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/general-tasks',
              pageKey: 'ZAInstitution_GeneralTasks',
            },
            {
              displayName: 'المهام اليومية',
              menuItem: 'daily-tasks',
              description: 'إدارة و عرض المهام اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/daily-tasks',
              pageKey: 'ZAInstitution_DailyTasks'
            }
          ]
        },
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'إدارة الحسابات',
          menuItem: '4',
          description: 'تتبع و إدارة الحسابات',
          icon: 'uil uil-credit-card',
          route: '/admin/za-institution/home/4',
          pageKey: 'ZAInstitution_ManageAccounts',
          subMenus: [
            {
              displayName: 'الايرادات',
              menuItem: 'account-import-money',
              description: 'تتبع و إدارة الايرادات اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/account-import-money',
              pageKey: 'ZAInstitution_AccountImportMoney',
            },
            {
              displayName: 'المصروفات',
              menuItem: 'account-export-money',
              description: 'تتبع و إدارة المصروفات اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/account-export-money',
              pageKey: 'ZAInstitution_AccountExportMoney',
            },
          ]
        },
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'خدمات اجتماعية',
          menuItem: '5',
          description: 'إدارة بيانات المحتاجين',
          icon: 'fa-solid fa-users',
          route: '/admin/za-institution/home/5',
          pageKey: 'ZAInstitution_ManageServices',
          subMenus: [
            {
              displayName: 'حالات عامة',
              menuItem: 'family-status',
              description: 'إدارة بيانات المحتاجين ومعلوماتهم',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-status',
              pageKey: 'ZAInstitution_FamilyStatus',
            },
            {
              displayName: 'الجنسيات',
              menuItem: 'family-nationality',
              description: 'عرض وإدارة الجنسيات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-nationality',
              pageKey: 'ZAInstitution_FamilyNationality',
            },
            {
              displayName: 'الاحتياجات',
              menuItem: 'family-needs',
              description: 'عرض وإدارة الاحتياجات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-needs',
              pageKey: 'ZAInstitution_FamilyNeeds',
            },
            {
              displayName: 'الفئات',
              menuItem: 'family-categories',
              description: 'عرض وإدارة الفئات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-categories',
              pageKey: 'ZAInstitution_FamilyCategories',
            },
            {
              displayName: 'أنواع المرض',
              menuItem: 'family-patientTypes',
              description: 'عرض وإدارة أنواع المرض',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-patientTypes',
              pageKey: 'ZAInstitution_FamilyPatientTypes',
            },
          ]
        }
      ]
    },
    {
      menuItemId: MenuType.Settings,
      displayName: 'الاعدادات',
      menuItem: 'Settings',
      subMenus: [
        {
          displayName: 'المستخدمين',
          menuItem: 'user',
          description: 'تتبع و إدارة المستخدمين',
          icon: 'uil uil-user',
          route: '/admin/settings/user',
        },
        {
          displayName: 'النسخ الاحتياطية',
          menuItem: 'backup',
          description: 'تتبع و إدارة النسخ الاحتياطية',
          icon: 'uil uil-archive',
          route: '/admin/settings/backup',
        }
      ]
    }
  ];

  filterMenusByUserPermissions(menus: MenuSidebarItem) {
    menus.subMenus = menus.subMenus?.filter(x => this.authService.hasPageAccess(x.pageKey));
    menus.subMenus?.forEach(subMenu => {
      this.filterMenusByUserPermissions(subMenu);
    });

    return menus;
  }

  getAllRoutesFromMenu(menu: MenuSidebarItem): string[] {
    let routes: string[] = [];

    if (menu.route) {
      const lastPart = menu.route.split('/').pop();
      if (lastPart) {
        routes.push(lastPart);
      }
    }

    if (menu.subMenus && menu.subMenus.length > 0) {
      menu.subMenus.forEach(sub => {
        routes = routes.concat(this.getAllRoutesFromMenu(sub));
      });
    }

    return routes;
  }
}
export enum MenuType {
  ZAInstitution = 1,
  Settings = 2
}
