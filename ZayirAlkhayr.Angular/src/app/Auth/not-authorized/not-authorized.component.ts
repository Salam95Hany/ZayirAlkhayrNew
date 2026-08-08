import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-not-authorized',
  standalone: true,
  imports: [],
  templateUrl: './not-authorized.component.html',
  styleUrl: './not-authorized.component.css'
})
export class NotAuthorizedComponent {
  constructor(
    private readonly router: Router,
    private readonly authService: AuthService
  ) { }

  get userName(): string {
    return this.authService.userName || 'مستخدم النظام';
  }

  goToHome(): void {
    this.router.navigate(['/admin/home']);
  }

  goToLogin(): void {
    this.authService.loginRedirect();
  }
}
