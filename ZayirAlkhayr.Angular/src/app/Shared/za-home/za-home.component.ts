import { Component } from '@angular/core';
import { ZaHeaderComponent } from "../za-header/za-header.component";
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-za-home',
  standalone: true,
  imports: [ZaHeaderComponent,CommonModule,RouterModule],
  templateUrl: './za-home.component.html',
  styleUrl: './za-home.component.css'
})
export class ZaHomeComponent {
  Lang = 'en';
  UserModel: any;
  customerApplications: any[] = [];
  systemURL: string; //environment.systemUrl;

  breakpoints: any = {
    '0': {
      slidesPerView: 1
    },
    '575': {
      slidesPerView: 2
    },
    '767': {
      slidesPerView: 3
    },
    '1024': {
      slidesPerView: 4
    },
    '1200': {
      slidesPerView: 5
    },
    '1376': {
      slidesPerView: 6
    }
  };
  constructor() {

  }
  ngOnInit(): void {
    // this.UserModel = this.authService.getCurrentUser();
    this.createClock();
  }

  // ngOnInit(): void {
  //   this.UserModel = JSON.parse(localStorage.getItem('UserModel'));
  //   this.Lang = localStorage.getItem('lang') ?? 'en';
  //   this.getCustomerApplications();

  // }

  GoToProductModule(App: any) {
    // this.router.navigateByUrl(App.redirectUri);
  }
  intervalClock;
  time = new Date();
  clock: string;
  createClock() {
    this.intervalClock = setInterval(() => {
      this.time = new Date();
      this.clock = this.time.getHours() + ':' + (this.time.getMinutes() < 10 ? '0' : '') + this.time.getMinutes()
    }, 1000);
  }

  // getCustomerApplications() {
  //   this._MainService.getCustomerApplications().subscribe((data: CustomerApplicationModel[]) => {
  //     this.customerApplications = data;
  //   }, (error) => {
  //   }, () => {

  //   })
  // }
}
