import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import { AppRoutingModule } from './app.routing';
import { SearchComponent, UserInfoComponent, RepoItemComponent } from './components';
import { ContentListComponent } from './pages';

@NgModule({
  declarations: [
    AppComponent,
    SearchComponent,
    ContentListComponent,
    UserInfoComponent,
    RepoItemComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
