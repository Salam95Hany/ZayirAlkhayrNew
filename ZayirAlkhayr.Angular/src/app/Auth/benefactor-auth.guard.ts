import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { BenefactorService } from '../Services/zainstitution/benefactor.service';

export const benefactorAuthGuard: CanActivateFn = (route, state) => {
  let benefactorAuthService = inject(BenefactorService);
  if (benefactorAuthService.isAuthenticated())
    return true;

  benefactorAuthService.loginRedirect();
  return false;
};
