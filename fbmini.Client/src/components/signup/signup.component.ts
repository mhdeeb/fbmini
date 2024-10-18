import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  FormControl,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormField, MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { BackdropDialogComponent } from '../backdrop/backdrop.component';
import { pop_up, PopUp } from '../../utility/popup';

function validateInput(c: FormControl) {
  const COUNT_REGEX = /.{8,}/;
  const NUMBER_REGEX = /\d/;
  const SPECIAL_REGEX = /[^A-Za-z0-9]/;
  const UPPER_REGEX = /[A-Z]/;
  const LOWER_REGEX = /[a-z]/;

  let pattern = {
    count: !COUNT_REGEX.test(c.value),
    number: !NUMBER_REGEX.test(c.value),
    special: !SPECIAL_REGEX.test(c.value),
    upper: !UPPER_REGEX.test(c.value),
    lower: !LOWER_REGEX.test(c.value),
  };

  for (const b of Object.values(pattern)) if (b) return { pattern };

  return null;
}

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatInputModule,
    MatFormField,
    MatCheckboxModule,
    MatButtonModule,
  ],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css'],
})
export class SignupComponent {
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
      username: ['', [Validators.required]],
      password: ['', [Validators.required, validateInput]],
    });
  }

  onSubmit(): void {
    const dialogRef = this.dialog.open(BackdropDialogComponent, {
      disableClose: true,
      panelClass: 'custom-dialog-container',
    });

    this.authService
      .register(
        this.form.get('username')?.value,
        this.form.get('password')?.value
      )
      .subscribe({
        next: (res) => {
          this.authService
            .login(
              this.form.get('username')?.value,
              this.form.get('password')?.value,
              true
            )
            .subscribe({
              next: (response) => {
                this.router.navigate(['/profile']);
                pop_up(this._snackBar, 'Account Created', PopUp.SUCCESS);
              },
              error: (error: HttpErrorResponse) => {
                dialogRef.close();
                pop_up(this._snackBar, error.error.message, PopUp.ERROR);
              },
              complete: () => {
                dialogRef.close();
              },
            });
        },
        error: (error: HttpErrorResponse) => {
          dialogRef.close();
          pop_up(this._snackBar, error.error[0].description, PopUp.ERROR);
        },
        complete: () => {
          dialogRef.close();
        },
      });
  }
}
