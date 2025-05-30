import { Routes } from '@angular/router';
import { ZAInstitutionRoutes } from './Components/ZAInstitution/za-institution.routes';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./Auth/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'home',
        loadComponent: () => import('./Shared/za-home/za-home.component').then(m => m.ZaHomeComponent)
    },
    ...ZAInstitutionRoutes

];
