import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { SIDEBAR_MENU, SidebarItem } from '../constant/admin-sidebar.config';
import { AuthStateService } from '../services/auth-State.Service';

@Component({
  selector: 'app-admin-layout',
  imports: [RouterOutlet, CommonModule, RouterLink],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
})
export class AdminLayout {
  activeMenu = signal<string | null>(null);
  mobileMenuOpen = signal(false);
  menus: SidebarItem[] = [];
  constructor(public authState: AuthStateService,private router : Router) {
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
  toggleMobileMenu() {
    this.mobileMenuOpen.update(v => !v);
  }

  closeMobileMenu() {
    this.mobileMenuOpen.set(false);
  }
  goToLogin() {
    this.router.navigate(["/login"]);
  }
  logout(){
    this.authState.logout();
    this.router.navigate(["/login"]);
  }
}
