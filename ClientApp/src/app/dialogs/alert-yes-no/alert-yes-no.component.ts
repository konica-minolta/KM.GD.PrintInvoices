import { Component, Inject } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
    selector: 'dialog-alert-yes-no',
    templateUrl: './alert-yes-no.component.html',
    styleUrls: ['./alert-yes-no.component.scss']
})
/** alert-yes-no component*/
export class AlertYesNoComponent {
    /** alert-yes-no ctor */
  constructor(public dialogRef: MatDialogRef<AlertYesNoComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {

  }
  close() {
    this.dialogRef.close(false);

  }
  confirm() {
    this.dialogRef.close(true);

  }
}
