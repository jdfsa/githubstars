import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search-content',
  templateUrl: './search-content.component.html'
})
export class SearchContentComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

}
