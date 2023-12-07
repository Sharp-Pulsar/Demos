import { enableProdMode, importProvidersFrom } from '@angular/core';
import { RouterModule } from '@angular/router';

import { provideAnimations } from '@angular/platform-browser/animations';
// import { AppModule } from './app/app.module';
import { bootstrapApplication} from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { APP_ROUTES } from './app/app-routing';
//import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { environment } from './environments/environment';

//import { AppModule } from './app/app.module';
if (environment.production) {
  enableProdMode();
}


bootstrapApplication(AppComponent, {

  providers: [importProvidersFrom(HttpClientModule),
    provideAnimations(),
  //importProvidersFrom(BrowserAnimationsModule),
    importProvidersFrom(RouterModule.forRoot(APP_ROUTES))
  ]
});
//platformBrowserDynamic().bootstrapModule(AppModule)
  //.catch(err => console.error(err));
