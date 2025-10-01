import { AbstractControl, ValidatorFn, Validators } from "@angular/forms";

const validCharacters = /[^\s\w,.:&\/()+%'`@-]/;
const urlPattern = /^(ftp|http|https):\/\/[^ "]+$/;

export class CustomValidators extends Validators {

    static regexPattern(type: RegexType, message: string = null): ValidatorFn {
        const regex = regexList.find(x => x.type === type);
        if (!regex) {
            return (control: AbstractControl) => null;
        }

        return (control: AbstractControl) => {
            if (control.value && !regex.pattern.test(control.value)) {
                return { regexPattern: message ? message : regex.message };
            }
            return null;
        };
    }
}

export interface RegexModel {
    pattern: RegExp;
    message: string;
    type: RegexType;
}

export enum RegexType {
    text = 1,
    email,
    url,
    number,
    date,
    alpha,
    alphaAllowSpaces,
    alphaAllowSpacesAndSplash,
    alphaNumeric,
    alphaNumericAllowSpaces,
    alphaNumericAllowDash,
    numericAllowDash,
    numeric,
    currency,
    addressLine,
    noSpace,
    phoneNumber

}
export const regexList: RegexModel[] = [
    {
        pattern: /^[0-9]+(\.[0-9]+)?$/,
        message: "يُسمح فقط بالأرقام (مع فاصلة عشرية اختيارية)",
        type: RegexType.number
    },
    {
        pattern: /^[a-zA-Z]+$/,
        message: "يُسمح فقط بالحروف.",
        type: RegexType.alpha
    },
    {
        pattern: /^[a-zA-Z\s]+$/,
        message: "يُسمح فقط بالحروف والمسافات.",
        type: RegexType.alphaAllowSpaces
    },
    {
        pattern: /^[a-zA-Z\s/]+$/,
        message: "يُسمح فقط بالحروف، المسافات والشرطة المائلة /.",
        type: RegexType.alphaAllowSpacesAndSplash
    },
    {
        pattern: /^[a-zA-Z0-9]+$/,
        message: "يُسمح فقط بالحروف والأرقام.",
        type: RegexType.alphaNumeric
    },
    {
        pattern: /^[a-zA-Z0-9\s]+$/,
        message: "يُسمح فقط بالحروف والأرقام والمسافات.",
        type: RegexType.alphaNumericAllowSpaces
    },
    {
        pattern: /^[a-zA-Z0-9-]+$/,
        message: "يُسمح فقط بالحروف والأرقام والشرطة -.",
        type: RegexType.alphaNumericAllowDash
    },
    {
        pattern: /^\d+$/,
        message: "يُسمح فقط بالأرقام.",
        type: RegexType.numeric
    },
    {
        pattern: /^[0-9-]+$/,
        message: "يُسمح فقط بالأرقام والشرطة -.",
        type: RegexType.numericAllowDash
    },
    {
        pattern: /^\d+(\.\d{1,2})?$/,
        message: "عملة صحيحة (حتى منزلتين عشريتين).",
        type: RegexType.currency
    },
    {
        pattern: /^[\w\s,-]+$/,
        message: "يُسمح فقط بالحروف، الأرقام، المسافات، الفواصل والشرطة.",
        type: RegexType.addressLine
    },
    {
        pattern: /^\d{4}-\d{2}-\d{2}$/,
        message: "صيغة التاريخ يجب أن تكون YYYY-MM-DD.",
        type: RegexType.date
    },
    {
        pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
        message: "البريد الإلكتروني غير صالح.",
        type: RegexType.email
    },
    {
        pattern: /^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$/,
        message: "الرابط غير صالح.",
        type: RegexType.url
    },
    {
        pattern: /^[a-zA-Z\s]+$/,
        message: "يُسمح فقط بالنصوص (حروف ومسافات).",
        type: RegexType.text
    },
    {
        pattern: /^(?!\s*$).+/,
        message: "لا يمكن ان يحتوي الحقل على مسافات فقط.",
        type: RegexType.noSpace
    },
    {
        pattern: /^(?:01[0125]\d{8}|00[1-9]\d{5,13})$/,
        message: "رقم التلفون غير صحيح.",
        type: RegexType.phoneNumber
    }
];