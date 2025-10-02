import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { ApiResponseModel } from '../Models/shared/ErrorResponseModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiURL = environment.apiUrl;
  private http = inject(HttpClient);
  private router = inject(Router);

  AdminLogin(model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Auth/AdminLogin', model);
  }

  AdminLogout(UserId: string) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'Auth/AdminLogout?UserId=' + UserId);
  }

  CreateSessionId() {
    let sessionId = localStorage.getItem('sessionId');
    if (sessionId)
      return;

    this.http.get<any>(this.apiURL + 'WebsiteHome/CreateSessionId').subscribe(data => {
      const sessionId = data.sessionId;
      localStorage.setItem('sessionId', sessionId);
    });
  }

  isAuthenticated(): boolean {
    let currentUser = JSON.parse(localStorage.getItem('UserModel'));
    if (!currentUser || this.isTokenExpired())
      return false;

    return true;
  }

  isTokenExpired(): boolean {
    let access_token = JSON.parse(localStorage.getItem('UserModel'))?.token;
    if (!access_token)
      return true;
    const decode = jwtDecode(access_token);
    if (!decode.exp)
      return true;
    const expirationDate = decode.exp * 1000;
    const now = new Date().getTime();
    return expirationDate < now;
  }

  isInRole(roles: string[]): boolean {
    let userModel = JSON.parse(localStorage.getItem('UserModel'));
    if (!userModel)
      return false;

    let ckeckRole = roles.some(i => i == userModel?.role);
    return ckeckRole;
  }

  loginRedirect(): void {
    localStorage.removeItem('UserModel');
    this.router.navigateByUrl('/login');
  }

  getUserInfo() {
    return JSON.parse(localStorage.getItem('UserModel'));
  }

  get userId(): string {
    return JSON.parse(localStorage.getItem('UserModel') ?? 'null')?.userId;
  }

  get userName(): string {
    return JSON.parse(localStorage.getItem('UserModel') ?? 'null')?.userName;
  }
}
