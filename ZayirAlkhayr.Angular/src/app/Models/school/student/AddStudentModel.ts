
export interface AddFamilyStatusModel {
    parentStudent: ParentStudent;
    student: StudentDetails[];
    review: StudentReview;
}

export interface ParentStudent {
    parentName: string;
    phone: string;
    address: string;
    childrenCount: number;
    insertUser: string;
    insertDate: string | null;
    updateUser: string;
    updateDate: string | null;
}
export interface StudentDetails {
    id: number;
    studentName: string; // اسم الطالب
    academicStageId: number; // المرحلة الدراسية
    birthDay: string; // تاريخ الميلاد
    governmentSchool: string; // المدرسة الحكومية 
    studyPeriod: string; // فترة الدراسة
    nationalityId: number; // الجنسية
    isHaveHealthCondition: boolean; // هل لديه حالة صحية
    healthConditionNote: string; // ملاحظات الحالة الصحية
    gender: string; // الجنس
    academicYear: string; // السنة الدراسية
    studyAmount: number; // قيمة الدراسة
    studentStatusId: string; // موجود او منسحب
    studentStatusReason: string; // لو كان منسحب سبب الانسحاب
    orderAmongChildren: number; // ترتيب الطفل بين إخوانه
}

export interface StudentReview {
    studentName: string;
    discountTypeId: number;
    discountReason: string;
    discountAmount: number;
}