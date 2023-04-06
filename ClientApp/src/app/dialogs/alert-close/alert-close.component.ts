import { Component, Inject } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
    selector: 'dialog-alert-close',
    templateUrl: './alert-close.component.html',
    styleUrls: ['./alert-close.component.scss']
})
/** alert-close component*/
export class AlertCloseComponent {
    /** alert-close ctor */
  constructor(public dialogRef: MatDialogRef<AlertCloseComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {

  }
  close() {
    this.dialogRef.close("Close");
  }
}
