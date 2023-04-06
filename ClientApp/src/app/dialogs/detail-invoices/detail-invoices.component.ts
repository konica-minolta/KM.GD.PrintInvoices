import { Component, Inject, ViewChild, VERSION, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
//import { faFilePdf } from '@fortawesome/free-solid-svg-icons';
import { FormControl, Validators } from '@angular/forms';
import { PrintInvoiceFiles } from '../../_models/PrintInvoiceFiles';
import { InvoiceFile } from '../../_models/InvoiceFile';
import { PrintInvoiceFilesList } from '../../_models/PrintInvoiceFilesList';


const materialModules = [
  MatButtonModule
];

@Component({
    selector: 'dialog-detail-invoices',
    templateUrl: './detail-invoices.component.html',
  styleUrls: ['./detail-invoices.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})
/** detail-invoices component*/
export class DetailInvoicesComponent {
  clicked = false;
    /** detail-invoices ctor */
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, public dialogRef: MatDialogRef<DetailInvoicesComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
    this.url = baseUrl;
  }


  ngOnInit() {
    this.http.get<Printer[]>(this.url + 'PrintFiles/printers').subscribe(result => {
      this.printers = result;
    }, error => console.error(error));
  }
 /* faFilePdf = faFilePdf;*/
  private url;
  printers: Printer[];
  displayedFileColumns: string[] = ['icon', 'fileName', 'position', 'shortPath', 'type', 'numCopy', 'toSign'];
  /*rebuildedData = this.buildMultiInvoice(this.data);*/
  dataSource = new MatTableDataSource<PrintInvoiceFiles[]>(this.data.printInvoiceFiles);
  
  columnsToDisplay = ['fileName', 'parentFolder'];
  columnsToDisplayWithExpand = [...this.columnsToDisplay, 'expand'];
  expandedElement: PrintInvoiceFiles | null;
  clickedRows = new Set<InvoiceFile>();
  pageSizeOptions: number[] = [5, 10, 25, 100];
  response: Response;
  txtNoteInvoice: string;
  selectedPrinter: string;
   printControl = new FormControl('', [Validators.required]);

  @ViewChild(MatPaginator) paginator: MatPaginator;
  ngAfterViewInit() {
   // this.dataSource = new MatTableDataSource<PrintInvoiceFiles[]>(this.data);
    this.dataSource.paginator = this.paginator;
  }

  close() {
    this.dialogRef.close();
  }

  print() {
    this.clicked = true;
    if (this.selectedPrinter === undefined) {
      this.clicked = false;
      return;
    }

    //var invoicesToSend = new PrintInvoiceFilesList();
    //var invoicesToSend = this.buildPrintInvoiceList(this.data);
    //invoicesToSend.note = this.txtNoteInvoice;
    //invoicesToSend.printerToUse = this.selectedPrinter;
 

    //invoicesToSend.invoiceFiles; //= this.dataSource.data.map(a => { return { ...a } });

    this.data.note = this.txtNoteInvoice;
    this.data.printerToUse = this.selectedPrinter;
    this.http.post<Response>(this.url + 'PrintFiles/printinvoice', this.data).subscribe(result => {
      this.response = result;
      this.dialogRef.close(result);
      this.clicked = false;
    }, error => {
      console.error(error);

      this.dialogRef.close(undefined);
      this.clicked = false;
    });
  }

  public clickedOnRow(row) {
  }

 // Rebuild detail invoiceFiles data files and put it in order
 public buildMultiInvoice(data){
   var result = new PrintInvoiceFiles();
   result.fileName = '';
   result.invoiceFiles = [];
   result.note = '';
   result.printerToUse = '';
   result.numDocs = 0;
  // data.sort((a, b) => (a.fileName > b.fileName) ? 1 : ((b.fileName > a.fileName) ? -1 :0) );
   data.forEach(row => {
     if (result.fileName =='') {
       result.fileName = row.fileName;
     } else {
       result.fileName += ', ' + row.fileName;
     }
     result.numDocs += row.numDocs;
     var resArr = result.invoiceFiles.concat(row.invoiceFiles);
     result.invoiceFiles = resArr;
    });
    return result;
  }

  public buildInvoiceList(data) {
    var result = [];
    data.forEach(row => {
      var singItem = new PrintInvoiceFiles();
      singItem.fileName = row.fileName;
      singItem.parentFolder = row.parentFolder;
      singItem.invoiceFiles = [...row.invoiceFiles];
    /*  singItem.invoiceFiles.push( row.invoiceFiles);*/
      singItem.numDocs = row.numDocs;
      result.push(singItem);
    });

    return result;
  }

  public buildPrintInvoiceList(data) {

    var result = new PrintInvoiceFilesList();
    result.note = '';
    result.printerToUse = '';
    result.printInvoiceFiles = new Array();
    data.forEach(row => {
      var singItem = new PrintInvoiceFiles();
      singItem.fileName = row.fileName;
      singItem.parentFolder = row.parentFolder;
      singItem.invoiceFiles = [...row.invoiceFiles];
      singItem.numDocs = row.numDocs;
      result.printInvoiceFiles.push(singItem);
    });

    return result;
  }
  
}

interface Printer {
  id: string;
  name: string; 
  description: string;
  useStandardWorksteps: boolean;
}
interface Response {
  code: string;
  message: string;
}
