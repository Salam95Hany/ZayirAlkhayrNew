import { Component } from '@angular/core';
import { ZaHeaderComponent } from "../../Shared/za-header/za-header.component";
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MenueService, MenuType } from '../../Services/shared/menue.service';
import { MenuSidebarItem } from '../../Models/shared/MenueSidebarItem';

@Component({
  selector: 'app-za-institution-layout',
  standalone: true,
  imports: [ZaHeaderComponent, CommonModule, RouterModule],
  templateUrl: './za-institution-layout.component.html',
  styleUrl: './za-institution-layout.component.css'
})
export class ZaInstitutionLayoutComponent {
  isToggle = false;
  toggler = false;
  menuItem: MenuSidebarItem;

  constructor(private menuService: MenueService) {
    this.menuItem = this.menuService.getMenuById(MenuType.ZAInstitution);
  }

  onToggleContent() {
    this.isToggle = !this.isToggle;
    const htmlElement = document.querySelector('html');
    if (this.isToggle) {
      htmlElement.style.cssText = `overflow: hidden`;
    } else {
      htmlElement.style.cssText = `overflow: auto`;
    }
  }

  onOverlayClicked() {
    this.isToggle = false;
    const htmlElement = document.querySelector('html');
    htmlElement.style.cssText = `overflow: auto`;
  }
  
  onToggler() {
    this.toggler = !this.toggler;
  }
  
  toggleMenu(menu: HTMLElement) {
    menu.classList.toggle('show');
  }

}
