import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import qz from 'qz-tray';

@Injectable({
  providedIn: 'root'
})
export class QzPrintService {
  private readonly preferredPrinterKeywords = ['xp-k200l', 'xprinter'];
  private qzSecurityInitialized = false;
  private qzConnectionPromise?: Promise<void>;

  constructor(private toaster: ToastrService) {
  }

  async Print(base64Pdf: string, jobName = 'Student Receipt'): Promise<void> {
    try {
      // this.setupQzSecurity();
      await this.InitQZ();
      const printer = await this.resolvePrinterName();
      const config = qz.configs.create(printer,
        {
          units: 'mm',
          margins: 0,
          orientation: 'portrait',
          scaleContent: false,
          rasterize: false,
          colorType: 'blackwhite',
          copies: 1,
          jobName
        }
      );

      const data: qz.PrintData[] = [
        {
          type: 'pixel',
          format: 'pdf',
          flavor: 'base64',
          data: base64Pdf,
          options: {
            ignoreTransparency: true
          }
        }
      ];

      await qz.print(config, data);

    } catch (error) {
      const message = this.showPrintError(error);
      const printableError = error instanceof Error ? error : new Error(message);
      (printableError as Error & { userMessage?: string }).userMessage = message;
      throw printableError;
    }
  }

  handlePrintError(error: any): string {
    const msg = `${error?.message || error || ''}`.toLowerCase();

    if (
      msg.includes('websocket')
      || msg.includes('connection')
      || msg.includes('closed before')
      || msg.includes('establish')
      || msg.includes('qz tray')
      || msg.includes('refused')
      || msg.includes('failed to fetch')
    ) {
      return '❌ برنامج QZ Tray غير شغال. يرجى تشغيله';
    }

    if (
      msg.includes('certificate')
      || msg.includes('allow')
      || msg.includes('blocked')
      || msg.includes('signature')
      || msg.includes('signing')
    ) {
      return '❌ مشكلة في صلاحيات الطباعة. يرجى الموافقة على Allow';
    }

    if (
      msg.includes('printer')
      || msg.includes('no printers')
      || msg.includes('not found')
    ) {
      return '❌ لم يتم العثور على الطابعة. تأكد من توصيلها';
    }

    if (
      msg.includes('timeout')
      || msg.includes('timed out')
      || msg.includes('not responding')
    ) {
      return '❌ الطابعة لا تستجيب. تأكد أنها شغالة وبها ورق';
    }

    return '❌ حدث خطأ أثناء الطباعة. حاول مرة أخرى';
  }

  private InitQZ(): Promise<void> {
    if (qz.websocket.isActive()) {
      return Promise.resolve();
    }

    if (!this.qzConnectionPromise) {
      this.qzConnectionPromise = this.connectQz()
        .catch(error => {
          this.qzConnectionPromise = undefined;
          throw error;
        });
    }

    return this.qzConnectionPromise;
  }

  private async connectQz(): Promise<void> {
    const isHttpsPage = typeof window !== 'undefined' && window.location?.protocol === 'https:';
    const secureModes = isHttpsPage ? [true] : [false, true];
    let lastError: unknown;

    for (const usingSecure of secureModes) {
      try {
        await qz.websocket.connect({
          usingSecure,
          keepAlive: 60,
          retries: 1,
          delay: 0.5
        });
        return;
      } catch (error) {
        lastError = error;
      }
    }

    throw lastError;
  }

  private async resolvePrinterName(): Promise<string> {
    const printers = await qz.printers.find();
    const printerList = Array.isArray(printers) ? printers.map(printer => `${printer}`) : [];

    if (!printerList.length) {
      throw new Error('No printers were found in QZ Tray.');
    }

    const preferredPrinter = printerList.find(printer =>
      this.preferredPrinterKeywords.some(keyword => printer.toLowerCase().includes(keyword))
    );

    if (preferredPrinter) {
      return preferredPrinter;
    }

    const defaultPrinter = await qz.printers.getDefault().catch(() => null);
    if (defaultPrinter) {
      return `${defaultPrinter}`;
    }

    return printerList[0];
  }

  private showPrintError(error: any): string {
    const message = this.handlePrintError(error);
    this.toaster.error(message, 'خطأ في الطباعة');
    console.error('QZ print error', error);
    return message;
  }

  private setupQzSecurity(): void {
    if (this.qzSecurityInitialized) {
      return;
    }

    qz.security.setCertificatePromise((resolve, reject) => {
      fetch('/qz/digital-certificate.txt', { cache: 'no-store' })
        .then(res => {
          if (!res.ok) {
            throw new Error(`Failed to load certificate: ${res.status} ${res.statusText}`);
          }
          return res.text();
        })
        .then(resolve)
        .catch(reject);
    });

    qz.security.setSignatureAlgorithm('SHA512');

    qz.security.setSignaturePromise((toSign) => {
      return (resolve, reject) => {
        fetch('http://127.0.0.1:3000/sign', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ data: toSign })
        })
          .then(res => {
            if (!res.ok) {
              throw new Error(`Signing server error: ${res.status}`);
            }
            return res.text();
          })
          .then(resolve)
          .catch(reject);
      };
    });

    this.qzSecurityInitialized = true;
  }
}
