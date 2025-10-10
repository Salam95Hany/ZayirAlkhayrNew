import { Routes } from '@angular/router';

export const ZAInstitutionRoutes: Routes = [
    {
        path: 'za-institution',
        loadComponent: () => import('./za-institution-layout.component').then(m => m.ZaInstitutionLayoutComponent),
        children: [
            {
                path: 'home',
                loadComponent: () =>
                    import('./za-institution-home/za-institution-home.component').then(m => m.ZaInstitutionHomeComponent),
            },
            {
                path: 'home/:tabName',
                loadComponent: () =>
                    import('./za-institution-home/za-institution-home.component').then(m => m.ZaInstitutionHomeComponent),
            },
            {
                path: 'slide-image',
                loadComponent: () =>
                    import('./WebSite/slide-image/slide-image.component').then(m => m.SlideImageComponent),
            },
            {
                path: 'activity',
                loadComponent: () =>
                    import('./WebSite/activity/activity.component').then(m => m.ActivityComponent),
            },
            {
                path: 'event',
                loadComponent: () =>
                    import('./WebSite/event/event.component').then(m => m.EventComponent),
            },
            {
                path: 'photo',
                loadComponent: () =>
                    import('./WebSite/photo/photo.component').then(m => m.PhotoComponent),
            },
            {
                path: 'project',
                loadComponent: () =>
                    import('./WebSite/project/project.component').then(m => m.ProjectComponent),
            },
            {
                path: 'benefactors',
                loadComponent: () =>
                    import('./BeneFactors/benefactor/benefactor.component').then(m => m.BenefactorComponent),
            },
            {
                path: 'benefactor-detail',
                loadComponent: () =>
                    import('./BeneFactors/benefactor-details/benefactor-details.component').then(m => m.BenefactorDetailsComponent),
            },
            {
                path: 'benefactor-note',
                loadComponent: () =>
                    import('./BeneFactors/benefactor-notes/benefactor-notes.component').then(m => m.BenefactorNotesComponent),
            },
            {
                path: 'benefactor-nationality',
                loadComponent: () =>
                    import('./BeneFactors/benefactor-nationalities/benefactor-nationalities.component').then(m => m.BenefactorNationalitiesComponent),
            },
            {
                path: 'benefactor-type',
                loadComponent: () =>
                    import('./BeneFactors/benefactor-types/benefactor-types.component').then(m => m.BenefactorTypesComponent),
            },
            {
                path: 'account-export-money',
                loadComponent: () =>
                    import('./Tasks/account-export-mony/account-export-mony.component').then(m => m.AccountExportMonyComponent),
            },
            {
                path: 'account-import-money',
                loadComponent: () =>
                    import('./Tasks/account-import-mony/account-import-mony.component').then(m => m.AccountImportMonyComponent),
            },
            {
                path: 'daily-tasks',
                loadComponent: () =>
                    import('./Tasks/daily-tasks/daily-tasks.component').then(m => m.DailyTasksComponent),
            },
            {
                path: 'general-tasks',
                loadComponent: () =>
                    import('./Tasks/general-tasks/general-tasks.component').then(m => m.GeneralTasksComponent),
            },
            {
                path: 'family-status',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/all-family-status/all-family-status.component').then(m => m.AllFamilyStatusComponent),
            },
            {
                path: 'add-family-status',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/addFamilyStatus/add-family-status.component').then(m => m.AddFamilyStatusComponent),
            },
            {
                path: 'family-nationality',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/family-nationality/family-nationality.component').then(m => m.FamilyNationalityComponent),
            },
            {
                path: 'family-needs',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/family-needs/family-needs.component').then(m => m.FamilyNeedsComponent),
            },
            {
                path: 'family-categories',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/family-categories/family-categories.component').then(m => m.FamilyCategoriesComponent),
            },
            {
                path: 'family-patientTypes',
                loadComponent: () =>
                    import('./GeneralServices/generalStatus/family-patienttypes/family-patienttypes.component').then(m => m.FamilyPatienttypesComponent),
            },
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: '**', redirectTo: 'home', pathMatch: 'full' },
        ]
    }
];
