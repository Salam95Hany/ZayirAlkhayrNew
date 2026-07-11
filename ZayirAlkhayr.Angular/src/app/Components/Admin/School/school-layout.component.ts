import { Component } from '@angular/core';
import { ZaHeaderComponent } from '../../../Shared/za-header/za-header.component';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NgFor, NgIf } from '@angular/common';
import { MenuSidebarItem } from '../../../Models/shared/MenueSidebarItem';
import { MenueService, MenuType } from '../../../Services/shared/menue.service';

@Component({
  selector: 'app-school-layout',
  standalone: true,
  imports: [ZaHeaderComponent, RouterOutlet,RouterLink,RouterLinkActive,NgIf,NgFor],
  templateUrl: './school-layout.component.html',
  styleUrl: './school-layout.component.css'
})
export class SchoolLayoutComponent {
isToggle = false;
  toggler = false;
  menuItem: MenuSidebarItem;

  constructor(private menuService: MenueService) {
    this.menuItem = this.menuService.getMenuById(MenuType.School);
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
