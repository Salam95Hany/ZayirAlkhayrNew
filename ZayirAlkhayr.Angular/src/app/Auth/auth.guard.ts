import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';


export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    authService.loginRedirect();
    return false;
  }

  if (authService.isSupperAdmin)
    return true;

  const pageKey = route.data['pageKey'];

  if (!pageKey) {
    router.navigateByUrl('/admin/not-authorized');
    return false;
  }

  if (!authService.hasPagePermission(pageKey)) {
    router.navigateByUrl('/admin/not-authorized');
    return false;
  }

  return true;
};
