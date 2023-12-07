import { APP_BASE_HREF, CommonModule, HashLocationStrategy, LocationStrategy } from '@angular/common';
import { Component, ErrorHandler, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SignalRService } from '../service/signalr.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { SharpInterceptor } from '../providers/httpinterceptor';
import { SharpErrorHandler } from '../providers/errorhandler';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatCardModule,
    MatDividerModule,
    ReactiveFormsModule,
    NgbModule
  ],
  providers: [SignalRService, { provide: HTTP_INTERCEPTORS, useClass: SharpInterceptor, multi: true }, { provide: ErrorHandler, useClass: SharpErrorHandler }, { provide: LocationStrategy, useClass: HashLocationStrategy },
    { provide: APP_BASE_HREF, useValue: '/' }],
})
export class LogComponent implements OnInit {
  signalR!: SignalRService;
  constructor(private app: AppComponent) {
  }
  ngOnInit() {
    
  }
  public Logs(): Array<string> {
    try {
      return this.signalR.logs;
    }
    catch (error) {
      const m = new Array<string>();
      m.push("");
      return m;
    }
  }
}
