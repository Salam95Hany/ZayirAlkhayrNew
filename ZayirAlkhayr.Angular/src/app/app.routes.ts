import { Routes } from '@angular/router';
import { AdminRoutes } from './Components/Admin/admin.routes';
import { ProjectauthGuard } from './Components/WebSite/ProjectAuth/projectauth.guard';
import { benefactorAuthGuard } from './Auth/benefactor-auth.guard';
import { provideHttpClient, withInterceptors, withRequestsMadeViaParent } from '@angular/common/http';
import { authInterceptor } from './Auth/auth.interceptor';

export const routes: Routes = [
    {
        path: 'benefactor-login',
        loadComponent: () => import('./Components/WebSite/benefactor/benefactor-login/benefactor-login.component').then(m => m.BenefactorLoginComponent)
    },
    {
        path: 'projects/events/:id',
        loadComponent: () => import('./Components/WebSite/projects/projects.component').then(m => m.ProjectsComponent),
        canActivate: [ProjectauthGuard]
    },
    {
        path: 'project-denied/:id',
        loadComponent: () => import('./Components/WebSite/ProjectAuth/project-denied/project-denied.component').then(m => m.ProjectDeniedComponent),
    },
    {
        path: 'benefactor-details',
        loadComponent: () => import('./Components/WebSite/benefactor/benefactor-web-details/benefactor-web-details.component').then(m => m.BenefactorWebDetailsComponent),
        canActivate: [benefactorAuthGuard]
    },
    {
        path: '',
        loadComponent: () => import('./Components/WebSite/home/home.component').then(m => m.HomeComponent),
        children: [
            {
                path: 'event',
                loadComponent: () => import('./Components/WebSite/event/event.component').then(m => m.EventComponent),
            },
            {
                path: 'aboutus',
                loadComponent: () => import('./Components/WebSite/aboutus/aboutus.component').then(m => m.AboutusComponent),
            },
            {
                path: 'activity',
                loadComponent: () => import('./Components/WebSite/activity/activity.component').then(m => m.ActivityComponent),
            },
            {
                path: 'activity-details/:id',
                loadComponent: () => import('./Components/WebSite/activity-details/activity-details.component').then(m => m.ActivityDetailsComponent),
            },
            {
                path: 'photos',
                loadComponent: () => import('./Components/WebSite/photos/photos.component').then(m => m.PhotosComponent),
            },
            {
                path: 'photo-details/:id',
                loadComponent: () => import('./Components/WebSite/photo-details/photo-details.component').then(m => m.PhotoDetailsComponent),
            },
            { path: '', redirectTo: 'activity', pathMatch: 'full' }
        ]
    },
    {
        path: 'admin',
        children: [...AdminRoutes],
    }
];
