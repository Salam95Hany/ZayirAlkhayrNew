import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-za-header',
  standalone: true,
  imports: [RouterLink,CommonModule,NgbModule],
  templateUrl: './za-header.component.html',
  styleUrl: './za-header.component.css'
})
export class ZaHeaderComponent {
  @Input() showToggler: boolean = true;
  @Output() toggler = new EventEmitter<boolean>();
  collapsed = true;
  showMenu: boolean = false;
  systemUrl: string; //environment.systemUrl;
  UserModel: any;
  selectedModuleName: string = 'الأنظمة';
  modulesMenu: any[] = [];
  constructor() {
    // this.modulesMenu = this.menuService.getMenuById(MenuType.MainModules)?.subMenus;
    // this.UserModel = this.authService.getCurrentUser();
    // this.routerSubscriber();
  }

  ngOnInit(): void {
  }

  routerSubscriber() {
    // this.setSelectedModule(this.router.url);

    // this.router.events.pipe(filter(event => event instanceof NavigationStart)).subscribe((event: NavigationStart) => {
    //   this.setSelectedModule(event.url);
    // });
  }
  setSelectedModule(url: string) {
    var selectedModule = url.split('/') ? url.split('/')[1] : '';
    if (selectedModule) {
      this.selectedModuleName = this.modulesMenu.find(x => x.route == `/${selectedModule}`)?.displayName;
    }
  }
  onToggler() {
    this.showMenu = !this.showMenu;
    console.log(this.toggler);
  }
  logout() {
    // this.authService.logout();
  }



}
