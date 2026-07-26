
export interface AddStudentModel {
    parentStudent: ParentStudent;
    student: StudentDetails[];
    // discount: StudentDiscount[];
}

export interface ParentStudent {
    parentId: number;
    parentName: string;
    fatherPhone: string;
    motherPhone: string;
    address: string;
    whatsappNumber: string;
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
    studyPeriodId: number; // فترة الدراسة
    studyPeriodName: string; // فترة الدراسة
    nationalityId: string; // الجنسية
    nationalityName: string; // اسم الجنسية
    studentTypeId: string; // نوع الطالب
    studentTypeName: string; // اسم نوع الطالب
    isHaveHealthCondition: boolean; // هل لديه حالة صحية
    healthConditionNote: string; // ملاحظات الحالة الصحية
    gender: number; // الجنس
    genderName: string; // الجنس
    academicYear: string; // السنة الدراسية
    academicYearId: string; // السنة الدراسية
    studentStatusId: number; // موجود او منسحب
    studentStatusReason: string; // لو كان منسحب سبب الانسحاب
    orderAmongChildren: number; // ترتيب الطفل بين إخوانه
    enrollmentDate: string;
    notes:string;
}

export interface StudentDiscount {
    studentName: string;
    academicYear: string;
    academicStageName: string;
    studyAmount: number;
    discountTypeId: number;
    discountReason: string;
    discountAmount: number;
    notes: string;
}