import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import { AppRoutingModule } from './app.routing';
import { SearchComponent, UserInfoComponent, RepoItemComponent } from './components';
import { ContentListComponent, AuthorizeComponent, SearchContentComponent } from './pages';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TokenInterceptor } from './auth/token.interceptor';
import { AuthService } from './auth/auth.service';
import { AuthCheckService } from './auth/auth-check.service';
import { UserService } from './services/user.service';
import { RepositoryService } from './services/repository.service';

@NgModule({
  declarations: [
    AppComponent,
    SearchComponent,
    ContentListComponent,
    UserInfoComponent,
    RepoItemComponent,
    AuthorizeComponent,
    SearchContentComponent
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
    UserService,
    RepositoryService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
