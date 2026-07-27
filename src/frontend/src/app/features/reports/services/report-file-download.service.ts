import { DOCUMENT } from '@angular/common';
import { Injectable, inject } from '@angular/core';

import { ShoppingListReportFile } from '../models/shopping-list-report-file.model';

@Injectable({ providedIn: 'root' })
export class ReportFileDownloadService {
  private readonly document = inject(DOCUMENT);

  download(file: ShoppingListReportFile): void {
    const temporaryUrl = URL.createObjectURL(file.content);
    const link = this.document.createElement('a');

    try {
      link.href = temporaryUrl;
      link.download = file.fileName;
      link.hidden = true;
      this.document.body.append(link);
      link.click();
    } finally {
      link.remove();
      URL.revokeObjectURL(temporaryUrl);
    }
  }
}
