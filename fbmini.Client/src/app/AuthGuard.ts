import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from './auth.component';
import { catchError, map } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}
  canActivate(): Observable<boolean | UrlTree> {
    return this.authService.isAuthenticated().pipe(
      map((response) => response || this.router.createUrlTree(['/login'])),
      catchError((error) => {
        console.log(error);
        return of(this.router.createUrlTree(['/login']));
      })
    );
  }
}

@Injectable({
  providedIn: 'root',
})
export class NoAuthGuard implements CanActivate {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) { }
  canActivate(): Observable<boolean | UrlTree> {
    return this.authService.isAuthenticated().pipe(
      map((response) => !(response) || this.router.createUrlTree(['/home'])),
      catchError((error) => {
        console.log(error);
        return of(this.router.createUrlTree(['/login']));
      })
    );
  }
}
