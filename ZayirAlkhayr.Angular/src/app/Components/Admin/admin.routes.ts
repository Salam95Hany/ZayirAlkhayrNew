import { Routes } from '@angular/router';
import { ZAInstitutionRoutes } from './ZAInstitution/za-institution.routes';
import { SettingRoutes } from './Settings/setting.routes';

export const AdminRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('../../Auth/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'home',
        loadComponent: () => import('../../Shared/za-home/za-home.component').then(m => m.ZaHomeComponent)
    },
    {
        path: 'not-authorized',
        loadComponent: () => import('../../Auth/not-authorized/not-authorized.component').then(m => m.NotAuthorizedComponent)
    },
    ...ZAInstitutionRoutes,
    ...SettingRoutes,

    { path: '', redirectTo: '', pathMatch: 'full' },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
