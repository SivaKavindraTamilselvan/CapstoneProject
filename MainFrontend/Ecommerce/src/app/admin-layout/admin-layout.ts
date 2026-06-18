import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  imports: [RouterOutlet,CommonModule],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
})
export class AdminLayout {
  activeMenu = signal<string | null>(null);
  toggleMenu(menu : string)
  {
    if(this.activeMenu() === menu)
    {
      this.activeMenu.set(null);
    }
    else
    {
      this.activeMenu.set(menu);
    }
  }
}
