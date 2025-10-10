import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { FormService } from '../../../../Services/shared/form.service';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { BenefactorService } from '../../../../Services/zainstitution/benefactor.service';

@Component({
  selector: 'app-benefactor-login',
  standalone: true,
  imports: [FormsModule,NgIf],
  templateUrl: './benefactor-login.component.html',
  styleUrls: ['./benefactor-login.component.css']
})
export class BenefactorLoginComponent {
  @ViewChild('LoginForm') LoginForm: any;
  BeneFactorLogin: BeneFactorLogin = {} as BeneFactorLogin;
  ErrorMessage = '';
  ButtonDisabled = false;
  isShowPassword = false;
  constructor(private router: Router, private formService: FormService,private benefactorService: BenefactorService) {

  }

  Login() {
    this.LoginForm.onSubmit();
    const isValid = this.LoginForm.form.valid;
    if (!isValid)
      return;

    this.ButtonDisabled = true;
    this.benefactorService.BeneFactorLogin(Number(this.BeneFactorLogin.code), this.BeneFactorLogin.name).subscribe(data => {
      this.ButtonDisabled = false;
      if (data.isSuccess) {
        localStorage.setItem('BeneFactorModel', JSON.stringify(data.results));
        this.router.navigateByUrl('/benefactor-details');
      } else
        this.ErrorMessage = data.message;
    });
  }

  NumbersOnly(key: any) {
    return this.formService.NumbersOnly(key);
  }
}

export interface BeneFactorLogin {
  code: string,
  name: string,
}

