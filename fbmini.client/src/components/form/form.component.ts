// app.component.ts
import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';


import { MatFormField, MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

interface DB {
  id: number;
  name: string;
}

@Component({
  selector: 'app-my-form',
  templateUrl: './form.component.html',
  styleUrls: ['./form.component.css'],
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatInputModule,
    MatFormField,
    MatButtonModule,
  ]
})
export class FormComponent {
  basicForm: FormGroup;
  public db: DB[] = [];
  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.basicForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  getDB() {
    this.http.get<DB[]>('api/db').subscribe(
      (result) => {
        this.db = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  ngOnInit() {
    this.getDB();
  }

  onSubmit() {
    if (this.basicForm.valid) {
      this.http.post<DB[]>('api/db', this.basicForm.value).subscribe(
        (result) => {
          this.getDB();
        },
        (error) => {
          console.error(error);
        }
      );
    }
    this.basicForm.reset();
  }
}
