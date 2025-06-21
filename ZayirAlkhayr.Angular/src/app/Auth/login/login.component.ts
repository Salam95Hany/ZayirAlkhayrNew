import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

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

  isLoading = signal(false);
  showError = signal(false);
  loginSuccess = signal(false);
  showPassword = signal(false);
  shapes = signal(this.generateShapes());

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    rememberMe: [false]
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

  forgotPassword(event: Event) {
    event.preventDefault();
    console.log('Forgot password clicked');
  }

  goToSignup(event: Event) {
    event.preventDefault();
    console.log('Go to signup clicked');
  }

  async onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading.set(true);
      this.showError.set(false);

      try {
        await this.simulateLogin();

        this.loginSuccess.set(true);

        setTimeout(() => {
          console.log('تم تسجيل الدخول بنجاح');
          // this.router.navigate(['/dashboard']);
        }, 1500);

      } catch (error) {
        this.showError.set(true);
        setTimeout(() => this.showError.set(false), 3000);
      } finally {
        this.isLoading.set(false);
      }
    } else {
      Object.keys(this.loginForm.controls).forEach(key => {
        const control = this.loginForm.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
    }
  }

  private simulateLogin(): Promise<void> {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        if (Math.random() > 0.2) {
          resolve();
        } else {
          reject(new Error('خطأ في تسجيل الدخول'));
        }
      }, 2000);
    });
  }
}
