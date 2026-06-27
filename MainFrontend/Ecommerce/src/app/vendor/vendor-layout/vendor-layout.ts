import { Component, signal } from '@angular/core';
import { AuthStateService } from '../../services/auth-State.Service';
import { VENDOR_SIDEBAR_MENU, VendorSidebarItem } from '../../constant/vendor-sidebar.config';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-vendor-layout',
  imports: [RouterOutlet, CommonModule, RouterLink],
  templateUrl: './vendor-layout.html',
  styleUrl: './vendor-layout.css',
})
export class VendorLayout {
  activeMenu = signal<string | null>(null);
  mobileMenuOpen = signal(false);
  menus: VendorSidebarItem[] = [];
  constructor(private authState: AuthStateService) {
    const role = this.authState.getVendorRole();
    console.log(role);
    if (role) {
      this.menus = VENDOR_SIDEBAR_MENU.filter(menu =>
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

