import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { /*interval,*/ Subject } from 'rxjs';
import { HttpTransportType, HubConnection, LogLevel } from '@microsoft/signalr';
import { /*CONFIGURATION*/ } from '../shared/app.constants';
import { v4 as uuid } from 'uuid';
import { DisconnectedModel } from '../_models/disconnected.model';
import { ConnectedModel } from '../_models/connected.model';
import { LoadedModel } from '../_models/loaded.model';
import { HttpHeaders } from '@angular/common/http';
import { LoggedModel } from '../_models/logged.model';
import { PostModel } from '../_models/post.model';
import { MessageModel } from '../_models/message.model';
import { ClientModel } from '../_models/client.model';
const WAIT_UNTIL_ASPNETCORE_IS_READY_DELAY_IN_MS = 2000;
const commander = uuid();
@Injectable()
export class SignalRService {
  $connected = new Subject<ConnectedModel>();
  $disconnected = new Subject<DisconnectedModel>();
  $logged = new Subject<LoggedModel>();
  $post = new Subject<Array<PostModel>>();
  $message = new Subject<MessageModel>();
  $messages = new Subject<Array<MessageModel>>();
  $loaded = new Subject<LoadedModel>();
  connectionEstablished = new Subject<boolean>();
  headers: HttpHeaders;
  hubConnection: HubConnection | undefined;
  logs = new Array<string>();
  posts = new Array<PostModel>();
  messages = new Array<MessageModel>();
  public user: ClientModel;
  clients = new Array<ClientModel>();
  constructor() {
    this.user = new ClientModel();
    this.headers = new HttpHeaders();
    this.headers = this.headers.set('Content-Type', 'application/json');
    this.headers = this.headers.set('Accept', 'application/json');
    //this.commander = uuid();
  }
  public connect(username: string, password: string) {
    this.createConnection(username, password);
    this.registerOnServerEvents();
    this.startConnection();
    this.$loaded.subscribe((loaded) => this.loaded(loaded));
    this.$logged.subscribe((log) => {
      this.logged(log);
    });
    this.$post.subscribe((posted) => this.Posted(posted));
    this.$messages.subscribe((event) => {
      this.Msgs(event);
    });
    this.$message.subscribe((event) => {
      this.Messaged(event);
    });
    this.$connected.subscribe((event) => {
      this.Connected(event);
    });
    this.$disconnected.subscribe((event) => {
      this.disconnected(event);
    });
  }
  private createConnection(username: string, password: string)
  {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`http://192.168.0.131:7000/chathub?username=${username}&password=${password}`, HttpTransportType.WebSockets)
      .withAutomaticReconnect()      
      .configureLogging(LogLevel.Debug)
      .build();
  }
  private startConnection() {
    setTimeout(() =>
    {
      this.hubConnection.onclose(() => {
        this.connectionEstablished.next(false);
        this.hubConnection.stop();
      });
      this.hubConnection.start().then(() => {
        console.log('Hub connection started');
        this.connectionEstablished.next(true);
      }).catch((err) => {
        console.error(err.toString());
      });
    }, WAIT_UNTIL_ASPNETCORE_IS_READY_DELAY_IN_MS);
  }
  private registerOnServerEvents(): void
  {
    this.hubConnection.on("Connected", (connected: ConnectedModel) => {
      this.$connected.next(connected);
    });
    this.hubConnection.on("Disconnected", (disconnected: DisconnectedModel) => {
      this.$disconnected.next(disconnected);
      console.log(disconnected);
    });
    this.hubConnection.on("Logged", (pulsar: LoggedModel) => this.$logged.next(pulsar));
    this.hubConnection.on("Posted", (posted: Array<PostModel>) => this.$post.next(posted));
    this.hubConnection.on("Messaged", (messaged: MessageModel) => this.$message.next(messaged));
    this.hubConnection.on("Messages", (messaged: Array<MessageModel>) => this.$messages.next(messaged));
    this.hubConnection.on("Loaded", (loaded: LoadedModel) => this.$loaded.next(loaded));
  }
  private logged(log: LoggedModel) {
    this.logs = log.logs.reverse();
  }
  private Messaged(msg: MessageModel)
  {
    console.log(msg);
    this.messages.unshift(msg);
  }
  private Msgs(messages: Array<MessageModel>) {
    console.log(messages);
    messages.forEach((msg) => {
      this.messages.unshift(msg);
    });
    console.log(this.messages);
    //this.messages.reverse();
  }
  private Connected(connected: ConnectedModel) {
    this.user = connected.client;
    console.log(this.user);
    console.log("Connected");
  }
  private disconnected(disconnected: DisconnectedModel) {
    this.user = null;
    console.log(disconnected);
    console.log(this.user);
  }
  private loaded(loaded: LoadedModel) {
    this.clients = loaded.clients.reverse();
    console.log(this.clients);
  }
  private Posted(post: Array<PostModel>) {
    console.log(post);
    this.posts = post;
  }
  public GetCommander(): string {
    return commander;
  }
  public Client(clients: Array<ClientModel>): Array<ClientModel> {
    return clients;
  }
  
  public Posts(): Array<PostModel> {
    return this.posts;
  }
  public Messages(): Array<MessageModel> {
    return this.messages;
  }
}
