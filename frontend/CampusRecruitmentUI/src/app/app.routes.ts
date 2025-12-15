import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login';
import { RegisterComponent } from './features/register/register';
import { DashboardComponent } from './profile/dashboard/dashboard';
import { PersonalinfoComponent } from './profile/personalinfo/personalinfo';
import { AddressComponent } from './profile/address/address';
import { EducationComponent } from './profile/education/education';
import { ProfileTabsComponent } from './profile/profile-tabs/profile-tabs';


export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  {
    path: 'dashboard',
    component: DashboardComponent,
    children: [
      {
        path: 'profile',
        component: ProfileTabsComponent,
        children: [
           {path: '', redirectTo: 'personal', pathMatch: 'full'},
           {path: 'personal', component: PersonalinfoComponent},
           {path: 'education', component: EducationComponent }, 
           {path: 'address', component: AddressComponent},
        ]
      }
    ]
  }
];

