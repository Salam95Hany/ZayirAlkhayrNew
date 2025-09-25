import { Component, inject } from '@angular/core';
import { ZaHeaderComponent } from "../za-header/za-header.component";
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../Auth/auth.service';

@Component({
  selector: 'app-za-home',
  standalone: true,
  imports: [ZaHeaderComponent, CommonModule, RouterModule],
  templateUrl: './za-home.component.html',
  styleUrl: './za-home.component.css'
})
export class ZaHomeComponent {
  private authService = inject(AuthService);
  Lang = 'en';
  UserModel: any;
  customerApplications: any[] = [];
  systemURL: string; //environment.systemUrl;
  intervalClock;
  time = new Date();
  clock: string;
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

  ngOnInit(): void {
    this.UserModel = this.authService.getUserInfo();
    this.createClock();
  }


  createClock() {
    this.intervalClock = setInterval(() => {
      this.time = new Date();
      this.clock = this.time.getHours() + ':' + (this.time.getMinutes() < 10 ? '0' : '') + this.time.getMinutes()
    }, 1000);
  }
}
