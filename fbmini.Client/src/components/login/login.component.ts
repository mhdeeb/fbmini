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
import { AuthService } from '../auth/auth.component';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BackdropDialogComponent } from '../backdrop/backdrop.component';
import { MatDialog } from '@angular/material/dialog';
import { pop_up, PopUp } from '../../utility/popup';

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
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  form: FormGroup;
  showPassword: boolean = false;
  isInputFocused: boolean = false;
  private readonly _snackBar = inject(MatSnackBar);

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    public dialog: MatDialog
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  onSubmit(): void {
    const dialogRef = this.dialog.open(BackdropDialogComponent, {
      disableClose: true,
      panelClass: 'custom-dialog-container',
    });
    this.authService
      .login(this.form.get('email')?.value, this.form.get('password')?.value)
      .subscribe({
        next: (response) => {
          this.router.navigate(['/home']);
          pop_up(this._snackBar, 'Successfully logged in', PopUp.SUCCESS);
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
