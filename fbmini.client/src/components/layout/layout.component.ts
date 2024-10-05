import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent {
  loggedUser : any;

 constructor(private _router: Router)
 {
  const localUser = localStorage.getItem('loggedUser');
  if(localUser != null)
    {
      this.loggedUser = JSON.parse(localUser);
    }
 }

 onLogOut()
 {
  localStorage.removeItem('loggedUser');
  this._router.navigateByUrl('/loginsignup');
 }
}
