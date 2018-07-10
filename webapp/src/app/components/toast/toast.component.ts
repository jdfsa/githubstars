import {
  animate,
  keyframes,
  state,
  style,
  transition,
  trigger
} from '@angular/animations';
import { Component, ViewChild, OnInit } from '@angular/core';
import { ToastContainerDirective, ToastrService, ToastPackage, Toast } from 'ngx-toastr';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  animations: [
    trigger('flyInOut', [
      state('inactive', style({
        display: 'none',
        opacity: 0
      })),
      transition('inactive => active', animate('400ms ease-out', keyframes([
        style({
          opacity: 0,
        }),
        style({
          opacity: 1,
        })
      ]))),
      transition('active => removed', animate('400ms ease-out', keyframes([
        style({
          opacity: 1,
        }),
        style({
          transform: 'translate3d(10%, 0, 0) skewX(10deg)',
          opacity: 0,
        }),
      ]))),
    ]),
  ],
  preserveWhitespaces: false,
})
export class ToastComponent extends Toast {
  @ViewChild(ToastContainerDirective) toastContainer: ToastContainerDirective;

  constructor(
    toastrService: ToastrService,
    toastPackage: ToastPackage) {
      super(toastrService, toastPackage);
    }

  onClick(event: Event) {
    event.stopPropagation();
    this.toastPackage.triggerAction();
    return false;
  }

}
