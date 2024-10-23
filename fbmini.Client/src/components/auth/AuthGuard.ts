import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { AuthService } from './auth.component';
import { catchError, map } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';

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
  ) {}
  canActivate(): Observable<boolean | UrlTree> {
    return this.authService.isAuthenticated().pipe(
      map((response) => !response || this.router.createUrlTree(['/home'])),
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
export class UserPreviewGuard implements CanActivate {
  constructor(
    private readonly router: Router,
    private readonly http: HttpClient
  ) {}

  canActivate(
    _route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> {
    return this.http.get<{ userName: string }>('api/user/name').pipe(
      map((result: { userName: string }) => {
        if (state.url.split('/').pop() === result.userName)
          return this.router.createUrlTree(['/profile']);
        return true;
      }),
      catchError((error) => {
        console.error(error);
        return of(this.router.createUrlTree(['/home']));
      })
    );
  }
}
