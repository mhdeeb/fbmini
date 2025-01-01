import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Inject, Input } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogClose,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { BackdropDialogComponent } from '../backdrop/backdrop.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../utility/popup';

@Component({
  selector: 'app-post-edit',
  templateUrl: './post-edit.component.html',
  styleUrl: './post-edit.component.css',
  standalone: true,
  imports: [
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
    private readonly http: HttpClient,
    public dialog: MatDialog,
    private readonly snackbar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: any
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
      const dialogRef = this.dialog.open(BackdropDialogComponent, {
        disableClose: true,
      });
      for (const value in this.form.value)
        formData.append(value, this.form.get(value)?.value);
      this.http
        .post(`api/post/create/${this.data.postId ?? ''}`, formData)
        .subscribe({
          next: (res) => {
            this.dialogRef.close();
            dialogRef.close();
            location.reload();
          },
          error: (error) => {
            dialogRef.close();
            pop_up(this.snackbar, error.error.message, PopUp.ERROR);
          },
        });
    }
  }
}
