
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(/*http: HttpClient*/) {
  }
  ngOnInit() {
    /*const data = history.state;
    console.log(`DATA '${data.username} ${data.password}`);
    this.username = data.username;
    this.password = data.password;
    if (data.username != null)
      this.user = true;*/
    //this.signalRService.connect(this.username, this.password);
  }
  title = 'home';
}
