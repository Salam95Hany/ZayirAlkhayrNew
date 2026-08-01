import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  isLoading = signal(false);
  showError = signal(false);
  showPassword = signal(false);

  loginForm: FormGroup = this.fb.group({
    userName: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  togglePassword(): void {
    this.showPassword.update(show => !show);
  }

  onSubmit(): void {
    this.loginForm.markAllAsTouched();
    if (this.loginForm.invalid || this.isLoading()) return;

    this.isLoading.set(true);
    this.showError.set(false);
    this.authService.AdminLogin(this.loginForm.value).subscribe({
      next: data => {
        this.isLoading.set(false);
        if (!data.isSuccess) {
          this.showError.set(true);
          return;
        }
        localStorage.setItem('UserModel', JSON.stringify(data.results));
        this.router.navigateByUrl('admin/home');
      },
      error: () => {
        this.isLoading.set(false);
        this.showError.set(true);
      }
    });
  }
}
