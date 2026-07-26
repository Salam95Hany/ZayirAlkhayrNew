import { Routes } from '@angular/router';
import { authGuard } from '../../../Auth/auth.guard';

export const SchoolRoutes: Routes = [
    {
        path: 'school',
        loadComponent: () => import('./school-layout.component').then(m => m.SchoolLayoutComponent),
        canActivate: [authGuard],
        data: { pageKey: 'School' },
        children: [
            {
                path: 'home',
                loadComponent: () =>
                    import('./school-home/school-home.component').then(m => m.SchoolHomeComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School' }
            },
            {
                path: 'home/:tabName',
                loadComponent: () =>
                    import('./school-home/school-home.component').then(m => m.SchoolHomeComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School' }
            },
            // ======================= Manage Students ==================================
            {
                path: 'students',
                loadComponent: () => import('./Students/manage-student/all-student/student.component').then(m => m.StudentComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_Student' }
            },
            {
                path: 'add-student',
                loadComponent: () =>
                    import('./Students/manage-student/add-student/add-student.component').then(m => m.AddStudentComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            {
                path: 'enrollment',
                loadComponent: () =>
                    import('./Students/manage-student/enrollment/enrollment.component').then(m => m.EnrollmentComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            {
                path: 'promotion',
                loadComponent: () =>
                    import('./Students/manage-student/promotion/promotion.component').then(m => m.PromotionComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            {
                path: 'withdrawals',
                loadComponent: () =>
                    import('./Students/manage-student/withdrawals/withdrawals.component').then(m => m.WithdrawalsComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            // ======================= Manage Parents ==================================
            {
                path: 'parents',
                loadComponent: () =>
                    import('./Students/manage-parent/parents/parents.component').then(m => m.ParentsComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            {
                path: 'add-parent',
                loadComponent: () =>
                    import('./Students/manage-parent/add-parent/add-parent.component').then(m => m.AddParentComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            {
                path: 'parent-template',
                loadComponent: () =>
                    import('./Students/manage-parent/parent-templates/parent-templates.component').then(m => m.ParentTemplatesComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageParents_ParentTemplate' }
            },
            // ======================= Manage Fees ==================================
            {
                path: 'student-fees',
                loadComponent: () =>
                    import('./Students/manage-fee/student-fees/student-fees.component').then(m => m.StudentFeesComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            {
                path: 'receiving-payment',
                loadComponent: () =>
                    import('./Students/manage-fee/receiving-payment/receiving-payment.component').then(m => m.ReceivingPaymentComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            {
                path: 'payment-logs',
                loadComponent: () =>
                    import('./Students/manage-fee/payment-logs/payment-logs.component').then(m => m.PaymentLogsComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            {
                path: 'debts',
                loadComponent: () =>
                    import('./Students/manage-fee/debts/debts.component').then(m => m.DebtsComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_ManageStudents_AddStudent' }
            },
            // ======================= Settings ==================================
            {
                path: 'academic-years',
                loadComponent: () => import('./Students/setting/academic-year/academic-year.component').then(m => m.AcademicYearComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Settings_AcademicYear' }
            },
            {
                path: 'academic-stages',
                loadComponent: () => import('./Students/setting/academic-stage/academic-stage.component').then(m => m.AcademicStageComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Settings_AcademicStages' }
            },
            {
                path: 'fee-types',
                loadComponent: () => import('./Students/setting/fee-type/fee-type.component').then(m => m.FeeTypeComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Settings_FeeTypes' }
            },
            {
                path: 'fee-templates',
                loadComponent: () => import('./Students/setting/fee-template/fee-template.component').then(m => m.FeeTemplateComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Settings_FeeTemplates' }
            },
            {
                path: 'discount-types',
                loadComponent: () => import('./Students/setting/discount-type/discount-type.component').then(m => m.DiscountTypeComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Settings_DiscountTypes' }
            },
            {
                path: 'nationalities',
                loadComponent: () => import('./Students/setting/student-nationality/student-nationality.component').then(m => m.StudentNationalityComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Settings_Nationalities' }
            },
            {
                path: 'student-types',
                loadComponent: () => import('./Students/setting/student-type/student-type.component').then(m => m.StudentTypeComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Settings_StudentTypes' }
            },
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: '**', redirectTo: 'home', pathMatch: 'full' },
        ]
    }
];
