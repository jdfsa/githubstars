import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router, ActivatedRoute } from '@angular/router';

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
    private userService: UserService) { }

  ngOnInit() {
    this.userService.getUserData().subscribe(data => {
      this.data = data.data;
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
