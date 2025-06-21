import { Injectable } from '@angular/core';
import { PDFHeaderSelectedModel, PDFModel } from '../../Models/shared/PDFHeaderSelected';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PdfDownloadService {
  PDFHeaderModel: PDFHeaderSelectedModel[] = [];
  apiURL = environment.apiUrl;
  constructor(private http: HttpClient) { }

  ConverHeaderToPDFModel(Arry: any[]) {
    Arry = [
      {
        "displayValue": "Code",
        "displayName": "الكود",
        "valueType": "Text"
      },
      {
        "displayValue": "FullName",
        "displayName": "الاسم",
        "valueType": "Text"
      },
      {
        "displayValue": "Description",
        "displayName": "الوصف",
        "valueType": "Text"
      },
      {
        "displayValue": "Phone",
        "displayName": "رقم التلفون",
        "valueType": "Text"
      },
      {
        "displayValue": "Phone2",
        "displayName": "رقم التلفون 2",
        "valueType": "Text"
      },
      {
        "displayValue": "Address",
        "displayName": "العنوان",
        "valueType": "Text"
      },
      {
        "displayValue": "Nationality",
        "displayName": "الجنسية",
        "valueType": "Text"
      },
      {
        "displayValue": "FaceBook",
        "displayName": "صفحة الفيس بوك",
        "valueType": "Text"
      }
    ]
    this.PDFHeaderModel = [];
    Arry.forEach((header, index) => {
      let obj: PDFHeaderSelectedModel = {} as PDFHeaderSelectedModel;
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

  DownloadFile(Model: PDFModel, fileName: string, downloadPath: string) {
    return this.http.post(this.apiURL + downloadPath, Model, {
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
