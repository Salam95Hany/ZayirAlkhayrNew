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
  shapes = signal(this.generateShapes());

  loginForm: FormGroup = this.fb.group({
    userName: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  constructor() {
    setInterval(() => {
      this.shapes.set(this.generateShapes());
    }, 3000);
  }

  private generateShapes() {
    return Array.from({ length: 6 }, () => ({
      size: Math.random() * 80 + 40,
      top: Math.random() * 100,
      left: Math.random() * 100,
      delay: Math.random() * 6
    }));
  }

  togglePassword() {
    this.showPassword.update(show => !show);
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading.set(true);
      this.showError.set(false);
      this.authService.AdminLogin(this.loginForm.value).subscribe(data => {
        if (data.isSuccess) {
          this.showError.set(false);
          this.isLoading.set(false);
          localStorage.setItem('UserModel', JSON.stringify(data.results));
          this.router.navigateByUrl('admin/home');
        } else {
          this.showError.set(true);
          this.isLoading.set(false);
        }
      });
    }
  }
}
