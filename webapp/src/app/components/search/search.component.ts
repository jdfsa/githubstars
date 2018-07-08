import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html'
})
export class SearchComponent implements OnInit {
  data: any;
  search: string;

  constructor(
    private router: Router,
    private active: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService) { }

  ngOnInit() {
    this.userService.getUserData().subscribe(data => {
      this.data = data;

      // stores user id
      this.authService.storeUserId(this.data.id);
    });

    this.active.queryParams.subscribe(params => {
      if (params['search']) {
        this.search = params['search'];
      }
    });
  }

  public doSearch() {
    this.router.navigateByUrl('/list?search=' + this.search);
  }

}
