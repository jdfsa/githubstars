import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html'
})
export class SearchComponent implements OnInit {
  data: any;

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.userService.getUserData().subscribe(data => {
      this.data = data.data;
    });
  }

}
