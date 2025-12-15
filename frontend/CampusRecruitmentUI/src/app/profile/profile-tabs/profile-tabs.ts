import { Component } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-profile-tabs',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './profile-tabs.html',
  styleUrl: './profile-tabs.css',
})
export class ProfileTabsComponent {

}
