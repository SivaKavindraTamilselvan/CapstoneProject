import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthStateService } from '../services/auth-State.Service';

@Component({
  selector: 'app-unauthorised',
  imports: [],
  templateUrl: './unauthorised.html',
  styleUrl: './unauthorised.css',
})
export class Unauthorised {
  constructor(private router : Router,private authStateService : AuthStateService){

  }
  handleclick(){
    const id = this.authStateService.getRoleId();
    if(id == '1'){
      this.router.navigate(['/admin']);
    }
    else if(id=='3'){
      this.router.navigate(['/vendor']);
    }
    else{
      this.router.navigate(['/']);
    }
  }
}
