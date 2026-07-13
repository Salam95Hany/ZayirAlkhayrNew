import { ParentStudent, StudentDetails, StudentDiscount } from "./AddStudentModel";
import { StudentLookups } from "./StudentLookups";

export interface UpdateStudentLookups {
    lookups: StudentLookups;
    parentStudent: ParentStudent;
    studentDetails: StudentDetails[];
    studentDiscount: StudentDiscount[];
}