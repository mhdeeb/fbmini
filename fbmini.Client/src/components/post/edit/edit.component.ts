import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  templateUrl: './edit.component.html',
  selector: 'app-post-edit',
  styleUrl: './edit.component.css',
  standalone: true,
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatIconModule,
    CommonModule,
    MatInputModule,
    ReactiveFormsModule,
  ],
})
export class PostEditDialog {
  form: FormGroup;
  imagePreview: string | ArrayBuffer | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly dialogRef: MatDialogRef<PostEditDialog>,
    private readonly http: HttpClient
  ) {
    this.form = this.fb.group({
      Title: ['', Validators.required],
      Content: [''],
      Attachment: [null],
    });
  }

  onPictureSelect(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.form.patchValue({ Attachment: file });

      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result;
      };
      reader.readAsDataURL(file);
    } else {
      this.form.patchValue({ Attachment: null });
      this.imagePreview = null;
    }
  }

  post() {
    if (this.form.valid) {
      let formData = new FormData();

      for (const value in this.form.value)
        formData.append(value, this.form.get(value)?.value);

      this.http.post('api/user/post', formData).subscribe({
        next: (res) => {
          this.dialogRef.close();
          location.reload();
        },
        error: (error) => {
          console.error(error);
        },
      });
    }
  }
}
