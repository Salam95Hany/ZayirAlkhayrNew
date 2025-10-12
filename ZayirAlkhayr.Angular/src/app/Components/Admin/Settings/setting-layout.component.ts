import { Component } from '@angular/core';
import { ZaHeaderComponent } from "../../../Shared/za-header/za-header.component";
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { MenuSidebarItem } from '../../../Models/shared/MenueSidebarItem';
import { MenueService, MenuType } from '../../../Services/shared/menue.service';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-setting-layout',
  standalone: true,
  imports: [ZaHeaderComponent, RouterOutlet,RouterLink,RouterLinkActive,NgIf,NgFor],
  templateUrl: './setting-layout.component.html',
  styleUrl: './setting-layout.component.css'
})
export class SettingLayoutComponent {
  isToggle = false;
  toggler = false;
  menuItem: MenuSidebarItem;

  constructor(private menuService: MenueService) {
    this.menuItem = this.menuService.getMenuById(MenuType.Settings);
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
