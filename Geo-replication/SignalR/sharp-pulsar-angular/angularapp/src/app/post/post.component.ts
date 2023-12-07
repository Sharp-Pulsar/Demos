import { Component, ErrorHandler, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { PostModel } from '../_models/post.model';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SignalRService } from '../service/signalr.service';
import { APP_BASE_HREF, CommonModule, HashLocationStrategy, LocationStrategy } from "@angular/common";
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatDividerModule } from '@angular/material/divider';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { SharpInterceptor } from '../providers/httpinterceptor';
import { SharpErrorHandler } from '../providers/errorhandler';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { ClientModel } from '../_models/client.model';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css'],
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
    MatChipsModule,
    MatSelectModule,
    MatListModule,
  ],
  providers: [SignalRService, { provide: HTTP_INTERCEPTORS, useClass: SharpInterceptor, multi: true }, { provide: ErrorHandler, useClass: SharpErrorHandler }, { provide: LocationStrategy, useClass: HashLocationStrategy },
    { provide: APP_BASE_HREF, useValue: '/' }],
})
export class PostComponent implements OnInit {
  form = inject(FormBuilder).group({ message: undefined });
  signalR!: SignalRService;
  user: ClientModel;
  constructor(private app: AppComponent) {
  }
  ngOnInit(): void {
    /** Invoked when is received new data*/
    this.app.onUser.subscribe((client: ClientModel) => {
      //console.log(`CLIENT: ${client.username}`);
      this.user = client;
    });
    this.app.onSignalR.subscribe((signalR: SignalRService) => {
      ///console.log(`CLIENT: ${signalR.clients}`);
      try {
        this.signalR = signalR;
      }
      catch (error) { /* empty */ }
    });
  }
  onPostSubmit() {
    const post = this.form.value as PostModel;
    post.connect_id = this.user.connectionId;
    post.username = this.user.username;
    post.timestamp = new Date().toString();
    this.signalR.hubConnection.invoke("Post", post).then(() => this.form.controls.message.reset());
  }
  openForm() {
    
    const open = document.getElementById("postForm") as HTMLDivElement | null;
    console.log(open);
    open.style.display = "block";
  }
  closeForm() {
    const opent = document.getElementById("postForm") as HTMLDivElement | null;
    opent.style.display = "none";
  }
  public Posts(): Array<PostModel> {
    try {
      return this.signalR.posts;
    }
    catch (error) {
      const m = new Array<PostModel>();
      m.push(new PostModel());
      return m;
    }
  }
}
