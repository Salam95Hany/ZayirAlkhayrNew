import { Routes } from '@angular/router';
import { ZaHomeComponent } from './Shared/za-home/za-home.component';
import { ZAInstitutionRoutes } from './Components/ZAInstitution/za-institution.routes';

export const routes: Routes = [
    { path: '', component: ZaHomeComponent },
    {
        path: '',
        loadComponent: () => import('./Shared/za-home/za-home.component').then(m => m.ZaHomeComponent)
    },
    ...ZAInstitutionRoutes

];
