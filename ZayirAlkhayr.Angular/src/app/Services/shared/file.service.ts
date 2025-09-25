import { Injectable } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { v4 as uuidv4 } from 'uuid';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  fileURL: any[] = [];

  getFileSize(file: any): number {
    const fileSizeInKB = file.size / 1024;
    const fileSizeInMB = Math.round(fileSizeInKB / 1024);
    return fileSizeInMB;
  }

  onSelectedFile(file: any): Promise<any[]> {
    if (file) {
      const promises = [];
      for (let x = 0; x < file.length; x++) {
        const reader = new FileReader();
        const URL = new Promise<any>((resolve, reject) => {
          reader.onload = (events: any) => {
            resolve({ image: events.target.result });
          };
          reader.readAsDataURL(file[x]);
        });
        const fileContent = new Promise<string>((resolve, reject) => {
          resolve(file);
        });
        promises.push(URL);
        promises.push(fileContent);
      }
      return Promise.all(promises);
    }
    return Promise.resolve([]);
  }

  onSelectedMultiFile(files: any): Promise<{ urls: any[]; fileContents: any[] }> {
    const urls: any[] = [];
    const fileContents: any[] = [];
    const promises = files.map((f: any, i) => {
      let uniqueId = uuidv4();
      const reader = new FileReader();
      return new Promise<void>((resolve, reject) => {
        reader.onload = (events: any) => {
          urls.push({ image: events.target.result, uniqueId: uniqueId });
          fileContents.push({ file: f, uniqueId: uniqueId });
          resolve();
        };
        reader.readAsDataURL(f);
      });
    });

    return Promise.all(promises).then(() => ({
      urls,
      fileContents,
    }));
  }
}
