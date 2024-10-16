import { Routes } from '@angular/router';
import { LayoutComponent } from '../components/layout/layout.component';
import { FormComponent } from '../components/form/form.component';
import { LoginComponent } from '../components/form/login/login.component';
import { SignupComponent } from '../components/form/signup/signup.component';
import { AuthGuard } from './AuthGuard';

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
    component: FormComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'signup',
    component: SignupComponent,
  },
];
