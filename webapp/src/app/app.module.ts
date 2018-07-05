import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import { AppRoutingModule } from './app.routing';
import { SearchComponent, UserInfoComponent, RepoItemComponent } from './components';
import { ContentListComponent } from './pages';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TokenInterceptor } from './auth/token.interceptor';
import { AuthorizeComponent } from './pages/authorize/authorize.component';
import { AuthService } from './auth/auth.service';
import { AuthCheckService } from './auth/auth-check.service';
import { UserService } from './services/user.service';

@NgModule({
  declarations: [
    AppComponent,
    SearchComponent,
    ContentListComponent,
    UserInfoComponent,
    RepoItemComponent,
    AuthorizeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    },
    AuthService,
    AuthCheckService,
    UserService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
