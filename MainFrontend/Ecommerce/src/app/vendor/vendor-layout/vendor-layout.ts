import { Component, signal } from '@angular/core';
import { SIDEBAR_MENU, SidebarItem } from '../../admin-sidebar.config';
import { AuthStateService } from '../../services/auth-State.Service';

@Component({
  selector: 'app-vendor-layout',
  imports: [],
  templateUrl: './vendor-layout.html',
  styleUrl: './vendor-layout.css',
})
export class VendorLayout {
  activeMenu = signal<string | null>(null);
  mobileMenuOpen = signal(false);
  menus: SidebarItem[] = [];
  constructor(private authState: AuthStateService) {
    const role = this.authState.getVendorRole();
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
  toggleMobileMenu() {
    this.mobileMenuOpen.update(v => !v);
  }

  closeMobileMenu() {
    this.mobileMenuOpen.set(false);
  }
}

