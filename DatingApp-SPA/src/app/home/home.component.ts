import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  constructor(private http: HttpClient) {}

  registerMode = false;


  ngOnInit() {
  }

  registerToggle() {
    this.registerMode = true;
  }


  // event passing parametar from child component "register"
  cancelRegistarMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
