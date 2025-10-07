import { FamilyDetails, FamilyExpenses, FamilyExtraDetails, FamilyIncome, FamilyNeeds, FamilyPatient, FamilyStatus } from "./AddFamilyStatusModel";
import { FamilyStatusLookups } from "./FamilyStatusLookups";

export interface UpdateFamilyStatusLookups {
    lookups: FamilyStatusLookups;
    familyStatus: FamilyStatus;
    familyIncome: FamilyIncome;
    familyExpenses: FamilyExpenses;
    familyExtraDetails: FamilyExtraDetails;
    familyDetails: FamilyDetails[];
    familyPatient: FamilyPatientGroup[];
    familyNeeds: FamilyNeeds[];
}


export interface FamilyPatientGroup {
    id?: number;
    familyStatusId?: number;
    name?: string;
    patientTypeIds?: number[];
    patientTypeNames?: string;
    patientTypeList?: FamilyPatientTypeNames[];
    patientDate?: string;
    specialization?: string;
    isMedicalReport?: boolean | null;
    isNeedProcess?: boolean | null;
    familyName?: string;
}


export interface FamilyPatientTypeNames {
    id: number;
    name: string;
}