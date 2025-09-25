import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../Auth/auth.service';

@Component({
  selector: 'app-za-header',
  standalone: true,
  imports: [RouterLink,CommonModule,NgbModule],
  templateUrl: './za-header.component.html',
  styleUrl: './za-header.component.css'
})
export class ZaHeaderComponent {
  private authService = inject(AuthService);
  @Input() showToggler: boolean = true;
  @Output() toggler = new EventEmitter<boolean>();
  collapsed = true;
  showMenu: boolean = false;
  systemUrl: string; //environment.systemUrl;
  UserModel: any;
  selectedModuleName: string = 'الأنظمة';
  modulesMenu: any[] = [];

  ngOnInit(): void {
    this.UserModel = this.authService.getUserInfo();
  }

  setSelectedModule(url: string) {
    var selectedModule = url.split('/') ? url.split('/')[1] : '';
    if (selectedModule) {
      this.selectedModuleName = this.modulesMenu.find(x => x.route == `/${selectedModule}`)?.displayName;
    }
  }

  onToggler() {
    this.showMenu = !this.showMenu;
  }

  logout() {
    // this.authService.logout();
  }



}
