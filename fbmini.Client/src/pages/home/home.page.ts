import { Component } from '@angular/core';
import { FeedComponent } from '../../components/feed/feed.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FeedComponent],
  templateUrl: './home.page.html',
  styleUrl: './home.page.css',
})
export class HomePage {}
