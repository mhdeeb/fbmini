import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
@Component({
  imports: [MatCardModule, MatIconModule, MatButtonModule],
  standalone: true,
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.css',
})
export class notFoundComponent {
  constructor() {}
}
