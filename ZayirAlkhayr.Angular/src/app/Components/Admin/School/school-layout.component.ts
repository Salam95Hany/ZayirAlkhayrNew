import { Component } from '@angular/core';
import { AdminHeaderComponent } from '../shared/admin-header/admin-header.component';
import { RouterOutlet } from '@angular/router';
import { MenuSidebarItem } from '../../../Models/shared/MenueSidebarItem';
import { MenueService, MenuType } from '../../../Services/shared/menue.service';
import { AdminSideMenuComponent } from '../shared/admin-side-menu/admin-side-menu.component';

@Component({
  selector: 'app-school-layout',
  standalone: true,
  imports: [AdminHeaderComponent, AdminSideMenuComponent, RouterOutlet],
  templateUrl: './school-layout.component.html',
  styleUrl: './school-layout.component.css'
})
export class SchoolLayoutComponent {
  menuItem: MenuSidebarItem;

  constructor(private menuService: MenueService) {
    this.menuItem = this.menuService.getMenuById(MenuType.School);
  }

}
