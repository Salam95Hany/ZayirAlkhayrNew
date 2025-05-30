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
                    import('./slide-image/slide-image.component').then(m => m.SlideImageComponent),
            },
            {
                path: 'activity',
                loadComponent: () =>
                    import('./activity/activity.component').then(m => m.ActivityComponent),
            },
            {
                path: 'event',
                loadComponent: () =>
                    import('./event/event.component').then(m => m.EventComponent),
            },
            {
                path: 'photo',
                loadComponent: () =>
                    import('./photo/photo.component').then(m => m.PhotoComponent),
            },
            {
                path: 'project',
                loadComponent: () =>
                    import('./project/project.component').then(m => m.ProjectComponent),
            },
            { path: '', redirectTo: 'home', pathMatch: 'full' },
        ]
    }
];
