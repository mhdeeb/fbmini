import { Component, inject } from '@angular/core';
import { pop_up, PopUp } from '../../utility/popup';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../auth/auth.component';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { BackdropDialogComponent } from '../backdrop/backdrop.component';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  templateUrl: './test.component.html',
  styleUrl: './test.component.css',
  imports: [MatButtonModule],
  standalone: true,
})
export class TestComponent {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    public dialog: MatDialog
  ) {}
  snackbar = inject(MatSnackBar);

  success() {
    pop_up(this.snackbar, 'Long Success Message!', PopUp.SUCCESS);
  }
  info() {
    pop_up(this.snackbar, 'Long Information Message', PopUp.INFO);
  }
  error() {
    pop_up(this.snackbar, 'Long Error Message!', PopUp.ERROR);
  }

  login(): void {
    const dialogRef = this.dialog.open(BackdropDialogComponent, {
      disableClose: true,
    });
    this.authService.login('admin', 'Admin@0', true).subscribe({
      next: (response) => {
        dialogRef.close();
        pop_up(this.snackbar, 'Successfully logged in', PopUp.SUCCESS);
      },
      error: (error: HttpErrorResponse) => {
        dialogRef.close();
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
      complete: () => {
        dialogRef.close();
      },
    });
  }

  logout(): void {
    this.authService.logout().subscribe({
      error: (error) => {
        pop_up(this.snackbar, error.error.message, PopUp.ERROR);
      },
      complete: () => {
        pop_up(this.snackbar, 'Successfully logged out', PopUp.SUCCESS);
      },
    });
  }
}
