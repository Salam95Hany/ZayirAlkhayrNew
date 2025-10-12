import { Injectable } from '@angular/core';
import { MenuSidebarItem } from '../../Models/shared/MenueSidebarItem';

@Injectable({
  providedIn: 'root'
})
export class MenueService {
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
          role: ['WebSite', 'SupperAdmin', 'Admin'],
          subMenus: [
            {
              displayName: 'شريط الصور',
              menuItem: 'slide-image',
              description: 'تتبع و إدارة شريط الصور',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/slide-image',
              role: ['WebSite', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'الأنشطة',
              menuItem: 'activity',
              description: 'تتبع و إدارة الأنشطة',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/activity',
              role: ['WebSite', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'الفعاليات',
              menuItem: 'event',
              description: 'تتبع و إدارة الفعاليات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/event',
              role: ['WebSite', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'الصور',
              menuItem: 'photo',
              description: 'تتبع و إدارة الصور',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/photo',
              role: ['WebSite', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'المشاريع',
              menuItem: 'project',
              description: 'تتبع و إدارة المشاريع',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/project',
              role: ['WebSite', 'SupperAdmin', 'Admin'],
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
          role: ['BeneFactors', 'SupperAdmin', 'Admin'],
          subMenus: [
            {
              displayName: 'المتبرعين',
              menuItem: 'benefactors',
              description: 'إدارة و عرض بيانات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactors',
              role: ['BeneFactors', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'تفاصيل المتبرعين',
              menuItem: 'benefactor-detail',
              description: 'عرض تفاصيل المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactor-detail',
              role: ['BeneFactors', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'ملاحظات المتبرعين',
              menuItem: 'benefactor-note',
              description: 'عرض ملاحظات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactor-note',
              role: ['BeneFactors', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'جنسيات المتبرعين',
              menuItem: 'benefactor-nationality',
              description: 'إدارة و عرض جنسيات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactor-nationality',
              role: ['BeneFactors', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'أنواع التبرع',
              menuItem: 'benefactor-type',
              description: 'إدارة و عرض أنواع التبرع',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/benefactor-type',
              role: ['BeneFactors', 'SupperAdmin', 'Admin'],
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
          subMenus: [
            {
              displayName: 'المهام العامة',
              menuItem: 'general-tasks',
              description: 'إدارة و عرض المهام العامة',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/general-tasks',
              role: ['SupperAdmin', 'Admin'],
            },
            {
              displayName: 'المهام اليومية',
              menuItem: 'daily-tasks',
              description: 'إدارة و عرض المهام اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/daily-tasks'
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
          role: ['Accounts', 'SupperAdmin', 'Admin'],
          subMenus: [
            {
              displayName: 'الايرادات',
              menuItem: 'account-import-money',
              description: 'تتبع و إدارة الايرادات اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/account-import-money',
              role: ['Accounts', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'المصروفات',
              menuItem: 'account-export-money',
              description: 'تتبع و إدارة المصروفات اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/account-export-money',
              role: ['Accounts', 'SupperAdmin', 'Admin'],
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
          role: ['Services', 'SupperAdmin', 'Admin'],
          subMenus: [
            {
              displayName: 'حالات عامة',
              menuItem: 'family-status',
              description: 'إدارة بيانات المحتاجين ومعلوماتهم',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-status',
              role: ['Services', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'الجنسيات',
              menuItem: 'family-nationality',
              description: 'عرض وإدارة الجنسيات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-nationality',
              role: ['Services', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'الاحتياجات',
              menuItem: 'family-needs',
              description: 'عرض وإدارة الاحتياجات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-needs',
              role: ['Services', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'الفئات',
              menuItem: 'family-categories',
              description: 'عرض وإدارة الفئات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-categories',
              role: ['Services', 'SupperAdmin', 'Admin'],
            },
            {
              displayName: 'أنواع المرض',
              menuItem: 'family-patientTypes',
              description: 'عرض وإدارة أنواع المرض',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/family-patientTypes',
              role: ['Services', 'SupperAdmin', 'Admin'],
            },
          ]
        }
      ]
    },
    {
      menuItemId: MenuType.Settings,
      displayName: 'الاعدادات',
      menuItem: 'Settings',
      role: ['SupperAdmin'],
      subMenus: [
        {
          displayName: 'المستخدمين',
          menuItem: 'user',
          description: 'تتبع و إدارة المستخدمين',
          icon: 'uil uil-user',
          route: '/admin/settings/user',
          role: ['SupperAdmin'],
        },
        {
          displayName: 'النسخ الاحتياطية',
          menuItem: 'backup',
          description: 'تتبع و إدارة النسخ الاحتياطية',
          icon: 'uil uil-archive',
          route: '/admin/settings/backup',
          role: ['SupperAdmin'],
        },
      ]
    }
  ];

  filterMenusByUserRole(menus: MenuSidebarItem, userRole: string): MenuSidebarItem {
    menus.subMenus = menus.subMenus?.filter(x => x.role?.includes(userRole) || !x.role || x.role.length == 0);
    menus.subMenus?.forEach(subMenu => {
      this.filterMenusByUserRole(subMenu, userRole);
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
