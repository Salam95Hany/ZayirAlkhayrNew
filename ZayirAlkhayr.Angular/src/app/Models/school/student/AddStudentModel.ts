
export interface AddStudentModel {
    parentStudent: ParentStudent;
    student: StudentDetails[];
    discount: StudentDiscount[];
}

export interface ParentStudent {
    parentId: number;
    parentName: string;
    phone: string;
    address: string;
    childrenCount: number;
    insertUser: string;
    insertDate?: string | null;
    updateUser?: string;
    updateDate?: string | null;
}
export interface StudentDetails {
    id: number;
    studentId?: number;
    studentName: string; // اسم الطالب
    academicStageId: string; // المرحلة الدراسية
    academicStageName: string; // اسم المرحلة الدراسية
    birthDay: string; // تاريخ الميلاد
    governmentSchool: string; // المدرسة الحكومية 
    studyPeriod: number; // فترة الدراسة
    studyPeriodName: string; // فترة الدراسة
    nationalityId: string; // الجنسية
    nationalityName: string; // اسم الجنسية
    isHaveHealthCondition: boolean; // هل لديه حالة صحية
    healthConditionNote: string; // ملاحظات الحالة الصحية
    gender: number; // الجنس
    genderName: string; // الجنس
    academicYear: string; // السنة الدراسية
    studyAmount: number; // قيمة الدراسة
    studentStatusId: number; // موجود او منسحب
    studentStatusReason: string; // لو كان منسحب سبب الانسحاب
    orderAmongChildren: number; // ترتيب الطفل بين إخوانه
}

export interface StudentDiscount {
    studentName: string;
    discountTypeId: number;
    discountReason: string;
    discountAmount: number;
}