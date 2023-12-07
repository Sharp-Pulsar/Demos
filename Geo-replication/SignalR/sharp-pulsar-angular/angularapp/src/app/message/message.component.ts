import { AfterViewInit, Component, ErrorHandler, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { SignalRService } from '../service/signalr.service';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageModel } from '../_models/message.model';
import { APP_BASE_HREF, CommonModule, HashLocationStrategy, LocationStrategy } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { SharpInterceptor } from '../providers/httpinterceptor';
import { SharpErrorHandler } from '../providers/errorhandler';
import { MatSelectModule } from '@angular/material/select';
import { ClientModel } from '../_models/client.model';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css'],
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
    NgbModule,
    MatSelectModule,
  ],
  providers: [SignalRService, { provide: HTTP_INTERCEPTORS, useClass: SharpInterceptor, multi: true }, { provide: ErrorHandler, useClass: SharpErrorHandler }, { provide: LocationStrategy, useClass: HashLocationStrategy },
    { provide: APP_BASE_HREF, useValue: '/' }],
})
export class MessageComponent implements OnInit, AfterViewInit {
  form = inject(FormBuilder).group({ send_to_username: undefined, message: undefined });
  @Output() onSignalR: EventEmitter<SignalRService> = new EventEmitter();
  signalR!: SignalRService;
  send_Username: string;
  constructor(private app: AppComponent, private route: ActivatedRoute) {
    
  }
    ngAfterViewInit(): void {
      //this.poll();
    }

  ngOnInit() {
    this.route.queryParams
      .subscribe(params => {
        console.log(params); // { orderby: "price" }
        this.send_Username = params['username'];
        console.log(this.send_Username); // price
      }
      );
    this.app.onSignalR.subscribe((signalR: SignalRService) => {
      ///console.log(`CLIENT: ${signalR.clients}`);
      try {
        this.signalR = signalR;
      }
      catch (error) { /* empty */ }
    });

  }
  openForm() {

    const open = document.getElementById("messageForm") as HTMLDivElement | null;
    console.log(open);
    open.style.display = "block";
  }
  closeForm() {
    const opent = document.getElementById("messageForm") as HTMLDivElement | null;
    opent.style.display = "none";
  }
  onMessageSubmit() {
    this.onSignalR.emit(this.signalR);
    const msg = this.form.value as MessageModel;
    msg.connect_id = this.signalR.user.connectionId;
    msg.username = this.signalR.user.username;
    msg.send_Username = this.send_Username;
    msg.timestamp = new Date().toString();
    this.signalR.hubConnection.invoke("Message", msg).then(() => this.form.controls.message.reset());
  }
  
  public Clients(): Array<ClientModel> {
    try {
      this.onSignalR.emit(this.signalR);
      return this.signalR.clients;
    }
    catch (error)
    {
      const m = new Array<ClientModel>();
      m.push(new ClientModel);
      return m;
    }
  }
  public Messages(): Array<MessageModel> { 
    try {
      const a = new Array<MessageModel>();
      this.signalR.messages.forEach((msg) => 
      {
        if (msg.username === this.send_Username)
        { /* empty */
          a.push(msg);
        }
        else if (msg.send_Username === this.send_Username) { /* empty */
          a.push(msg);
        }
      });

      return a;
    }
    catch (error) {
      const m = new Array<MessageModel>();
      m.push(new MessageModel());
      return m;
    }
  }
  updateEveryMS: 1000;  

  async poll()
  {
    this.onSignalR.emit(this.signalR);
    const t = true;
    while (t)
    {
      await this.sleep(this.updateEveryMS);
      // code to poll server and update models and view ...
      this.signalR.hubConnection.invoke("Messages", this.send_Username)
      
    }
  }
  sleep(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}

