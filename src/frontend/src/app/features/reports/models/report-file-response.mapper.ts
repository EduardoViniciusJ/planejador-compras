import { HttpResponse } from '@angular/common/http';

import { ShoppingListReportFile } from './shopping-list-report-file.model';

export function mapReportFileResponse(
  response: HttpResponse<Blob>,
  fallbackFileName: string,
): ShoppingListReportFile {
  if (!response.body) {
    throw new Error('The report response did not contain a file.');
  }

  return {
    content: response.body,
    fileName:
      readFileName(response.headers.get('Content-Disposition')) ?? fallbackFileName,
  };
}

function readFileName(contentDisposition: string | null): string | null {
  if (!contentDisposition) {
    return null;
  }

  const encodedFileName = contentDisposition.match(
    /filename\*\s*=\s*(?:UTF-8'')?([^;]+)/i,
  )?.[1];
  const regularFileName =
    contentDisposition.match(/filename\s*=\s*"([^"]+)"/i)?.[1] ??
    contentDisposition.match(/filename\s*=\s*([^;]+)/i)?.[1];
  const candidate = encodedFileName
    ? decodeFileName(encodedFileName)
    : regularFileName?.trim();

  if (!candidate) {
    return null;
  }

  const safeFileName = candidate
    .replace(/^["']|["']$/g, '')
    .split(/[\\/]/)
    .at(-1)
    ?.replace(/[\u0000-\u001f\u007f]/g, '')
    .trim();

  return safeFileName || null;
}

function decodeFileName(value: string): string {
  const normalizedValue = value.trim().replace(/^["']|["']$/g, '');

  try {
    return decodeURIComponent(normalizedValue);
  } catch {
    return normalizedValue;
  }
}
