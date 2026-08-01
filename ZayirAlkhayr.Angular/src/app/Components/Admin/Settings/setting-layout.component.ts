import { Component } from '@angular/core';
import { AdminHeaderComponent } from '../shared/admin-header/admin-header.component';
import { RouterOutlet } from '@angular/router';
import { MenuSidebarItem } from '../../../Models/shared/MenueSidebarItem';
import { MenueService, MenuType } from '../../../Services/shared/menue.service';
import { AdminSideMenuComponent } from '../shared/admin-side-menu/admin-side-menu.component';

@Component({
  selector: 'app-setting-layout',
  standalone: true,
  imports: [AdminHeaderComponent, AdminSideMenuComponent, RouterOutlet],
  templateUrl: './setting-layout.component.html',
  styleUrl: './setting-layout.component.css'
})
export class SettingLayoutComponent {
  menuItem: MenuSidebarItem;

  constructor(private menuService: MenueService) {
    this.menuItem = this.menuService.getMenuById(MenuType.Settings);
  }

}
