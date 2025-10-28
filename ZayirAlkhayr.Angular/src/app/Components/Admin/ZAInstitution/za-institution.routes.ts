import { Routes } from '@angular/router';
import { authGuard } from '../../../Auth/auth.guard';

export const ZAInstitutionRoutes: Routes = [
    {
        path: 'za-institution',
        loadComponent: () => import('./za-institution-layout.component').then(m => m.ZaInstitutionLayoutComponent),
        canActivate: [authGuard],
        data: { pageKey: 'ZAInstitution' },
        children: [
            {
                path: 'home',
                loadComponent: () =>
                    import('./za-institution-home/za-institution-home.component').then(m => m.ZaInstitutionHomeComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution' }
            },
            {
                path: 'home/:tabName',
                loadComponent: () =>
                    import('./za-institution-home/za-institution-home.component').then(m => m.ZaInstitutionHomeComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution' }
            },
            {
                path: 'slide-image',
                loadComponent: () =>
                    import('./WebSite/slide-image/slide-image.component').then(m => m.SlideImageComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_SlideImage' }
            },
            {
                path: 'activity',
                loadComponent: () =>
                    import('./WebSite/activity/activity.component').then(m => m.ActivityComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_Activity' }
            },
            {
                path: 'event',
                loadComponent: () =>
                    import('./WebSite/event/event.component').then(m => m.EventComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_Event' }
            },
            {
                path: 'photo',
                loadComponent: () =>
                    import('./WebSite/photo/photo.component').then(m => m.PhotoComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_Photo' }
            },
            {
                path: 'project',
                loadComponent: () =>
                    import('./WebSite/project/project.component').then(m => m.ProjectComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_Project' }
            },
            {
                path: 'benefactors',
                loadComponent: () =>
                    import('./BeneFactors/benefactor/benefactor.component').then(m => m.BenefactorComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_Benefactors' }
            },
            {
                path: 'benefactor-detail',
                loadComponent: () =>
                    import('./BeneFactors/benefactor-details/benefactor-details.component').then(m => m.BenefactorDetailsComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_BenefactorDetail' }
            },
            {
                path: 'benefactor-note',
                loadComponent: () =>
                    import('./BeneFactors/benefactor-notes/benefactor-notes.component').then(m => m.BenefactorNotesComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_BenefactorNote' }
            },
            {
                path: 'benefactor-nationality',
                loadComponent: () =>
                    import('./BeneFactors/benefactor-nationalities/benefactor-nationalities.component').then(m => m.BenefactorNationalitiesComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_BenefactorNationality' }
            },
            {
                path: 'benefactor-type',
                loadComponent: () =>
                    import('./BeneFactors/benefactor-types/benefactor-types.component').then(m => m.BenefactorTypesComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_BenefactorType' }
            },
            {
                path: 'account-export-money',
                loadComponent: () =>
                    import('./Tasks/account-export-mony/account-export-mony.component').then(m => m.AccountExportMonyComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_AccountExportMoney' }
            },
            {
                path: 'account-import-money',
                loadComponent: () =>
                    import('./Tasks/account-import-mony/account-import-mony.component').then(m => m.AccountImportMonyComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_AccountImportMoney' }
            },
            {
                path: 'daily-tasks',
                loadComponent: () =>
                    import('./Tasks/daily-tasks/daily-tasks.component').then(m => m.DailyTasksComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_DailyTasks' }
            },
            {
                path: 'general-tasks',
                loadComponent: () =>
                    import('./Tasks/general-tasks/general-tasks.component').then(m => m.GeneralTasksComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_GeneralTasks' }
            },
            {
                path: 'family-status',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/all-family-status/all-family-status.component').then(m => m.AllFamilyStatusComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_FamilyStatus' }
            },
            {
                path: 'add-family-status',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/addFamilyStatus/add-family-status.component').then(m => m.AddFamilyStatusComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_FamilyStatus' }
            },
            {
                path: 'family-nationality',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/family-nationality/family-nationality.component').then(m => m.FamilyNationalityComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_FamilyNationality' }
            },
            {
                path: 'family-needs',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/family-needs/family-needs.component').then(m => m.FamilyNeedsComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_FamilyNeeds' }
            },
            {
                path: 'family-categories',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/family-categories/family-categories.component').then(m => m.FamilyCategoriesComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_FamilyCategories' }
            },
            {
                path: 'family-patientTypes',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/family-patienttypes/family-patienttypes.component').then(m => m.FamilyPatienttypesComponent),
                canActivate: [authGuard],
                data: { pageKey: 'ZAInstitution_FamilyPatientTypes' }
            },
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: '**', redirectTo: 'home', pathMatch: 'full' },
        ]
    }
];
