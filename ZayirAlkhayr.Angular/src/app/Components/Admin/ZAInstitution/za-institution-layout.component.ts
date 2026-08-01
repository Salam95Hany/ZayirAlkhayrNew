import { Component } from '@angular/core';
import { AdminHeaderComponent } from '../shared/admin-header/admin-header.component';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { MenueService, MenuType } from '../../../Services/shared/menue.service';
import { MenuSidebarItem } from '../../../Models/shared/MenueSidebarItem';
import { AdminSideMenuComponent } from '../shared/admin-side-menu/admin-side-menu.component';

@Component({
  selector: 'app-za-institution-layout',
  standalone: true,
  imports: [AdminHeaderComponent, AdminSideMenuComponent, CommonModule, RouterOutlet],
  templateUrl: './za-institution-layout.component.html',
  styleUrl: './za-institution-layout.component.css'
})
export class ZaInstitutionLayoutComponent {
  menuItem: MenuSidebarItem;

  constructor(private menuService: MenueService) {
    let menue = JSON.parse(JSON.stringify(this.menuService.getMenuById(MenuType.ZAInstitution)));
    this.menuItem = this.menuService.filterMenusByUserPermissions(menue);
  }

}
