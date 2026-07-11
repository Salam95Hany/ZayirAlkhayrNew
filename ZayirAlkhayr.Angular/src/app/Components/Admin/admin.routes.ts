import { Routes } from '@angular/router';
import { ZAInstitutionRoutes } from './ZAInstitution/za-institution.routes';
import { SettingRoutes } from './Settings/setting.routes';
import { homeAuthGuard } from '../../Auth/home-auth.guard';
import { SchoolRoutes } from './School/school.routes';

export const AdminRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('../../Auth/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'home',
        loadComponent: () => import('../../Shared/za-home/za-home.component').then(m => m.ZaHomeComponent),
        canActivate: [homeAuthGuard]
    },
    {
        path: 'not-authorized',
        loadComponent: () => import('../../Auth/not-authorized/not-authorized.component').then(m => m.NotAuthorizedComponent)
    },
    ...ZAInstitutionRoutes,
    ...SchoolRoutes,
    ...SettingRoutes,

    { path: '', redirectTo: '', pathMatch: 'full' },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
