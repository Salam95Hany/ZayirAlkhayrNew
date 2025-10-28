import { Routes } from '@angular/router';
import { authGuard } from '../../../Auth/auth.guard';

export const SettingRoutes: Routes = [
    {
        path: 'settings',
        loadComponent: () => import('./setting-layout.component').then(m => m.SettingLayoutComponent),
        canActivate: [authGuard],
        children: [
            {
                path: 'user',
                loadComponent: () => import('./user/user.component').then(m => m.UserComponent),
                canActivate: [authGuard]
            },
            {
                path: 'backup',
                loadComponent: () => import('./backup/backup.component').then(m => m.BackupComponent),
                canActivate: [authGuard]
            },

            { path: '', redirectTo: 'user', pathMatch: 'full' },
            { path: '**', redirectTo: 'user', pathMatch: 'full' },
        ]
    }
];
