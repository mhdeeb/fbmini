import { RouterModule, Routes } from '@angular/router';
import { HomePage } from '../pages/home/home.page';
import { LoginPage } from '../pages/login/login.page';
import { SignupPage } from '../pages/signup/signup.page';
import { ProfilePage } from '../pages/profile/profile.page';
import {
  AuthGuard,
  NoAuthGuard,
} from '../components/auth/AuthGuard';
import { TestPage } from '../pages/test/test.page';
import { notFoundPage } from '../pages/not-found/not-found.page';
import { NgModule } from '@angular/core';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    component: HomePage,
    canActivate: [AuthGuard],
  },
  {
    path: 'profile',
    component: ProfilePage,
    canActivate: [AuthGuard],
  },
  {
    path: 'user/:username',
    component: ProfilePage,
    canActivate: [AuthGuard],
  },
  {
    path: 'login',
    component: LoginPage,
    canActivate: [NoAuthGuard],
  },
  {
    path: 'signup',
    component: SignupPage,
    canActivate: [NoAuthGuard],
  },
  {
    path: 'test',
    component: TestPage,
  },
  { path: '**', component: notFoundPage },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
