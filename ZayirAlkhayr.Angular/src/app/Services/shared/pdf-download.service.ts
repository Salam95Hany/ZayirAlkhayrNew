import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { PDFHeaderSelected, SearchReportModel } from '../../Models/shared/SearchReportModel';

@Injectable({
  providedIn: 'root'
})
export class PdfDownloadService {
  PDFHeaderModel: PDFHeaderSelected[] = [];
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  ConverHeaderToPDFModel(Arry: any[]) {
    this.PDFHeaderModel = [];
    Arry.forEach((header, index) => {
      let obj: PDFHeaderSelected = {} as PDFHeaderSelected;
      obj.nameEn = header.displayValue;
      obj.nameAr = header.displayName;
      obj.isSelected = false;
      obj.displayOrder = index + 1;
      obj.valueType = header.valueType;
      obj.isAllowSummation = false;
      this.PDFHeaderModel.push(obj);
    });

    return this.PDFHeaderModel;
  }

  DownloadFile(Model: SearchReportModel, fileName: string) {
    return this.http.post(this.apiURL + 'CreateReport/CreateGeneralReport', Model, {
      responseType: 'blob',
      observe: 'response'
    }).pipe(
      map((response: any) => {
        const downloadLink = document.createElement('a');
        downloadLink.href = URL.createObjectURL(new Blob([response.body], { type: response.body.type }));
        downloadLink.download = fileName;
        downloadLink.click();
      })
    );
  }
}
