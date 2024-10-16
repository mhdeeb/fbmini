import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
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
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

//const uppercasePattern = /[A-Z]/;
//const numberPattern = /[0-9]/;
//const specialPattern = /[!@#\$%\^\&*\)\(+=._-]+/;
//const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
const passwordPattern =
  /(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/;

//type RegisterModel = {
//  name: string;
//  email: string;
//  password: string;
//}

//type LoginModel = {
//  email: string;
//  password: string;
//}

//enum Form {
//  Login = 'LOGIN',
//  Register = 'REGISTER'
//}

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
  title = 'Sign In';
  form: FormGroup;
  showPassword: boolean = false;
  isInputFocused: boolean = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, validateInput]],
    });
  }

  sendCredential(email: string, password: string) {
    const url = '/api/account/register';
    const body = {
      email,
      password,
    };
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    return this.http.post(url, body, { headers });
  }

  onSubmit(): void {
    this.sendCredential(
      this.form.get('email')?.value,
      this.form.get('password')?.value
    ).subscribe({
      next: (res) => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  onFocus(isFocused: boolean): void {
    this.isInputFocused = isFocused;
  }
  //public Form = Form;

  //activeForm: Form = Form.Login;
  //showPassword: boolean = false;
  //registerObj: RegisterModel;

  //toggleForm(form: Form) {
  //  this.activeForm = form;
  //}

  //togglePasswordVisibility() {
  //  this.showPassword = !this.showPassword;
  //}

  //registerForm() {
  //  //let isValid = true;

  //  //if (!this.isEmailValid(this.registerObj.email)) {
  //  //  this.emailError = true;
  //  //  isValid = false;
  //  //} else {
  //  //  this.emailError = false;
  //  //}

  //  //if (!this.isPasswordValid(this.registerObj.password)) {
  //  //  this.passwordError = true;
  //  //  isValid = false;
  //  //} else {
  //  //  this.passwordError = false;
  //  //}

  //  //if (!isValid) return;

  //  //const localUsers = localStorage.getItem('users');
  //  //const users = localUsers ? JSON.parse(localUsers) : [];

  //  //const isUserExists = users.find((user: RegisterModel) => user.email === this.registerObj.email);
  //  //if (isUserExists) {
  //  //  this._snackbar.open('User already exists!', 'Close', { duration: 2000 });
  //  //  return;
  //  //}

  //  //users.push(this.registerObj);
  //  //localStorage.setItem('users', JSON.stringify(users));
  //  //this._snackbar.open('User registered successfully', 'Close', { duration: 2000 });
  //}

  //loginForm() {
  //  //let isValid = true;

  //  //if (!this.isEmailValid(this.loginObj.email)) {
  //  //  this.emailError = true;
  //  //  isValid = false;
  //  //} else {
  //  //  this.emailError = false;
  //  //}

  //  //if (!this.isPasswordValid(this.loginObj.password)) {
  //  //  this.passwordError = true;
  //  //  isValid = false;
  //  //} else {
  //  //  this.passwordError = false;
  //  //}

  //  //if (!isValid) return;

  //  //const localUsers = localStorage.getItem('users');
  //  //if (localUsers) {
  //  //  const users = JSON.parse(localUsers);
  //  //  const isUserExist = users.find((user: RegisterModel) => user.email === this.loginObj.email && user.password === this.loginObj.password);

  //  //  if (isUserExist) {
  //  //    this._snackbar.open('Login successful', 'Close', { duration: 2000 });
  //  //    localStorage.setItem('loggedUser', JSON.stringify(isUserExist));
  //  //    this._router.navigateByUrl('/dashboard');
  //  //  } else {
  //  //    this._snackbar.open('Email or password is incorrect!', 'Close', { duration: 2000 });
  //  //  }
  //  //} else {
  //  //  this._snackbar.open('No users registered yet.', 'Close', { duration: 2000 });
  //  //}
  //}

  //isPasswordValid(password: string): boolean {
  //  return uppercasePattern.test(password) && numberPattern.test(password) && specialPattern.test(password);
  //}

  //isEmailValid(email: string): boolean {
  //  return emailPattern.test(email);
  //}
}

//const authForm = document.getElementById("authForm");
//const formTitle = document.getElementById("formTitle");
//const authButton = document.getElementById("authButton");
//const toggleLink = document.getElementById("toggleLink");
//const errorMessage = document.getElementById("errorMessage");
//const showPassword = document.getElementById("showPassword");
//const passwordField = document.getElementById("password");

//let isLogin = true;

//showPassword.addEventListener("change", function () {
//  passwordField.type = this.checked ? "text" : "password";
//});

//toggleLink.addEventListener("click", function () {
//  isLogin = !isLogin;
//  formTitle.textContent = isLogin ? "Login" : "Sign Up";
//  authButton.textContent = isLogin ? "Sign In" : "Create Profile";
//  toggleLink.textContent = isLogin ? "Sign Up" : "Login";
//  errorMessage.textContent = "";
//  document.getElementById("email").value = "";
//  document.getElementById("password").value = "";
//});

//authForm.addEventListener("submit", function (event) {
//  event.preventDefault();

//  const email = document.getElementById("email").value.trim();
//  const password = passwordField.value.trim();

//  const users = JSON.parse(localStorage.getItem("users")) || {};

//  // Check for empty fields
//  if (!email || !password) {
//    errorMessage.textContent = "Please fill in all fields.";
//    errorMessage.className = "message error";
//    return;
//  }

//  // Email validation regex
//  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

//  if (isLogin) {
//    if (users[email]) {
//      if (users[email].password === password) {
//        localStorage.setItem("currentUser", email);
//        window.location.href = "profile.html";
//      } else {
//        errorMessage.textContent = "Incorrect password.";
//        errorMessage.className = "message error";
//      }
//    } else {
//      errorMessage.textContent =
//        "This is your first time. You need to sign up.";
//      errorMessage.className = "message error";
//    }
//  } else {
//    if (users[email]) {
//      errorMessage.textContent =
//        "You are already registered. Please login.";
//      errorMessage.className = "message error";
//    } else {
//      users[email] = { password };
//      localStorage.setItem("users", JSON.stringify(users));
//      errorMessage.textContent = `Profile created for ${email}. Redirecting to profile...`;
//      errorMessage.className = "message success";
//      setTimeout(() => {
//        localStorage.setItem("currentUser", email);
//        window.location.href = "profile.html";
//      }, 2000);
//    }
//  }
//});
