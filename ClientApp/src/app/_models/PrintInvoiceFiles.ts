import { InvoiceFile } from "./InvoiceFile";

export class PrintInvoiceFiles {
  fileName: string;
  numDocs: number;
  invoiceFiles: InvoiceFile[];
 
  parentFolder: string;

  note: string;
  printerToUse: string;
}
