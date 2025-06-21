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
            { path: '', redirectTo: 'home', pathMatch: 'full' },
        ]
    }
];
