import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.UserModel?.token;
  const isLoginRequest = req.url.includes('/admin');
  let request = req;

  if (token && auth.isAuthenticated() && !isLoginRequest) {
    request = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {

      if (error.status === 401) {
        auth.loginRedirect();
      }

      return throwError(() => error);
    })
  );
};
