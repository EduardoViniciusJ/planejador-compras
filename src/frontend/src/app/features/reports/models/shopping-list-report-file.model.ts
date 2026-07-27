export type ShoppingListReportFormat = 'pdf' | 'excel';

export interface ShoppingListReportFile {
  readonly content: Blob;
  readonly fileName: string;
}
