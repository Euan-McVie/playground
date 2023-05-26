import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Tour of Heroes';
  links = [
    { display: "Dashboard", path: "/dashboard" },
    { display: "Heroes", path: "/heroes" }
  ];
}
