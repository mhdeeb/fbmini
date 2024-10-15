import { Routes } from '@angular/router';
import { LoginSignupComponent } from '../components/login-signup/login-signup.component';
import { LayoutComponent } from '../components/layout/layout.component';
import { DashboardComponent } from '../components/dashboard/dashboard.component';
import { FormComponent } from '../components/form/form.component';

export const routes: Routes = [
  {
    path: '',
    component: LoginSignupComponent
  },
  {
    path: '',
    component: LayoutComponent,
    children:
      [
        {
          path: 'dashboard',
          component: DashboardComponent
        }
      ]
  },
  {
    path: 'form',
    component: FormComponent
  }
];
