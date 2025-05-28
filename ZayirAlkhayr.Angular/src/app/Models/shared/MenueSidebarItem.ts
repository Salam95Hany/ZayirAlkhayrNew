export class MenuSidebarItem {
    menuItemId?:number;
    menuItem?: string;
    displayName?: string;
    description?: string;
    route?: string;
    icon?: string;
    subMenus?: MenuSidebarItem[] = [];
  }