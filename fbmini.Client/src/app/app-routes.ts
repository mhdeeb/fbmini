import { Routes } from '@angular/router';
import { LayoutComponent } from '../components/layout/layout.component';
import { LoginComponent } from '../components/login/login.component';
import { SignupComponent } from '../components/signup/signup.component';
import { ProfileComponent } from '../components/profile/profile.component';
import { AuthGuard, NoAuthGuard } from './AuthGuard';
import { BackdropComponent } from '../components/backdrop/backdrop.component';
import { notFoundComponent } from '../components/not-found/not-found.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    component: LayoutComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [NoAuthGuard],
  },
  {
    path: 'signup',
    component: SignupComponent,
    canActivate: [NoAuthGuard],
  },
  {
    path: 'test',
    component: BackdropComponent
  },
  { path: '**', component: notFoundComponent }
];
