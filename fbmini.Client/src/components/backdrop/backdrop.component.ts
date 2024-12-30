import { Component } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'backdrop-dialog',
  imports: [MatProgressSpinnerModule],
  standalone: true,
  template: `
    <div class="custom flex items-center justify-center">
      <mat-spinner></mat-spinner>
    </div>
  `,
})
export class BackdropDialogComponent {}
