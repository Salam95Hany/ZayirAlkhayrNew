import { Routes } from '@angular/router';

export const SettingRoutes: Routes = [
    {
        path: 'settings',
        loadComponent: () => import('./setting-layout.component').then(m => m.SettingLayoutComponent),
        children: [
            {
                path: 'user',
                loadComponent: () => import('./user/user.component').then(m => m.UserComponent),
            },
            {
                path: 'backup',
                loadComponent: () => import('./backup/backup.component').then(m => m.BackupComponent),
            },

            { path: '', redirectTo: 'user', pathMatch: 'full' },
            { path: '**', redirectTo: 'user', pathMatch: 'full' },
        ]
    }
];
