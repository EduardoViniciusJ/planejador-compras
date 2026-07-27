import { TestBed } from '@angular/core/testing';

import { ReportFileDownloadService } from './report-file-download.service';

describe('ReportFileDownloadService', () => {
  it('should click a temporary link and always revoke the blob URL', () => {
    const createObjectUrl = vi.fn(() => 'blob:report-url');
    const revokeObjectUrl = vi.fn();
    const click = vi
      .spyOn(HTMLAnchorElement.prototype, 'click')
      .mockImplementation(() => undefined);
    Object.defineProperty(URL, 'createObjectURL', {
      configurable: true,
      value: createObjectUrl,
    });
    Object.defineProperty(URL, 'revokeObjectURL', {
      configurable: true,
      value: revokeObjectUrl,
    });
    TestBed.configureTestingModule({});
    const service = TestBed.inject(ReportFileDownloadService);
    const content = new Blob(['report']);

    service.download({ content, fileName: 'compras-julho.pdf' });

    expect(createObjectUrl).toHaveBeenCalledWith(content);
    expect(click).toHaveBeenCalledOnce();
    expect(revokeObjectUrl).toHaveBeenCalledWith('blob:report-url');
    expect(document.querySelector('a[download="compras-julho.pdf"]')).toBeNull();
  });
});
