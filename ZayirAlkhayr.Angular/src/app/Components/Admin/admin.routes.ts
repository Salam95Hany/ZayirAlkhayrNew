import { Routes } from '@angular/router';
import { ZAInstitutionRoutes } from './ZAInstitution/za-institution.routes';
import { SettingRoutes } from './Settings/setting.routes';
import { homeAuthGuard } from '../../Auth/home-auth.guard';
import { SchoolRoutes } from './School/school.routes';
import { authGuard } from '../../Auth/auth.guard';

export const AdminRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('../../Auth/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'home',
        loadComponent: () => import('./admin-home/admin-home.component').then(m => m.AdminHomeComponent),
        canActivate: [homeAuthGuard]
    },
    {
        path: 'not-authorized',
        loadComponent: () => import('../../Auth/not-authorized/not-authorized.component').then(m => m.NotAuthorizedComponent)
    },
    {
        path: 'user-profile',
        loadComponent: () => import('./Settings/user-profile/user-profile.component').then(m => m.UserProfileComponent)
    },
    ...ZAInstitutionRoutes,
    ...SchoolRoutes,
    ...SettingRoutes,

    { path: '', redirectTo: '', pathMatch: 'full' },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
