import { HomeComponent } from './home/home.component';
import { PostComponent } from './post/post.component';
import { MessageComponent } from './message/message.component';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  { path: '', redirectTo: '/', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'post', component: PostComponent },
  { path: 'message', component: MessageComponent }
];
