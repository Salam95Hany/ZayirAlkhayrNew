import { NgIf } from '@angular/common';
import { Component, DestroyRef, OnInit, TemplateRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { NgxLoadingModule } from 'ngx-loading';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../../../Auth/auth.service';
import { SettingService } from '../../../../Services/settings/setting.service';
import { AdminBreadcrumbComponent } from '../../shared/admin-breadcrumb/admin-breadcrumb.component';

function matchingPasswords(control: AbstractControl): ValidationErrors | null {
  const password = control.get('newPassword')?.value;
  const confirmation = control.get('confirmPassword')?.value;
  return password === confirmation ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [AdminBreadcrumbComponent, ReactiveFormsModule, NgIf, NgxLoadingModule, NgbModule],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css'
})
export class UserProfileComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly settingService = inject(SettingService);
  private readonly authService = inject(AuthService);
  private readonly toaster = inject(ToastrService);
  private readonly modalService = inject(NgbModal);
  private readonly destroyRef = inject(DestroyRef);

  readonly TitleList = ['الإعدادات', 'الملف الشخصي'];
  currentUser: any = null;
  showLoader = false;
  showNewPassword = false;
  showConfirmPassword = false;

  readonly profileForm = this.fb.group({
    userName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required]],
    address: ['', [Validators.required]]
  });

  readonly passwordForm = this.fb.group({
    newPassword: ['', [Validators.required, Validators.minLength(4)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: matchingPasswords });

  get userInitials(): string {
    const name = this.profileForm.controls.userName.value || this.authService.userName || 'مستخدم';
    const parts = name.trim().split(/\s+/).filter(Boolean);
    return `${parts[0]?.[0] || ''}${parts[1]?.[0] || ''}`;
  }

  get roleName(): string {
    return this.currentUser?.roleNameAr || (this.currentUser?.role === 'SupperAdmin' ? 'مدير النظام' : 'مستخدم النظام');
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  openProfileModal(content: TemplateRef<unknown>): void {
    this.patchProfileForm();
    this.profileForm.markAsPristine();
    this.profileForm.markAsUntouched();
    this.openModal(content, 'profile-edit-modal');
  }

  openPasswordModal(content: TemplateRef<unknown>): void {
    this.passwordForm.reset();
    this.showNewPassword = false;
    this.showConfirmPassword = false;
    this.openModal(content, 'password-edit-modal');
  }

  isProfileFieldInvalid(field: keyof typeof this.profileForm.controls): boolean {
    const control = this.profileForm.controls[field];
    return control.invalid && (control.dirty || control.touched);
  }

  isPasswordFieldInvalid(field: keyof typeof this.passwordForm.controls): boolean {
    const control = this.passwordForm.controls[field];
    return control.invalid && (control.dirty || control.touched);
  }

  updateProfile(): void {
    this.profileForm.markAllAsTouched();
    if (this.profileForm.invalid || !this.currentUser) return;

    const values = this.profileForm.getRawValue();
    const profile = {
      userName: values.userName?.trim(),
      email: values.email?.trim(),
      phoneNumber: values.phoneNumber?.trim(),
      address: values.address?.trim()
    };
    const model = {
      userId: this.currentUser.userId,
      ...profile,
      password: null
    };

    this.showLoader = true;
    this.settingService.EditUser(model)
      .pipe(finalize(() => this.showLoader = false), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: data => {
          if (!data.isSuccess) {
            this.toaster.error(data.message);
            return;
          }
          Object.assign(this.currentUser, profile);
          this.updateAdminSession(profile);
          this.profileForm.markAsPristine();
          this.modalService.dismissAll();
          this.toaster.success(data.message || 'تم تحديث الملف الشخصي بنجاح');
        },
        error: () => this.toaster.error('تعذر تحديث الملف الشخصي، حاول مرة أخرى')
      });
  }

  updatePassword(): void {
    this.passwordForm.markAllAsTouched();
    if (this.passwordForm.invalid || !this.currentUser) return;

    const model = {
      userId: this.currentUser.userId,
      userName: this.currentUser.userName,
      email: this.currentUser.email,
      phoneNumber: this.currentUser.phoneNumber,
      address: this.currentUser.address,
      password: this.passwordForm.controls.newPassword.value
    };

    this.showLoader = true;
    this.settingService.EditUser(model)
      .pipe(finalize(() => this.showLoader = false), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: data => {
          if (!data.isSuccess) {
            this.toaster.error(data.message);
            return;
          }
          this.passwordForm.reset();
          this.showNewPassword = false;
          this.showConfirmPassword = false;
          this.modalService.dismissAll();
          this.toaster.success(data.message || 'تم تحديث كلمة المرور بنجاح');
        },
        error: () => this.toaster.error('تعذر تحديث كلمة المرور، حاول مرة أخرى')
      });
  }

  private loadProfile(): void {
    this.showLoader = true;
    this.settingService.GetAllUsers()
      .pipe(finalize(() => this.showLoader = false), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: data => {
          const sessionUser = this.authService.getUserInfo();
          this.currentUser = data.results?.find((user: any) => user.userId === this.authService.userId) || sessionUser;
          this.patchProfileForm();
        },
        error: () => this.toaster.error('تعذر تحميل بيانات الملف الشخصي')
      });
  }

  private patchProfileForm(): void {
    this.profileForm.patchValue({
      userName: this.currentUser?.userName || '',
      email: this.currentUser?.email || '',
      phoneNumber: this.currentUser?.phoneNumber || '',
      address: this.currentUser?.address || ''
    });
  }

  private openModal(content: TemplateRef<unknown>, windowClass: string): void {
    this.modalService.open(content, {
      centered: true,
      scrollable: true,
      size: 'lg',
      windowClass
    });
  }

  private updateAdminSession(profile: Partial<{ userName: string | null; email: string | null; phoneNumber: string | null; address: string | null }>): void {
    const sessionUser = this.authService.getUserInfo();
    if (!sessionUser) return;
    Object.assign(sessionUser, profile);
    localStorage.setItem('UserModel', JSON.stringify(sessionUser));
  }
}
