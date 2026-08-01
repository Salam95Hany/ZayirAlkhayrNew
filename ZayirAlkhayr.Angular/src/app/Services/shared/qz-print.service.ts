import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import html2canvas from 'html2canvas';
import qz from 'qz-tray';
import { ReceiptFeeAllocation, ReceiptFinancialSummary, ReceiptLookup, SchoolPaymentReceiptModel } from '../../Models/school/payment/SchoolPaymentReceiptModel';

@Injectable({ providedIn: 'root' })
export class QzPrintService {
  private readonly printableWidthMm = 72;
  private readonly printerDensityDpmm = 8;
  private readonly receiptWidthPx = this.printableWidthMm * this.printerDensityDpmm;
  private readonly renderScale = 4;
  private readonly printBottomPaddingPx = 48;
  private readonly extraFeedMm = 12;
  private readonly minimumPaperHeightMm = 40;
  private readonly preferredPrinterKeywords = ['xp-q810k', 'xprinter'];
  private qzConnectionPromise?: Promise<void>;

  constructor(private readonly toaster: ToastrService) { }

  async Print(receipt: SchoolPaymentReceiptModel): Promise<void> {
    let container: HTMLDivElement | undefined;

    try {
      await this.initQz();
      const printer = await this.resolvePrinterName();
      container = this.createPrintContainer(receipt);
      document.body.appendChild(container);
      await this.waitForContainerReady(container);

      const renderedCanvas = await this.renderReceipt(container);
      const printCanvas = this.prepareCanvasForPrint(renderedCanvas);
      const config = this.createReceiptConfig(
        printer,
        this.calculatePaperHeight(printCanvas.height),
        receipt.receipt.number
      );

      await qz.print(config, [this.buildReceiptImage(printCanvas)]);
    } catch (error) {
      const message = this.showPrintError(error);
      const printableError = error instanceof Error ? error : new Error(message);
      (printableError as Error & { userMessage?: string }).userMessage = message;
      throw printableError;
    } finally {
      container?.remove();
    }
  }

  BuildReceiptBody(receipt: SchoolPaymentReceiptModel): string {
    const { school, payment, student, guardian, cashier, summary, presentation } = receipt;
    const issuedAt = this.formatIssuedAt(receipt.receipt.issuedAt);
    const title = receipt.receipt.title || 'إيصال سداد';

    return `
      <div class="school-receipt" dir="rtl">
        <style>
          .school-receipt{width:${this.receiptWidthPx}px;background:#fff;color:#111;box-sizing:border-box;padding:14px 18px 8px;font-family:Tahoma,Arial,sans-serif;font-size:19px;line-height:1.35}
          .school-receipt *{box-sizing:border-box}
          .receipt-header{display:grid;grid-template-columns:1fr 1.75fr;gap:14px;align-items:center;text-align:center;padding-bottom:11px;border-bottom:3px solid #111}
          .school-logo{max-width:142px;max-height:126px;object-fit:contain;margin:auto;display:block}
          .school-name{font-size:27px;font-weight:900;line-height:1.2}
          .school-name-en{font-size:15px;font-weight:700;direction:ltr;letter-spacing:1.4px;margin-top:3px}
          .school-slogan{font-size:17px;margin-top:6px}
          .contacts{grid-column:1/-1;display:flex;flex-wrap:wrap;justify-content:space-between;gap:5px 10px;font-size:14px;margin-top:3px;direction:rtl}
          .contacts span{min-width:0;overflow-wrap:anywhere}
          .receipt-title{text-align:center;font-size:31px;font-weight:900;margin:10px 0 8px}
          .metadata{display:grid;grid-template-columns:1fr 1fr;gap:5px 24px;padding-bottom:9px;border-bottom:2px dashed #555}
          .meta-row,.detail-row{display:flex;justify-content:space-between;gap:7px;align-items:baseline;min-width:0}
          .meta-row .label,.detail-row .label{font-weight:700;white-space:nowrap}
          .meta-row .value,.detail-row .value{text-align:left;overflow-wrap:anywhere}
          .pill{display:inline-block;background:#111;color:#fff;border-radius:5px;padding:2px 10px;font-weight:800;line-height:1.25}
          .party-grid{display:grid;grid-template-columns:1fr 1fr;gap:0;margin:12px 0}
          .party-card{padding:0 13px 0 0;min-width:0}
          .party-card+ .party-card{border-right:2px dashed #777;padding-right:16px}
          .party-title{text-align:center;font-size:22px;font-weight:900;margin-bottom:7px}
          .detail-row{padding:3px 0;font-size:17px}
          .fees{width:100%;border-collapse:collapse;table-layout:fixed;text-align:center;font-size:16px}
          .fees th{background:#111;color:#fff;padding:7px 4px;font-weight:800}
          .fees th:first-child{border-radius:0 8px 8px 0}
          .fees th:last-child{border-radius:8px 0 0 8px}
          .fees td{padding:7px 4px;border-bottom:1.5px dashed #888;overflow-wrap:anywhere}
          .fees th:first-child,.fees td:first-child{width:40%;text-align:right;padding-right:9px}
          .fees th:not(:first-child),.fees td:not(:first-child){direction:ltr}
          .empty-row{text-align:center!important;padding:14px!important}
          .totals{display:grid;grid-template-columns:repeat(3,1fr);border:2px solid #111;border-radius:9px;margin-top:8px;overflow:hidden;text-align:center}
          .total{padding:7px 3px}
          .total+ .total{border-right:1.5px dashed #555}
          .total-label{font-size:17px;font-weight:800}
          .total-value{font-size:22px;font-weight:900;direction:ltr}
          .amount-words{display:flex;gap:8px;padding:9px 2px;border-bottom:2px dashed #555;font-size:16px}
          .amount-words strong{white-space:nowrap}
          .cashier{font-size:14px;padding-top:6px;text-align:left}
          .footer{display:grid;grid-template-columns:1fr .85fr 1fr;gap:8px;align-items:center;text-align:center;padding-top:10px}
          .footer-copy{font-size:15px;line-height:1.5}
          .qr,.stamp{max-width:106px;max-height:106px;object-fit:contain;margin:auto;display:block}
          .barcode{max-width:210px;max-height:68px;object-fit:contain;margin:8px auto 0;display:block}
          .caption{font-size:13px;font-weight:700;margin-top:2px}
          .note{font-size:13px;margin-top:5px}
        </style>

        <header class="receipt-header">
          <div>${this.buildImage(school.logoUrl, 'school-logo', school.nameAr)}</div>
          <div>
            <div class="school-name">${this.escapeHtml(school.nameAr)}</div>
            ${school.nameEn ? `<div class="school-name-en">${this.escapeHtml(school.nameEn)}</div>` : ''}
            ${school.slogan ? `<div class="school-slogan">${this.escapeHtml(school.slogan)}</div>` : ''}
          </div>
          ${this.buildContacts(school.contacts)}
        </header>

        <div class="receipt-title">${this.escapeHtml(title)}</div>
        <section class="metadata">
          ${this.buildMetaRow('رقم الإيصال', receipt.receipt.number)}
          ${this.buildLookupRow('نوع السداد', payment.type, true)}
          ${this.buildMetaRow('تاريخ الإيصال', issuedAt.date)}
          ${this.buildLookupRow('طريقة السداد', payment.method)}
          ${this.buildMetaRow('وقت الإيصال', issuedAt.time)}
          ${this.buildLookupRow('حالة السداد', payment.status, true)}
          ${receipt.receipt.externalReference ? this.buildMetaRow('المرجع', receipt.receipt.externalReference) : ''}
          ${payment.transactionReference ? this.buildMetaRow('مرجع الدفع', payment.transactionReference) : ''}
        </section>

        <section class="party-grid">
          ${this.buildStudentCard(student)}
          ${this.buildGuardianCard(guardian)}
        </section>

        ${this.buildAllocationsTable(receipt.allocations, summary)}
        ${this.buildTotals(summary)}
        ${summary.amountInWords ? `<div class="amount-words"><strong>المبلغ المدفوع كتابة:</strong><span>${this.escapeHtml(summary.amountInWords)}</span></div>` : ''}
        ${cashier ? `<div class="cashier">استلمه: ${this.escapeHtml(cashier.fullName)}${cashier.employeeNumber ? ` (${this.escapeHtml(cashier.employeeNumber)})` : ''}${cashier.branchName ? ` — ${this.escapeHtml(cashier.branchName)}` : ''}</div>` : ''}

        <footer class="footer">
          <div>
            ${school.stampUrl ? `<div class="caption">ختم المدرسة</div>${this.buildImage(school.stampUrl, 'stamp', 'ختم المدرسة')}` : ''}
          </div>
          <div>
            ${this.buildImage(presentation?.qrCode?.imageUrl, 'qr', 'QR code')}
            ${presentation?.qrCode?.caption ? `<div class="caption">${this.escapeHtml(presentation.qrCode.caption)}</div>` : ''}
          </div>
          <div class="footer-copy">
            <div>${this.escapeHtml(presentation?.thankYouMessage || 'شكراً لثقتكم بنا')}</div>
            ${presentation?.retentionNote ? `<div class="note">${this.escapeHtml(presentation.retentionNote)}</div>` : ''}
            ${(presentation?.notes || []).map(note => `<div class="note">${this.escapeHtml(note)}</div>`).join('')}
          </div>
        </footer>
        ${presentation?.barcode ? `${this.buildImage(presentation.barcode.imageUrl, 'barcode', presentation.barcode.value)}<div class="caption">${this.escapeHtml(presentation.barcode.caption || presentation.barcode.value)}</div>` : ''}
      </div>
    `;
  }

  handlePrintError(error: unknown): string {
    const message = `${(error as { message?: string })?.message || error || ''}`.toLowerCase();

    if (this.includesAny(message, ['websocket', 'connection', 'closed before', 'establish', 'qz tray', 'refused', 'failed to fetch'])) {
      return 'برنامج QZ Tray غير مشغل. يرجى تشغيله.';
    }
    if (this.includesAny(message, ['certificate', 'allow', 'blocked', 'signature', 'signing'])) {
      return 'مشكلة في صلاحيات الطباعة. يرجى الموافقة على Allow.';
    }
    if (this.includesAny(message, ['printer', 'no printers', 'not found'])) {
      return 'لم يتم العثور على الطابعة. تأكد من توصيلها.';
    }
    if (this.includesAny(message, ['timeout', 'timed out', 'not responding'])) {
      return 'الطابعة لا تستجيب. تأكد أنها مشغلة وبها ورق.';
    }
    return 'حدث خطأ أثناء الطباعة. حاول مرة أخرى.';
  }

  private buildContacts(contacts: SchoolPaymentReceiptModel['school']['contacts']): string {
    if (!contacts) return '';
    const values = [contacts.phone, contacts.address, contacts.website, contacts.email].filter(Boolean);
    return values.length
      ? `<div class="contacts">${values.map(value => `<span>${this.escapeHtml(value)}</span>`).join('')}</div>`
      : '';
  }

  private buildMetaRow(label: string, value: unknown, pill = false): string {
    return `<div class="meta-row"><span class="label">${this.escapeHtml(label)}:</span><span class="value${pill ? ' pill' : ''}">${this.displayValue(value)}</span></div>`;
  }

  private buildLookupRow(label: string, lookup: ReceiptLookup, pill = false): string {
    return this.buildMetaRow(label, lookup.label || lookup.code, pill);
  }

  private buildStudentCard(student: SchoolPaymentReceiptModel['student']): string {
    return `<div class="party-card">
      <div class="party-title">بيانات الطالب</div>
      ${this.buildDetailRow('اسم الطالب', student.fullName)}
      ${this.buildDetailRow('المرحلة', student.academicStage)}
      ${this.buildDetailRow('الصف', student.grade)}
      ${student.classroom ? this.buildDetailRow('الفصل', student.classroom) : ''}
      ${student.academicYear ? this.buildDetailRow('العام الدراسي', student.academicYear) : ''}
      ${this.buildDetailRow('رقم الطالب', student.studentNumber)}
    </div>`;
  }

  private buildGuardianCard(guardian?: SchoolPaymentReceiptModel['guardian']): string {
    if (!guardian) return '<div class="party-card"><div class="party-title">بيانات ولي الأمر</div><div class="detail-row">—</div></div>';
    return `<div class="party-card">
      <div class="party-title">بيانات ولي الأمر</div>
      ${this.buildDetailRow('الاسم', guardian.fullName)}
      ${guardian.relationship ? this.buildDetailRow('صلة القرابة', guardian.relationship) : ''}
      ${guardian.phone ? this.buildDetailRow('رقم الهاتف', guardian.phone) : ''}
    </div>`;
  }

  private buildDetailRow(label: string, value: unknown): string {
    return `<div class="detail-row"><span class="label">${this.escapeHtml(label)}:</span><span class="value">${this.displayValue(value)}</span></div>`;
  }

  private buildAllocationsTable(items: ReceiptFeeAllocation[], summary: ReceiptFinancialSummary): string {
    const rows = items.length
      ? items.map(item => `<tr>
          <td>${this.escapeHtml(item.fee.name)}${item.academicPeriod ? `<div class="note">${this.escapeHtml(item.academicPeriod)}</div>` : ''}</td>
          <td>${this.formatAmount(item.assessedAmount, summary)}</td>
          <td>${this.formatAmount(item.paidAmount, summary)}</td>
          <td>${this.formatAmount(item.remainingAmount, summary)}</td>
        </tr>`).join('')
      : '<tr><td class="empty-row" colspan="4">لا توجد بنود مالية</td></tr>';

    return `<table class="fees">
      <thead><tr><th>البند</th><th>المبلغ الكلي</th><th>المبلغ المدفوع</th><th>المتبقي</th></tr></thead>
      <tbody>${rows}</tbody>
    </table>`;
  }

  private buildTotals(summary: ReceiptFinancialSummary): string {
    const totals = [
      ['إجمالي المبلغ', summary.totalAssessed],
      ['إجمالي المدفوع', summary.totalPaid],
      ['المتبقي', summary.totalRemaining]
    ];
    return `<div class="totals">${totals.map(([label, value]) => `<div class="total"><div class="total-label">${label}</div><div class="total-value">${this.formatAmount(Number(value), summary)}</div></div>`).join('')}</div>`;
  }

  private buildImage(source: string | undefined, className: string, alt: string): string {
    return source ? `<img class="${className}" src="${this.escapeHtml(source)}" alt="${this.escapeHtml(alt)}" crossorigin="anonymous">` : '';
  }

  private formatIssuedAt(value: string | Date): { date: string; time: string } {
    const date = value instanceof Date ? value : new Date(value);
    if (Number.isNaN(date.getTime())) return { date: String(value), time: '—' };
    return {
      date: new Intl.DateTimeFormat('en-GB').format(date),
      time: new Intl.DateTimeFormat('en-US', { hour: 'numeric', minute: '2-digit', hour12: true }).format(date)
    };
  }

  private formatAmount(value: number, summary: ReceiptFinancialSummary): string {
    const digits = summary.currency.fractionDigits ?? 2;
    const amount = new Intl.NumberFormat('en-US', { minimumFractionDigits: digits, maximumFractionDigits: digits }).format(Number(value) || 0);
    return this.escapeHtml(summary.currency.symbol ? `${amount} ${summary.currency.symbol}` : amount);
  }

  private displayValue(value: unknown): string {
    const text = String(value ?? '').trim();
    return this.escapeHtml(text || '—');
  }

  private includesAny(value: string, candidates: string[]): boolean {
    return candidates.some(candidate => value.includes(candidate));
  }

  private initQz(): Promise<void> {
    if (qz.websocket.isActive()) return Promise.resolve();
    if (!this.qzConnectionPromise) {
      this.qzConnectionPromise = this.connectQz().catch(error => {
        this.qzConnectionPromise = undefined;
        throw error;
      });
    }
    return this.qzConnectionPromise;
  }

  private async connectQz(): Promise<void> {
    const secureModes = window.location.protocol === 'https:' ? [true] : [false, true];
    let lastError: unknown;
    for (const usingSecure of secureModes) {
      try {
        await qz.websocket.connect({ usingSecure, keepAlive: 60, retries: 1, delay: 0.5 });
        return;
      } catch (error) {
        lastError = error;
      }
    }
    throw lastError;
  }

  private async resolvePrinterName(): Promise<string> {
    const result = await qz.printers.find();
    const printers = Array.isArray(result) ? result.map(String) : [];
    if (!printers.length) throw new Error('No printers were found in QZ Tray.');

    const preferred = printers.find(printer =>
      this.preferredPrinterKeywords.some(keyword => printer.toLowerCase().includes(keyword))
    );
    if (preferred) return preferred;

    const defaultPrinter = await qz.printers.getDefault().catch(() => null);
    return defaultPrinter ? String(defaultPrinter) : printers[0];
  }

  private createPrintContainer(receipt: SchoolPaymentReceiptModel): HTMLDivElement {
    const container = document.createElement('div');
    Object.assign(container.style, {
      position: 'fixed', left: '-9999px', top: '0', background: '#fff', direction: 'rtl', zIndex: '-1'
    });
    container.setAttribute('dir', 'rtl');
    container.innerHTML = this.BuildReceiptBody(receipt);
    return container;
  }

  private async waitForContainerReady(container: HTMLElement): Promise<void> {
    const images = Array.from(container.querySelectorAll('img')).filter(image => !image.complete);
    await Promise.all(images.map(image => new Promise<void>(resolve => {
      image.addEventListener('load', () => resolve(), { once: true });
      image.addEventListener('error', () => resolve(), { once: true });
    })));
    await new Promise<void>(resolve => requestAnimationFrame(() => requestAnimationFrame(() => resolve())));
  }

  private renderReceipt(container: HTMLElement): Promise<HTMLCanvasElement> {
    return html2canvas(container, {
      scale: Math.max(this.renderScale, Math.ceil(window.devicePixelRatio || 1)),
      useCORS: true,
      backgroundColor: '#ffffff',
      height: container.scrollHeight,
      windowHeight: container.scrollHeight,
      imageTimeout: 0,
      logging: false
    });
  }

  private prepareCanvasForPrint(canvas: HTMLCanvasElement): HTMLCanvasElement {
    const height = Math.max(1, Math.ceil(canvas.height * this.receiptWidthPx / canvas.width));
    const output = document.createElement('canvas');
    output.width = this.receiptWidthPx;
    output.height = height + this.printBottomPaddingPx;
    const context = output.getContext('2d');
    if (!context) return canvas;

    context.fillStyle = '#fff';
    context.fillRect(0, 0, output.width, output.height);
    context.imageSmoothingEnabled = true;
    context.imageSmoothingQuality = 'high';
    context.drawImage(canvas, 0, 0, output.width, height);
    return output;
  }

  private createReceiptConfig(printer: string, height: number, receiptNumber: string) {
    return qz.configs.create(printer, {
      units: 'mm', margins: 0,
      size: { width: this.printableWidthMm, height, custom: true },
      density: { cross: this.printerDensityDpmm, feed: this.printerDensityDpmm },
      fallbackDensity: this.printerDensityDpmm,
      orientation: 'portrait', scaleContent: false, interpolation: 'bicubic',
      jobName: `School-Receipt-${receiptNumber}`
    });
  }

  private buildReceiptImage(canvas: HTMLCanvasElement) {
    return { type: 'pixel', format: 'image', flavor: 'base64', data: canvas.toDataURL('image/png').split(',')[1] };
  }

  private calculatePaperHeight(heightPx: number): number {
    const contentHeightMm = heightPx / this.printerDensityDpmm;
    return Math.max(this.minimumPaperHeightMm, Math.ceil(contentHeightMm) + this.extraFeedMm);
  }

  private showPrintError(error: unknown): string {
    const message = this.handlePrintError(error);
    this.toaster.error(message, 'خطأ في الطباعة');
    console.error('QZ print error', error);
    return message;
  }

  private escapeHtml(value: unknown): string {
    return String(value ?? '')
      .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
  }
}
