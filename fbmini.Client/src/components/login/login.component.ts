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
import { AuthService } from '../../app/auth.component';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

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
    private readonly router: Router
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, validateInput]],
    });
  }

  onSubmit(): void {
    this.authService
      .login(this.form.get('email')?.value, this.form.get('password')?.value)
      .subscribe({
        next: (response) => {
          this.router.navigate(['/home']);
        },
        error: (error: HttpErrorResponse) => {
          this._snackBar.open(error.error.message, '', {
            horizontalPosition: 'center',
            verticalPosition: 'top',
            duration: 3000,
            panelClass: 'snack-bar',
          });
          console.error('Login failed', error);
        },
      });
  }
}
