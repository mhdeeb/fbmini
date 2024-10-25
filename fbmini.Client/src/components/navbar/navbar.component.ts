import { Component, HostListener } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { RouterModule } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../auth/auth.component';
import { MatDialog } from '@angular/material/dialog';
import { BackdropDialogComponent } from '../backdrop/backdrop.component';
import { pop_up, PopUp } from '../../utility/popup';
import { SearchBarComponent } from '../search-bar/search-bar.component';
import { PostEditDialog } from '../post/edit/edit.component';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { SearchDialog } from '../search-dialog/search-dialog.component';
import { SvgLogoComponent } from '../svg-logo/svg-logo.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  imports: [
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    RouterModule,
    SearchBarComponent,
    CommonModule,
    MatSidenavModule,
    SvgLogoComponent,
  ],
  standalone: true,
  animations: [
    trigger('slideInOut', [
      state('visible', style({ transform: 'translateY(0)' })),
      state('hidden', style({ transform: 'translateY(-100%)' })),
      transition('visible <=> hidden', animate('0.3s ease-in-out')),
    ]),
  ],
})
export class NavbarComponent {
  isAuth: boolean = false;
  isNavbarVisible = true;
  lastScrollTop = 0;

  constructor(
    private readonly authService: AuthService,
    public dialog: MatDialog,
    private readonly snackbar: MatSnackBar
  ) {}

  @HostListener('window:scroll', [])
  onWindowScroll() {
    const currentScroll = window.scrollY || document.documentElement.scrollTop;
    if (currentScroll > this.lastScrollTop) this.isNavbarVisible = false;
    else this.isNavbarVisible = true;
    this.lastScrollTop = currentScroll <= 0 ? 0 : currentScroll;
  }

  ngOnInit() {
    this.authService.isAuthenticated().subscribe((isAuth) => {
      this.isAuth = isAuth;
    });
  }

  createPost(): void {
    this.dialog.open(PostEditDialog, { disableClose: true });
  }

  searchUser() {
    this.dialog.open(SearchDialog);
  }

  logout(): void {
    const dialogRef = this.dialog.open(BackdropDialogComponent, {
      disableClose: true,
    });
    this.authService.logout().subscribe({
      error: (error) => {
        dialogRef.close();
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
      complete: () => {
        dialogRef.close();
        location.reload();
      },
    });
  }
}
