import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-signup',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login-signup.component.html',
  styleUrls: ['./login-signup.component.css']
})
export class LoginSignupComponent {
  activeForm: 'login' | 'register' = 'login';
  registerObj: RegisterModel = new RegisterModel();
  loginObj: LoginModel = new LoginModel();
  passwordError: boolean = false;
  emailError: boolean = false;
  showPassword: boolean = false;

  constructor(private _snackbar: MatSnackBar, private _router: Router) {}

  toggleForm(form: 'login' | 'register') {
    this.activeForm = form;
    if (form === 'register') {
      this.registerObj = new RegisterModel();
      this.passwordError = false;
      this.emailError = false;
    } else {
      this.loginObj = new LoginModel();
    }
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  registerForm() {
    let isValid = true;

    if (!this.isEmailValid(this.registerObj.email)) {
      this.emailError = true;
      isValid = false;
    } else {
      this.emailError = false;
    }

    if (!this.isPasswordValid(this.registerObj.password)) {
      this.passwordError = true;
      isValid = false;
    } else {
      this.passwordError = false;
    }

    if (!isValid) return;

    const localUsers = localStorage.getItem('users');
    const users = localUsers ? JSON.parse(localUsers) : [];

    const isUserExists = users.find((user: RegisterModel) => user.email === this.registerObj.email);
    if (isUserExists) {
      this._snackbar.open('User already exists!', 'Close', { duration: 2000 });
      return;
    }

    users.push(this.registerObj);
    localStorage.setItem('users', JSON.stringify(users));
    this._snackbar.open('User registered successfully', 'Close', { duration: 2000 });
  }

  loginForm() {
    let isValid = true;

    if (!this.isEmailValid(this.loginObj.email)) {
      this.emailError = true;
      isValid = false;
    } else {
      this.emailError = false;
    }

    if (!this.isPasswordValid(this.loginObj.password)) {
      this.passwordError = true;
      isValid = false;
    } else {
      this.passwordError = false;
    }

    if (!isValid) return;

    const localUsers = localStorage.getItem('users');
    if (localUsers) {
      const users = JSON.parse(localUsers);
      const isUserExist = users.find((user: RegisterModel) => user.email === this.loginObj.email && user.password === this.loginObj.password);

      if (isUserExist) {
        this._snackbar.open('Login successful', 'Close', { duration: 2000 });
        localStorage.setItem('loggedUser', JSON.stringify(isUserExist));
        this._router.navigateByUrl('/dashboard');
      } else {
        this._snackbar.open('Email or password is incorrect!', 'Close', { duration: 2000 });
      }
    } else {
      this._snackbar.open('No users registered yet.', 'Close', { duration: 2000 });
    }
  }

  isPasswordValid(password: string): boolean {
    const uppercasePattern = /[A-Z]/;
    const numberPattern = /[0-9]/;
    return uppercasePattern.test(password) && numberPattern.test(password);
  }

  isEmailValid(email: string): boolean {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
  }
}

export class RegisterModel {
  name: string;
  email: string;
  password: string;

  constructor() {
    this.name = '';
    this.email = '';
    this.password = '';
  }
}

export class LoginModel {
  email: string;
  password: string;

  constructor() {
    this.email = '';
    this.password = '';
  }
}
