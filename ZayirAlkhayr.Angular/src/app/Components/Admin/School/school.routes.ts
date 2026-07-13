import { Routes } from '@angular/router';
import { authGuard } from '../../../Auth/auth.guard';

export const SchoolRoutes: Routes = [
    {
        path: 'school',
        loadComponent: () => import('./school-layout.component').then(m => m.SchoolLayoutComponent),
        canActivate: [authGuard],
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
              {
                path: 'add-student',
                loadComponent: () =>
                    import('./addStudent/add-student.component').then(m => m.AddStudentComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Student' }
            },
            {
                path: 'students',
                loadComponent: () => import('./student/student.component').then(m => m.StudentComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Student' }
            },
            {
                path: 'academic-stages',
                loadComponent: () => import('./academic-stage/academic-stage.component').then(m => m.AcademicStageComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_AcademicStages' }
            },
            {
                path: 'discount-types',
                loadComponent: () => import('./discount-type/discount-type.component').then(m => m.DiscountTypeComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_DiscountTypes' }
            },
            {
                path: 'nationalities',
                loadComponent: () => import('./student-nationality/student-nationality.component').then(m => m.StudentNationalityComponent),
                canActivate: [authGuard],
                data: { pageKey: 'School_Nationalities' }
            },

            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: '**', redirectTo: 'home', pathMatch: 'full' },
        ]
    }
];
