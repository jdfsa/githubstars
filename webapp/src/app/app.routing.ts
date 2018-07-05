import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ContentListComponent, AuthorizeComponent } from './pages';
import { AuthCheckService } from './auth/auth-check.service';

const ROUTES: Routes = [
  {
    path: '',
    redirectTo: '/list',
    pathMatch: 'full'
  },
  {
    path: 'list',
    component: ContentListComponent,
    data: {
      search: ''
    },
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
