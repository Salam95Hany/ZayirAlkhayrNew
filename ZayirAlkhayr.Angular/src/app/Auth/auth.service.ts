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

  get UserModel() {
    return JSON.parse(localStorage.getItem('UserModel'));
  }

  AdminLogin(model: any) {
    return this.http.post<ApiResponseModel<any>>(this.apiURL + 'Auth/AdminLogin', model);
  }

  AdminLogout(UserId: string) {
    return this.http.get<ApiResponseModel<any>>(this.apiURL + 'Auth/AdminLogout?UserId=' + UserId);
  }

  CreateSessionId() {
    let sessionId = localStorage.getItem('sessionId');
    if (sessionId) return;

    this.http.get<any>(this.apiURL + 'WebsiteHome/CreateSessionId').subscribe(data => {
      const sessionId = data.sessionId;
      localStorage.setItem('sessionId', sessionId);
    });
  }

  isAuthenticated(): boolean {
    const currentUser = this.UserModel;
    if (!currentUser || this.isTokenExpired())
      return false;
    return true;
  }

  isTokenExpired(): boolean {
    const access_token = this.UserModel?.token;
    if (!access_token) return true;
    const decode: any = jwtDecode(access_token);
    if (!decode.exp) return true;
    const expirationDate = decode.exp * 1000;
    const now = new Date().getTime();
    return expirationDate < now;
  }

  isInRole(roles: string[]): boolean {
    const userModel = this.UserModel;
    if (!userModel) return false;
    return roles.some(i => i == userModel?.role);
  }

  loginRedirect(): void {
    localStorage.removeItem('UserModel');
    this.router.navigateByUrl('/admin');
  }

  getUserInfo() {
    return this.UserModel;
  }

  get userId(): string {
    return this.UserModel?.userId;
  }

  get userName(): string {
    return this.UserModel?.userName;
  }

  get userRole(): string {
    return this.UserModel?.role;
  }
}
