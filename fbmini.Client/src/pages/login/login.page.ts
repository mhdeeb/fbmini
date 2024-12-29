import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormField, MatInputModule } from '@angular/material/input';
import { AuthService } from '../../components/auth/auth.component';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BackdropDialogComponent } from '../../components/backdrop/backdrop.component';
import { MatDialog } from '@angular/material/dialog';
import { pop_up, PopUp } from '../../utility/popup';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatInputModule,
    MatFormField,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.css'],
})
export class LoginPage {
  form: FormGroup;
  showPassword: boolean = false;
  rememberMe: boolean = false;
  isInputFocused: boolean = false;
  private readonly _snackBar = inject(MatSnackBar);

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    public dialog: MatDialog
  ) {
    this.form = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      remember_me: [false],
    });
  }

  toggleHide(event: MouseEvent) {
    this.showPassword = !this.showPassword;
    event.stopPropagation();
  }

  onSubmit(): void {
    const dialogRef = this.dialog.open(BackdropDialogComponent, {
      disableClose: true,
    });
    this.authService
      .login(
        this.form.get('username')?.value,
        this.form.get('password')?.value,
        this.form.get('remember_me')?.value
      )
      .subscribe({
        next: (response) => {
          this.router.navigate(['/home']);
          location.reload();
        },
        error: (error: HttpErrorResponse) => {
          dialogRef.close();
          pop_up(this._snackBar, error.error.message, PopUp.ERROR);
        },
        complete: () => {
          dialogRef.close();
        },
      });
  }
}
