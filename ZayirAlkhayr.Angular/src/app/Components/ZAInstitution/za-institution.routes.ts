import { Routes } from '@angular/router';

export const ZAInstitutionRoutes: Routes = [
    {
        path: 'za-institution',
        loadComponent: () =>
            import('./za-institution-layout.component').then(m => m.ZaInstitutionLayoutComponent),
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
            { path: '', redirectTo: 'home', pathMatch: 'full' },
        ]
    }
];
