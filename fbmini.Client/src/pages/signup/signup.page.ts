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
import { AuthService } from '../../components/auth/auth.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { BackdropDialogComponent } from '../../components/backdrop/backdrop.component';
import { pop_up, PopUp } from '../../utility/popup';

function validateInput(c: FormControl) {
  const COUNT_REGEX: RegExp = /.{6,}/;
  const NUMBER_REGEX: RegExp = /\d/;
  const SPECIAL_REGEX: RegExp = /[^A-Za-z0-9]/;
  const UPPER_REGEX: RegExp = /[A-Z]/;
  const LOWER_REGEX: RegExp = /[a-z]/;

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

function validUsername(c: FormControl) {
  const NO_SPECIAL_REGEX: RegExp = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;

  if (NO_SPECIAL_REGEX.test(c.value)) return { pattern: true };

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
    MatIconModule,
  ],
  templateUrl: './signup.page.html',
  styleUrls: ['./signup.page.css'],
})
export class SignupPage {
  form: FormGroup;
  showPassword: boolean = false;
  isInputFocused: boolean = false;
  isUserInputFocused: boolean = false;
  private readonly _snackBar = inject(MatSnackBar);

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    public dialog: MatDialog
  ) {
    this.form = this.fb.group({
      username: ['', [Validators.required, validUsername]],
      password: ['', [Validators.required, validateInput]],
    });
  }

  toggleHide(event: MouseEvent) {
    this.showPassword = !this.showPassword;
    event.stopPropagation();
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
                this.router.navigate(['/user']);
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
