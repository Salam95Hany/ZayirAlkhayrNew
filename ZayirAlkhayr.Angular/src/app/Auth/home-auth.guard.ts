import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';

export const homeAuthGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
  
    if (!authService.isAuthenticated()) {
      authService.loginRedirect();
      return false;
    }
  
    return true;
};
