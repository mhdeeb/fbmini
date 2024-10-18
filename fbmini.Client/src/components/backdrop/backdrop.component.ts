import { Component } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

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
