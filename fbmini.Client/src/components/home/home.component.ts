import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../auth/auth.component';
import { CommonModule } from '@angular/common';
import { BackdropDialogComponent } from '../backdrop/backdrop.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../utility/popup';

type User = {
  userName: string;
  email: string;
};

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterOutlet, MatButtonModule, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  constructor(
    private readonly http: HttpClient,
    private readonly authService: AuthService,
    public dialog: MatDialog
  ) {}

  public profile: User = {
    userName: '',
    email: '',
  };
  snackbar = inject(MatSnackBar);

  listProfile() {
    this.http.get<User>('api/User').subscribe({
      next: (result) => {
        this.profile = result;
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  ngOnInit() {
    this.listProfile();
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
