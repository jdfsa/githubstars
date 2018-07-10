import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastContainerModule, ToastrModule } from 'ngx-toastr';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app.routing';
import { AuthCheckService, AuthService, TokenInterceptor } from './auth';
import { RepoItemComponent, SearchComponent, ToastComponent, UserInfoComponent } from './components';
import { AuthorizeComponent, ContentListComponent, SearchContentComponent } from './pages';
import { RepositoryService, UserService } from './services';

@NgModule({
  declarations: [
    AppComponent,
    SearchComponent,
    ContentListComponent,
    UserInfoComponent,
    RepoItemComponent,
    AuthorizeComponent,
    SearchContentComponent,
    ToastComponent
  ],
  imports: [
    FormsModule,
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ToastContainerModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-left',
      preventDuplicates: false,
      toastComponent: ToastComponent
    }),
    AppRoutingModule
  ],
  entryComponents: [
    ToastComponent
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
