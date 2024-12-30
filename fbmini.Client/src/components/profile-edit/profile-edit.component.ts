import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { pop_up, PopUp } from '../../utility/popup';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { User, UserView } from '../../utility/types';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
} from '@angular/forms';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
  MatDialog,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { BackdropDialogComponent } from '../backdrop/backdrop.component';

@Component({
  selector: 'app-profile-edit',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatCardModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatDividerModule,
    CommonModule,
  ],
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.css'],
})
export class ProfileEditDialog {
  public form: FormGroup;
  public previews: {
    picture: string | ArrayBuffer | null;
    cover: string | ArrayBuffer | null;
  } = {
      picture: null,
      cover: null
    };

  constructor(
    private readonly http: HttpClient,
    private readonly fb: FormBuilder,
    private readonly dialogRef: MatDialogRef<ProfileEditDialog>,
    private readonly snackbar: MatSnackBar,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: User
  ) {
    this.form = this.fb.group(new UserView());
    this.form.patchValue({
      email: data.email,
      bio: data.bio,
      phoneNumber: data.phoneNumber,
    });

    this.previews.picture = data.pictureUrl ?? null;
    this.previews.cover = data.coverUrl ?? null;

    this.form.markAsPristine();
  }

  onFileSelected(event: any, id: string): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        if (id === 'picture') {
          this.previews.picture = reader.result;
          this.form.patchValue({ picture: file });
          this.form.controls['picture'].markAsDirty();
        } else if (id === 'cover') {
          this.previews.cover = reader.result;
          this.form.patchValue({ cover: file });
          this.form.controls['cover'].markAsDirty();
        }
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit() {
    if (this.form.valid) {
      const dialogRef = this.dialog.open(BackdropDialogComponent, {
        disableClose: true,
      });

      let formData = new FormData();

      for (const value in this.form.value)
        if (this.form.get(value)?.dirty)
          formData.append(value, this.form.get(value)?.value);

      this.http.patch(`api/user/profile/${this.data.userName}`, formData).subscribe({
        next: (result: any) => {
          dialogRef.close();
          this.dialogRef.close(true);
          pop_up(this.snackbar, result.message, PopUp.SUCCESS);
        },
        error: (error) => {
          dialogRef.close();
          pop_up(this.snackbar, error.error.message, PopUp.ERROR);
        },
      });
    }
  }

  onClose() {
    this.dialogRef.close(false);
  }
}
