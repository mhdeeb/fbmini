import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-backdrop',
  templateUrl: './backdrop.component.html',
  styleUrl: './backdrop.component.css',
  standalone: true
})
export class BackdropComponent {
  constructor(public dialog: MatDialog) {}

  openBackdrop(): void {
    const dialogRef = this.dialog.open(BackdropDialogComponent, {
      disableClose: true,
      panelClass: 'custom-dialog-container',
    });

    // Close the backdrop after some delay or action
    setTimeout(() => {
      dialogRef.close();
    }, 1000000); // You can remove this timeout based on your need
  }
}

// Backdrop Dialog Component
@Component({
  selector: 'backdrop-dialog',
  imports: [MatProgressSpinnerModule],
  standalone: true,
  template: `
    <div class="backdrop-dialog-content">
      <mat-spinner></mat-spinner>
    </div>
  `,
})
export class BackdropDialogComponent {}
