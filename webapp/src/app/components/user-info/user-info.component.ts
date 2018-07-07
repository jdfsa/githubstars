import { Component, OnInit, Input, OnChanges } from '@angular/core';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html'
})
export class UserInfoComponent {
  @Input() data: any;
}
