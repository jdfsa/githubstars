import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ContentListComponent, AuthorizeComponent } from './pages';
import { AuthCheckService } from './auth/auth-check.service';
import { SearchContentComponent } from './pages';

const ROUTES: Routes = [
  {
    path: '',
    redirectTo: '/search',
    pathMatch: 'full'
  },
  {
    path: 'search',
    component: SearchContentComponent,
    data: {
      search: ''
    },
    canActivate: [AuthCheckService]
  },
  {
    path: 'list',
    component: ContentListComponent,
    canActivate: [AuthCheckService]
  },
  {
    path: 'authorize',
    component: AuthorizeComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(ROUTES)
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
