import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PosPrinterService {
  apiURL = environment.apiUrl + 'POSPrinters';
  constructor(private http: HttpClient) { }

  // ============================= Student ==============================

  GetStudentReceiptData(EnrollmentId: number, StudentPaymentId: number): Observable<ArrayBuffer> {
    return this.http.get(this.apiURL + '/GetStudentReceiptData?EnrollmentId=' + EnrollmentId + '&StudentPaymentId=' + StudentPaymentId, { responseType: 'arraybuffer' });
  }

  arrayBufferToBase64(buffer: ArrayBuffer): string {
    const bytes = new Uint8Array(buffer);
    const chunkSize = 0x8000;
    let binary = '';
    for (let offset = 0; offset < bytes.length; offset += chunkSize) {
      const chunk = bytes.subarray(offset, Math.min(offset + chunkSize, bytes.length));

      binary += String.fromCharCode(...chunk);
    }

    return btoa(binary);
  }

  downloadPdf(arrayBuffer: ArrayBuffer, fileName: string): void {
    const blob = new Blob([arrayBuffer], {
      type: 'application/pdf'
    });

    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }
}
