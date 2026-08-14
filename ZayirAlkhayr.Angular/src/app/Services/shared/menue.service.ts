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
            {
              displayName: 'الباقي',
              menuItem: 'net-value',
              description: 'تعرف على المبلغ المتبقي بعد خصم المصروفات من الإيرادات',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/net-value',
              pageKey: 'ZAInstitution_NetValue',
            },
            {
              displayName: 'النسب',
              menuItem: 'percentage',
              description: 'إدارة النسب',
              icon: 'uil uil-sliders-v-alt',
              route: '/admin/za-institution/percentage',
              pageKey: 'ZAInstitution_Percentage',
            }
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
      menuItemId: MenuType.School,
      displayName: 'مركز بشائر القرآن',
      menuItem: 'School',
      subMenus: [
        {
          menuItemId: MenuType.School,
          displayName: 'إدارة الطلاب',
          menuItem: '1',
          description: 'إدارة بيانات الطلاب والتسجيل والترحيل',
          icon: 'fa-solid fa-user-graduate',
          route: '/admin/school/home/1',
          pageKey: 'School_ManageStudents',
          subMenus: [
            {
              displayName: 'قائمة الطلاب',
              menuItem: 'students',
              description: 'عرض والبحث وتعديل بيانات الطلاب',
              icon: 'uil uil-users-alt',
              route: '/admin/school/students',
              pageKey: 'School_ManageStudents_Student',
            },
            {
              displayName: 'تسجيل طالب جديد',
              menuItem: 'add-student',
              description: 'تسجيل طالب جديد وإضافة بياناته',
              icon: 'uil uil-user-plus',
              route: '/admin/school/add-student',
              pageKey: 'School_ManageStudents_AddStudent',
            },
            {
              displayName: 'التسجيل السنوي',
              menuItem: 'enrollment',
              description: 'تسجيل الطلاب للعام الدراسي الجديد',
              icon: 'uil uil-calendar-alt',
              route: '/admin/school/enrollment',
              pageKey: 'School_ManageStudents_Enrollment',
            },
            {
              displayName: 'الترحيل',
              menuItem: 'promotion',
              description: 'ترحيل الطلاب إلى سنة أو مرحلة جديدة',
              icon: 'uil uil-arrow-right',
              route: '/admin/school/promotion',
              pageKey: 'School_ManageStudents_Promotion',
            },
            {
              displayName: 'الطلاب المنسحبون',
              menuItem: 'withdrawals',
              description: 'متابعة الطلاب المنسحبين وأسباب الانسحاب',
              icon: 'uil uil-user-times',
              route: '/admin/school/withdrawals',
              pageKey: 'School_ManageStudents_Withdrawals',
            }
          ]
        },
        {
          menuItemId: MenuType.School,
          displayName: 'أولياء الأمور',
          menuItem: '2',
          description: 'إدارة بيانات أولياء الأمور وعلاقاتهم بالطلاب',
          icon: 'fa-solid fa-people-roof',
          route: '/admin/school/home/2',
          pageKey: 'School_ManageParents',
          subMenus: [
            {
              displayName: 'قائمة أولياء الأمور',
              menuItem: 'parents',
              description: 'عرض وإدارة بيانات أولياء الأمور',
              icon: 'uil uil-users-alt',
              route: '/admin/school/parents',
              pageKey: 'School_ManageParents_Parents',
            },
            // {
            //   displayName: 'إضافة ولي أمر',
            //   menuItem: 'add-parent',
            //   description: 'إضافة ولي أمر جديد للنظام',
            //   icon: 'uil uil-user-plus',
            //   route: '/admin/school/add-parent',
            //   pageKey: 'School_ManageParents_AddParent',
            // },
            {
              displayName: 'القوالب',
              menuItem: 'parent-template',
              description: 'عرض وإضافة قالب جديد للنظام',
              icon: 'uil uil-user-plus',
              route: '/admin/school/parent-template',
              pageKey: 'School_ManageParents_ParentTemplate',
            }
          ]
        },
        {
          menuItemId: MenuType.School,
          displayName: 'إدارة الرسوم',
          menuItem: '3',
          description: 'إدارة الرسوم الدراسية والدفعات',
          icon: 'fa-solid fa-money-bill-wave',
          route: '/admin/school/home/3',
          pageKey: 'School_ManageFee',
          subMenus: [
            {
              displayName: 'رسوم الطلاب',
              menuItem: 'student-fees',
              description: 'متابعة الرسوم المستحقة لكل طالب',
              icon: 'uil uil-money-withdraw',
              route: '/admin/school/student-fees',
              pageKey: 'School_ManageFee_StudentFees',
            },
            {
              displayName: 'استلام دفعة',
              menuItem: 'receiving-payment',
              description: 'تسجيل دفعة جديدة وطباعة الإيصال',
              icon: 'uil uil-money-bill',
              route: '/admin/school/receiving-payment',
              pageKey: 'School_ManageFee_ReceivingPayment',
            },
            {
              displayName: 'سجل الدفعات',
              menuItem: 'payment-logs',
              description: 'مراجعة جميع عمليات الدفع السابقة',
              icon: 'uil uil-receipt',
              route: '/admin/school/payment-logs',
              pageKey: 'School_ManageFee_PaymentLogs',
            },
            {
              displayName: 'المديونيات',
              menuItem: 'debts',
              description: 'متابعة الطلاب المتأخرين في السداد',
              icon: 'uil uil-wallet',
              route: '/admin/school/debts',
              pageKey: 'School_ManageFee_Debts',
            }
          ]
        },
        // {
        //   menuItemId: MenuType.School,
        //   displayName: 'التقارير',
        //   menuItem: '4',
        //   description: 'تقارير وإحصائيات النظام المختلفة',
        //   icon: 'fa-solid fa-chart-column',
        //   route: '/admin/school/home/4',
        //   pageKey: 'School_Reports',
        //   subMenus: [
        //     {
        //       displayName: 'الطلاب',
        //       menuItem: 'student-report',
        //       description: 'تقرير وإحصائيات الطلاب',
        //       icon: 'uil uil-user-square',
        //       route: '/admin/school/student-report',
        //       pageKey: 'School_Reports_Students',
        //     },
        //     {
        //       displayName: 'الإيرادات',
        //       menuItem: 'revenue-report',
        //       description: 'تقرير الإيرادات والتحصيلات',
        //       icon: 'uil uil-chart-line',
        //       route: '/admin/school/academic-stages',
        //       pageKey: 'School_Reports_Revenue',
        //     },
        //     {
        //       displayName: 'الرسوم',
        //       menuItem: 'fee-report',
        //       description: 'تقرير الرسوم الدراسية',
        //       icon: 'uil uil-money-stack',
        //       route: '/admin/school/academic-stages',
        //       pageKey: 'School_Reports_Fees',
        //     },
        //     {
        //       displayName: 'الديون',
        //       menuItem: 'debt-report',
        //       description: 'تقرير المبالغ المستحقة',
        //       icon: 'uil uil-file-exclamation-alt',
        //       route: '/admin/school/academic-stages',
        //       pageKey: 'School_Reports_Debts',
        //     },
        //      {
        //       displayName: 'الخصومات',
        //       menuItem: 'discount-report',
        //       description: 'تقرير الخصومات الممنوحة',
        //       icon: 'uil uil-percentage',
        //       route: '/admin/school/academic-stages',
        //       pageKey: 'School_Reports_Discounts',
        //     },
        //      {
        //       displayName: 'الترحيل',
        //       menuItem: 'promotion-report',
        //       description: 'تقرير عمليات ترحيل الطلاب',
        //       icon: 'uil uil-exchange-alt',
        //       route: '/admin/school/academic-stages',
        //       pageKey: 'School_Reports_Promotion',
        //     }
        //   ]
        // },
        {
          menuItemId: MenuType.School,
          displayName: 'الإعدادات',
          menuItem: '5',
          description: 'إعدادات النظام والبيانات الأساسية',
          icon: 'fa-solid fa-gears',
          route: '/admin/school/home/5',
          pageKey: 'School_Settings',
          subMenus: [
            {
              displayName: 'السنوات الدراسية',
              menuItem: 'academic-years',
              description: 'إدارة السنوات الدراسية',
              icon: 'uil uil-calendar-alt',
              route: '/admin/school/academic-years',
              pageKey: 'School_Settings_AcademicYear',
            },
            {
              displayName: 'المراحل الدراسية',
              menuItem: 'academic-stages',
              description: 'إدارة المراحل الدراسية',
              icon: 'uil uil-book-open',
              route: '/admin/school/academic-stages',
              pageKey: 'School_Settings_AcademicStages',
            },
            {
              displayName: 'أنواع الرسوم',
              menuItem: 'fee-types',
              description: 'تعريف أنواع الرسوم الدراسية',
              icon: 'uil uil-money-insert',
              route: '/admin/school/fee-types',
              pageKey: 'School_Settings_FeeTypes',
            },
            {
              displayName: 'قوالب الرسوم',
              menuItem: 'fee-templates',
              description: 'إعداد قوالب الرسوم حسب المرحلة',
              icon: 'uil uil-file-alt',
              route: '/admin/school/fee-templates',
              pageKey: 'School_Settings_FeeTemplates',
            },
            {
              displayName: 'أنواع الخصومات',
              menuItem: 'discount-types',
              description: 'تعريف أنواع الخصومات المتاحة',
              icon: 'uil uil-percentage',
              route: '/admin/school/discount-types',
              pageKey: 'School_Settings_DiscountTypes',
            },
            {
              displayName: 'الجنسيات',
              menuItem: 'nationalities',
              description: 'إدارة قائمة الجنسيات',
              icon: 'uil uil-globe',
              route: '/admin/school/nationalities',
              pageKey: 'School_Settings_Nationalities',
            },
            {
              displayName: 'أنواع الطلاب',
              menuItem: 'student-types',
              description: 'إدارة أنواع الطلاب',
              icon: 'uil uil-globe',
              route: '/admin/school/student-types',
              pageKey: 'School_Settings_StudentTypes',
            }
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
  Settings = 2,
  School = 3
}
