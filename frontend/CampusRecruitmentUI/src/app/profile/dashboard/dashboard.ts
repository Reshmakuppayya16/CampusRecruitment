import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule,RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  FullName: string = '';
  showWelcome: boolean = true;  // by default show welcome

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
  const user = this.authService.getUserData();

  if (user && user.fullName) {
    this.FullName = user.fullName;
  } else {
    this.router.navigate(['/login']);
  }
}

goToPersonal(){
  this.router.navigate(['/dashboard/personal']);
}

goToEducation(){
  this.router.navigate(['/dashboard/education']);
}

goToAddress(){
  this.router.navigate(['/dashboard/address']);
}

 onChildLoad() {
    this.showWelcome = false;  // hide welcome message when profile loads
  }


  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
