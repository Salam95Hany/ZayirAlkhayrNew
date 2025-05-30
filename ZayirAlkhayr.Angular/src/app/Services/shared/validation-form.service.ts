import { Injectable } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { v4 as uuidv4 } from 'uuid';

@Injectable({
  providedIn: 'root'
})
export class ValidationFormService {
fileURL: any[] = [];

  constructor() { }

  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }

  buildFormData(formData, data, parentKey = null, key = null) {
    if (data instanceof File)
      formData.append('Files', data);
    else if (data && typeof data === 'object' && !(data instanceof File)) {
      Object.keys(data).forEach(key => {
        this.buildFormData(formData, data[key], parentKey ? `${parentKey}[${key}]` : key, key);
      });
    } else {
      const value = data == null ? '' : data;
      formData.append(parentKey, value);
    }
  }

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

  NumbersOnly(key: any): boolean {
    let patt = /^([0-9\+.])$/;
    let result = patt.test(key);
    return result;
  }

  TrimFormInputValue(ItemForm: FormGroup) {
    Object.keys(ItemForm.value).forEach(key => {
      if (typeof (ItemForm.value[key]) == 'string') {
        ItemForm.get(key).setValue(ItemForm.value[key]?.trim())
        ItemForm.get(key).setValue(ItemForm.value[key].replace(/\s+/g, ' '))
      }
    });

    return ItemForm;
  }

  noSpaceValidator(control: FormControl) {
    if (control.value) {
      let value = control.value;
      if (typeof value === 'number') {
        value = value.toString();
      }
  
      if (value.trim().length === 0) {
        return { noSpace: true };
      }
    }

    if (!control.value) {
      return null;
    }


    return null;
  }
}
