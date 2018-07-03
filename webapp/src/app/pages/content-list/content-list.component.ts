import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-content-list',
  templateUrl: './content-list.component.html'
})
export class ContentListComponent implements OnInit {
  hasResults: boolean;

  constructor() { }

  ngOnInit() {
    this.hasResults = true;
  }

}
