//https://stackoverflow.com/questions/62576780/angular-not-subscribing-to-signalr-message
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, ErrorHandler, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { APP_BASE_HREF, CommonModule, HashLocationStrategy, LocationStrategy } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
//import { ToastrModule } from 'ngx-toastr';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharpInterceptor } from './providers/httpinterceptor';
import { SharpErrorHandler } from './providers/errorhandler';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClientModel } from './_models/client.model';
import { SignalRService } from './service/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    RouterModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatCardModule,
    MatDividerModule,
    NgbModule
  ],
  providers: [SignalRService, { provide: HTTP_INTERCEPTORS, useClass: SharpInterceptor, multi: true }, { provide: ErrorHandler, useClass: SharpErrorHandler }, { provide: LocationStrategy, useClass: HashLocationStrategy },
  { provide: APP_BASE_HREF, useValue: '/' }],
})
export class AppComponent implements OnInit {
  @Output() onUser: EventEmitter<ClientModel> = new EventEmitter();
  @Output() onSignalR: EventEmitter<SignalRService> = new EventEmitter();
  form = inject(FormBuilder).group({ connectionId: undefined, username: undefined, password: undefined });
  public username: boolean = false;
  public forecasts?: WeatherForecast[];
  constructor(http: HttpClient, private signalRService: SignalRService, public router: Router) {
    //this.username = '';
    //http.get<WeatherForecast[]>('/weatherforecast').subscribe(result => {
    //  this.forecasts = result;
    //}, error => console.error(error));
    //SignalRService.$loaded.subscribe((loaded) => this.loaded(loaded));
    
  }
  ngOnInit() {
  }
  async connect() {
    const client = this.form.value as ClientModel;
    if ((client.username != null && client.username != '')
      && (client.password != null && client.password != '')) {
      this.signalRService.connect(client.username, client.password);
      this.form.controls.username.reset();
      this.form.controls.password.reset();
      await delay(2000);
      console.log('client');
      this.username = true;
    }
    else {
      console.log('null client');
    }

  }
  public Clients(): Array<ClientModel> {
    this.onUser.emit(this.signalRService.user);
    this.onSignalR.emit(this.signalRService);
    return this.signalRService.clients;
  }
  
  public Logs(): Array<string> {
    try {
      this.onSignalR.emit(this.signalRService);
      return this.signalRService.logs;
    }
    catch (error) {
      const m = new Array<string>();
      m.push("");
      return m;
    }
  }
  title = 'angularapp';
  /*connectll() {
    const client = this.form.value as ClientModel;
    if ((client.username != null && client.username != '')
      && (client.password != null && client.password != '')) {
      this.username = true;
      this.router.navigateByUrl('/home', { state: { username: client.username, password: client.password } });
      console.log(this.username);
      this.form.controls.username.reset();
      this.form.controls.password.reset();
       
    }
    else {
      console.log('null client');
    }
  } */ 
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

function delay(ms: number) {
  return new Promise(resolve => setTimeout(resolve, ms));
}
