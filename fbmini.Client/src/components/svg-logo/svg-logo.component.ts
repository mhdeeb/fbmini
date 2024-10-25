import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-svg-logo',
  templateUrl: './svg-logo.component.html',
  styleUrls: ['./svg-logo.component.css'],
  standalone: true,
})
export class SvgLogoComponent {
  @Input() width: string = '24';
  @Input() height: string = '24';
  @Input() color: string = 'black';
}
