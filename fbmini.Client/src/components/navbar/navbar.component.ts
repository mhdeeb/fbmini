import { Component } from '@angular/core';
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
  ],
  standalone: true,
})
export class NavbarComponent {
  constructor(
    private readonly authService: AuthService,
    public dialog: MatDialog,
    private readonly snackbar: MatSnackBar
  ) {}

  createPost(): void {
    this.dialog.open(PostEditDialog, { disableClose: true });
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
