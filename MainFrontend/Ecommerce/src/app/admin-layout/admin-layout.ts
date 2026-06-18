import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SIDEBAR_MENU, SidebarItem } from '../admin-sidebar.config';
import { AuthStateService } from '../services/auth-State.Service';

@Component({
  selector: 'app-admin-layout',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
})
export class AdminLayout {
  activeMenu = signal<string | null>(null);
  menus: SidebarItem[] = [];
  constructor(private authState: AuthStateService) {
    const role = this.authState.getAdminRole();
    console.log(role);
    if (role) {
      this.menus = SIDEBAR_MENU.filter(menu =>
        role ? menu.roles.includes(role) : false
      );
    }
  }
  toggleMenu(menu: string) {
    if (this.activeMenu() === menu) {
      this.activeMenu.set(null);
    }
    else {
      this.activeMenu.set(menu);
    }
  }
}
