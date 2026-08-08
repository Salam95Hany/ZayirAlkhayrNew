import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import qz from 'qz-tray';

@Injectable({
  providedIn: 'root'
})
export class QzPrintService {
  private readonly paperWidthMm = 80;
  private readonly preferredPrinterKeywords = ['xp-q810k', 'xprinter'];
  private qzSecurityInitialized = false;
  private qzConnectionPromise?: Promise<void>;

  constructor(private toaster: ToastrService) {
  }

  async Print(element: HTMLElement): Promise<void> {
    try {
      // this.setupQzSecurity();
      await this.InitQZ();
      const printer = await this.resolvePrinterName();
      await this.waitForContainerReady(element);
      const html = this.buildPrintableHtml(element);
      const heightMm = this.calculateHtmlHeightMm(element);
      const config = this.createHtmlReceiptConfig(printer, heightMm);
     await qz.print(config, [
    {
        type: 'pixel',
        format: 'html',
        flavor: 'plain',

        data: html,

        options: {
            pageWidth: 3.15,
            pageHeight: 12
        }
    }
]);

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

  private async waitForContainerReady(container: HTMLElement): Promise<void> {
    if ('fonts' in document)
      await (document as any).fonts.ready;

    await this.waitForImages(container);
    await this.waitForNextPaint();
  }

  private waitForImages(container: HTMLElement): Promise<void> {
    const pendingImages = Array.from(container.querySelectorAll('img'))
      .filter(image => !image.complete);

    if (!pendingImages.length) {
      return Promise.resolve();
    }

    return Promise.all(
      pendingImages.map(image => new Promise<void>(resolve => {
        const done = () => resolve();
        image.addEventListener('load', done, { once: true });
        image.addEventListener('error', done, { once: true });
      }))
    ).then(() => undefined);
  }

  private waitForNextPaint(): Promise<void> {
    return new Promise(resolve => {
      requestAnimationFrame(() => requestAnimationFrame(() => resolve()));
    });
  }

  private setupQzSecurity(): void {
    if (this.qzSecurityInitialized) {
      return;
    }

    qz.security.setCertificatePromise((resolve, reject) => {
      fetch('/assets/qz/digital-certificate.txt', { cache: 'no-store' })
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

  private buildPrintableHtml(element: HTMLElement): string {
    return `
<!DOCTYPE html>
<html lang="ar" dir="rtl">

<head>
    <meta charset="UTF-8">

    <style>

        @page {
            size: 80mm auto;
            margin: 0;
        }

       html,
body {
    width: 227px;
}

.receipt {
    width: 219px;

    margin-left: 8px;
    padding: 7px 5px 9px;
}


        /* =========================================
           Header
        ========================================= */

        .school-header {
            padding-bottom: 2mm;
            border-bottom: 0.35mm solid #111;
        }

        .school-brand {
            width: 100%;
            display: table;
            table-layout: fixed;
        }

        .school-logo {
            display: table-cell;
            width: 22mm;
            vertical-align: middle;
            text-align: center;
        }

        .school-title {
            display: table-cell;
            vertical-align: middle;
            text-align: center;
        }

        .stars {
            margin-bottom: 1mm;
            font-size: 7px;
            text-align: center;
        }

        .stars i {
            margin: 0 1px;
        }

        .logo-shield {
            position: relative;

            width: 15mm;
            height: 15mm;

            margin: auto;

            text-align: center;

            border: 0.6mm solid #111;
            border-radius: 2mm 2mm 6mm 6mm;
        }

        .logo-shield > i {
            line-height: 15mm;
            font-size: 18px;
        }

        .logo-shield .cap {
            position: absolute;

            bottom: -2.2mm;
            left: 50%;

            margin-left: -7px;

            background: #fff;

            font-size: 15px;
        }

        .school-title h1 {
            margin: 0;

            font-size: 17px;
            line-height: 1.25;

            font-weight: bold;

            white-space: nowrap;
        }

        .en-name {
            direction: ltr;

            margin-top: 0.6mm;

            font-size: 7.5px;
            font-weight: bold;

            text-align: center;
        }

        .school-title p {
            margin: 1.2mm 0 0;

            font-size: 9px;
            font-weight: bold;
        }


        /* =========================================
           Contact
        ========================================= */

        .contact-row {
            width: 100%;

            display: table;

            margin-top: 2mm;

            padding: 0 2mm;

            font-size: 7.5px;
            font-weight: bold;
        }

        .contact-item {
            display: table-cell;
            vertical-align: middle;

            width: 50%;

            white-space: nowrap;
        }

        .contact-item:first-child {
            text-align: right;
        }

        .contact-item:last-child {
            text-align: left;
        }

        .contact-item i {
            font-size: 8px;
        }


        /* =========================================
           Receipt title
        ========================================= */

        .receipt-title {
            margin: 2.5mm 0 2mm;

            text-align: center;

            font-size: 17px;
            font-weight: bold;
        }

        .receipt-title i {
            margin-left: 1.5mm;
            font-size: 13px;
        }


        /* =========================================
           Metadata
        ========================================= */

        .meta-grid {
            width: 100%;

            display: table;

            padding-bottom: 2.5mm;

            border-bottom: 0.25mm dashed #777;

            table-layout: fixed;
        }

        .meta-column {
            display: table-cell;

            width: 50%;

            vertical-align: top;

            padding: 0 1mm;
        }

        .meta-row {
            width: 100%;

            display: table;

            margin-bottom: 1.7mm;

            font-size: 8px;
            line-height: 1.4;
        }

        .meta-label {
            display: table-cell;

            width: 18mm;

            vertical-align: middle;

            white-space: nowrap;

            font-weight: bold;
        }

        .meta-value {
            display: table-cell;

            vertical-align: middle;

            text-align: center;

            font-size: 8px;
            font-weight: bold;

            white-space: nowrap;
        }

        .tag {
            display: inline-block;

            min-width: 15mm;

            padding: 1mm 1.5mm;

            border-radius: 1mm;

            background: #111;
            color: #fff;

            text-align: center;

            white-space: nowrap;

            font-size: 8px;
            line-height: 1;

            font-weight: bold;
        }

        .payment-method {
            display: table-cell;

            text-align: center;

            white-space: nowrap;

            font-weight: bold;
        }

        .payment-method i {
            font-size: 9px;
        }


        /* =========================================
           Student / Guardian
        ========================================= */

        .people-grid {
            width: 100%;

            display: table;

            margin-top: 2.5mm;
            margin-bottom: 2.5mm;

            table-layout: fixed;
        }

        .person-card {
            display: table-cell;

            width: 50%;

            vertical-align: top;

            padding: 0 2mm;
        }

        .student-card {
            border-left: 0.25mm dashed #777;
        }

        .person-card h2 {
            margin: 0 0 2mm;

            text-align: center;

            font-size: 11px;
            line-height: 1.3;

            font-weight: bold;

            white-space: nowrap;
        }

        .person-card h2 i {
            margin-left: 1mm;
            font-size: 12px;
        }

        .field-row {
            width: 100%;

            display: table;

            margin: 1.5mm 0;

            font-size: 7.8px;
            line-height: 1.4;
        }

        .field-row > span {
            display: table-cell;

            width: 18mm;

            white-space: nowrap;

            font-weight: bold;
        }

        .field-row strong {
            display: table-cell;

            text-align: center;

            font-weight: bold;

            white-space: nowrap;
        }


        /* =========================================
           Fees
        ========================================= */

       .fees-table {
    width: 100%;
    margin-top: 2mm;
    font-size: 8px;
}

.fees-table table {
    width: 100%;
    border-collapse: collapse;
    table-layout: fixed;
    direction: rtl;
}

.fees-table th,
.fees-table td {
    padding: 2mm 1mm;
    text-align: center;
    vertical-align: middle;

    font-weight: bold;
}

.fees-table th {
    background: #111;
    color: #fff;

    font-weight: bold;
}

.fees-table td {
    border-bottom: 0.2mm dashed #999;
}

.fees-table th:nth-child(1),
.fees-table td:nth-child(1) {
    width: 43%;
}

.fees-table th:nth-child(2),
.fees-table td:nth-child(2),
.fees-table th:nth-child(3),
.fees-table td:nth-child(3),
.fees-table th:nth-child(4),
.fees-table td:nth-child(4) {
    width: 19%;
}

.item-cell {
    text-align: right !important;
    white-space: nowrap;
}


        /* =========================================
           Totals
        ========================================= */

        .totals-box {
            width: 100%;

            display: table;

            margin-top: 2mm;

            border: 0.35mm solid #111;
            border-radius: 1.5mm;

            table-layout: fixed;
        }

        .total-card {
            position: relative;

            display: table-cell;

            width: 33.33%;

            height: 14mm;

            vertical-align: middle;

            text-align: center;

            padding: 1.5mm 1mm;
        }

        .total-card.center {
            border-right: 0.2mm dashed #777;
            border-left: 0.2mm dashed #777;
        }

        .total-icon {
            display: block;

            margin-bottom: 0.8mm;

            font-size: 10px;
        }

        .total-card > span {
            display: block;

            font-size: 8.5px;

            font-weight: bold;

            white-space: nowrap;
        }

        .total-card strong {
            display: block;

            margin-top: 0.8mm;

            font-size: 12px;
            line-height: 1;

            font-weight: bold;
        }


        /* =========================================
           Amount words
        ========================================= */

        .amount-words {
            width: 100%;

            display: table;

            padding: 2.5mm 1mm;

            border-bottom: 0.25mm dashed #777;

            font-size: 8px;
            line-height: 1.6;
        }

        .amount-words strong {
            display: table-cell;

            width: 30mm;

            white-space: nowrap;

            font-weight: bold;
        }

        .amount-words span {
            display: table-cell;

            font-weight: bold;
        }


        /* =========================================
           Footer
        ========================================= */

        .receipt-footer {
            width: 100%;

            display: table;

            padding: 3mm 1mm 1mm;

            font-size: 7.5px;
        }

        .footer-thanks {
            display: table-cell;

            width: 40%;

            vertical-align: top;

            text-align: center;

            font-size: 9px;
            font-weight: bold;
        }

        .footer-thanks i {
            margin-right: 1mm;

            font-size: 11px;
        }

        .footer-note {
            display: table-cell;

            width: 60%;

            vertical-align: top;

            line-height: 1.7;

            font-weight: bold;
        }

        .footer-note strong {
            font-weight: bold;
        }


        /* =========================================
           LTR
        ========================================= */

        [dir="ltr"],
        .en-name {
            direction: ltr;
            unicode-bidi: embed;
        }


    </style>
</head>

<body>

    ${element.outerHTML}

</body>
</html>
`;
  }


  private createHtmlReceiptConfig(printer: string, heightMm: number) {
   return qz.configs.create(printer, {

        units: 'mm',

        margins: 0,

        size: {
            width: 80,
            height: 305,
            custom: true
        },

        orientation: 'portrait',

        scaleContent: false,

        legacy: true,

        jobName: `SchoolReceipt-${Date.now()}`
    });
  }

  private calculateHtmlHeightMm(element: HTMLElement): number {
    const heightPx = Math.max(element.scrollHeight, element.getBoundingClientRect().height);
    const heightMm = (heightPx / 96) * 25.4;
    const bottomFeedMm = 8;
    return Math.ceil(heightMm + bottomFeedMm);
  }
}
