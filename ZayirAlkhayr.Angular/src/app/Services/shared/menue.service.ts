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
          route: '/za-institution/home/1',
          subMenus: [
            {
              displayName: 'شريط الصور',
              menuItem: 'slide-image',
              description: 'تتبع و إدارة شريط الصور',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/slide-image'
            },
            {
              displayName: 'الأنشطة',
              menuItem: 'activity',
              description: 'تتبع و إدارة الأنشطة',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/activity'
            },
            {
              displayName: 'الفعاليات',
              menuItem: 'event',
              description: 'تتبع و إدارة الفعاليات',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/event'
            },
            {
              displayName: 'الصور',
              menuItem: 'photo',
              description: 'تتبع و إدارة الصور',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/photo'
            },
            {
              displayName: 'المشاريع',
              menuItem: 'project',
              description: 'تتبع و إدارة المشاريع',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/project'
            }
          ]
        },
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'إدارة المتبرعين',
          menuItem: '2',
          description: 'إدارة و عرض بيانات المتبرعين',
          icon: 'fa-solid fa-file-invoice-dollar',
          route: '/za-institution/home/2',
          subMenus: [
            {
              displayName: 'المتبرعين',
              menuItem: 'benefactors',
              description: 'إدارة و عرض بيانات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/benefactors'
            },
            {
              displayName: 'تفاصيل المتبرعين',
              menuItem: 'benefactor-detail',
              description: 'عرض تفاصيل المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/benefactor-detail'
            },
            {
              displayName: 'ملاحظات المتبرعين',
              menuItem: 'benefactor-note',
              description: 'عرض ملاحظات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/benefactor-note'
            },
            {
              displayName: 'جنسيات المتبرعين',
              menuItem: 'benefactor-nationality',
              description: 'إدارة و عرض جنسيات المتبرعين',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/benefactor-nationality'
            },
            {
              displayName: 'أنواع التبرع',
              menuItem: 'benefactor-type',
              description: 'إدارة و عرض أنواع التبرع',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/benefactor-type'
            },
          ]
        },
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'إدارة المهام',
          menuItem: '3',
          description: 'إدارة المهام و الحسابات',
          icon: 'uil uil-sliders-v-alt',
          route: '/za-institution/home/3',

          subMenus: [
            {
              displayName: 'المهام العامة',
              menuItem: 'general-tasks',
              description: 'إدارة و عرض المهام العامة',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/general-tasks'
            },
            {
              displayName: 'المهام اليومية',
              menuItem: 'daily-tasks',
              description: 'إدارة و عرض المهام اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/daily-tasks'
            },
            {
              displayName: 'الايرادات',
              menuItem: 'account-import-money',
              description: 'تتبع و إدارة الايرادات اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/account-import-money'
            },
            {
              displayName: 'الصادرات',
              menuItem: 'account-export-money',
              description: 'تتبع و إدارة الصادرات اليومية',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/account-export-money'
            },
          ]
        },
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'إدارة الحالات',
          menuItem: '4',
          description: 'إدارة بيانات المحتاجين',
          icon: 'fa-solid fa-users',
          route: '/za-institution/home/4',
          subMenus: [
            {
              displayName: 'حالات عامة',
              menuItem: 'family-status',
              description: 'إدارة بيانات المحتاجين ومعلوماتهم',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/family-status'
            },
            {
              displayName: 'الجنسيات',
              menuItem: 'family-nationality',
              description: 'عرض وإدارة الجنسيات',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/family-nationality'
            },
            {
              displayName: 'الاحتياجات',
              menuItem: 'family-needs',
              description: 'عرض وإدارة الاحتياجات',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/family-needs'
            },
            {
              displayName: 'الفئات',
              menuItem: 'family-categories',
              description: 'عرض وإدارة الفئات',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/family-categories'
            },
            {
              displayName: 'أنواع المرض',
              menuItem: 'family-patientTypes',
              description: 'عرض وإدارة أنواع المرض',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/family-patientTypes'
            },
          ]
        },
        {
          menuItemId: MenuType.ZAInstitution,
          displayName: 'الاعدادات',
          menuItem: '5',
          description: 'التحكم في المستخدمين والنسخ الاحتياطية',
          icon: 'fa-solid fa-tools',
          route: '/za-institution/home/5',

          subMenus: [
            {
              displayName: 'المستخدمين',
              menuItem: 'user',
              description: 'عرض وإدارة المستخدمين',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/user'
            },
            {
              displayName: 'النسخ الاحتياطية',
              menuItem: 'backup',
              description: 'عرض وإدارة النسخ الاحتياطية',
              icon: 'uil uil-sliders-v-alt',
              route: '/za-institution/backup'
            }
          ]
        }
      ]
    }
  ];
}
export enum MenuType {
  ZAInstitution = 1,
}
