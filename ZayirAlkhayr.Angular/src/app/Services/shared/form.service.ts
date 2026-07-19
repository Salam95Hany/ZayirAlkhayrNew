import { Injectable } from '@angular/core';
import { AbstractControl, FormArray, FormGroup, ValidatorFn, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class FormService {

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

  // get all values of the formGroup, loop over them
  // then mark each field as touched
  public markFormGroupTouched(formGroup: FormGroup) {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  public markControlsAsTouched(control: AbstractControl): void {

    if (control instanceof FormGroup) {
      Object.values(control.controls).forEach(c => this.markControlsAsTouched(c));
    }

    if (control instanceof FormArray) {
      control.controls.forEach(c => this.markControlsAsTouched(c));
    }

    control.markAsTouched();
    control.updateValueAndValidity();
  }

  // return list of error messages
  public validationMessages() {
    const messages = {
      required: 'هذا الحقل مطلوب',
      email: 'البريد الإلكتروني غير صالح',
      pattern: 'النمط المدخل غير صحيح',
      min: 'القيمة المدخلة أقل من الحد الأدنى المسموح',
      max: 'القيمة المدخلة أكبر من الحد الأقصى المسموح',
      invalid_URL: 'الرابط غير صالح',
      invalidAcademicYear:'يجب أن تكون سنة البداية أصغر من سنة النهاية',
      futureDate: 'لا يمكن أن يكون تاريخ الميلاد أكبر من تاريخ اليوم.',
      minAge: (error: any) => `يجب ألا يقل عمر الطالب عن ${error.requiredAge} سنوات.`,
      maxAge: (error: any) => `يجب ألا يزيد عمر الطالب عن ${error.requiredAge} سنوات.`,
      endDateLessThanStartDate: (error: string) => error || 'تاريخ النهاية يجب أن أكبر من تاريخ البداية',
      regexPattern: (error: string) => error || 'النمط المدخل غير صحيح',
      arrayLength: (error: string) => error || 'عدد العناصر غير صحيح',
      invalidExtension: (matches: any[]) => {
        let matchedCharacters = matches;
        matchedCharacters = matchedCharacters.reduce((characterString, character, index) => {
          let string = characterString;
          string += character;

          if (matchedCharacters.length !== index + 1) {
            string += ', ';
          }

          return string;
        }, '');

        return `امتداد الملف غير مسموح، المسموح: ${matchedCharacters}`;
      },
      invalid_characters: (matches: any[]) => {

        let matchedCharacters = matches;

        matchedCharacters = matchedCharacters.reduce((characterString, character, index) => {
          let string = characterString;
          string += character;

          if (matchedCharacters.length !== index + 1) {
            string += ', ';
          }

          return string;
        }, '');

        return `أحرف غير صالحة: ${matchedCharacters}`;
      },
    };

    return messages;
  }

  public validateForm(formToValidate: FormGroup, formErrors: any, checkDirty?: boolean) {
    const form = formToValidate;

    for (const field in formErrors) {
      if (field) {
        formErrors[field] = '';
        const control = form.get(field);

        const messages = this.validationMessages();
        if (control && !control.valid) {
          if (!checkDirty || (control.dirty || control.touched)) {
            for (const key in control.errors) {

              if (key && ![
                'invalid_characters',
                'invalidExtension',
                'endDateLessThanStartDate',
                'regexPattern',
                'dateGreaterThan',
                'dateLessThan',
                'arrayLength',
                'minAge',
                'maxAge'
              ].includes(key)) {
                formErrors[field] = formErrors[field] || messages[key];
              }
              else {
                formErrors[field] = formErrors[field] || messages[key](control.errors[key]);
              }
            }
          }
        }
      }
    }

    return formErrors;
  }

  public updateFieldsRequiredValidation(formGroup: FormGroup, field: string, isRequired: boolean) {
    const control: AbstractControl | null = formGroup.get(field);
    if (!control) return;

    if (isRequired) {
      control.addValidators(Validators.required);
    } else {
      control.removeValidators(Validators.required);
    }

    control.updateValueAndValidity();
  }

  public updateFieldValidators(formGroup: FormGroup, field: string, enable: boolean, validators: ValidatorFn[]): void {
    const control: AbstractControl | null = formGroup.get(field);
    if (!control) return;

    if (enable) {
      control.setValidators(validators);
    } else {
      control.clearValidators();
    }

    control.updateValueAndValidity();
  }
}
