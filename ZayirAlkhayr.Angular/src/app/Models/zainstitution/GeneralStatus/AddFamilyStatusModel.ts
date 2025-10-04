import { FamilyPatientGroup } from "./UpdateFamilyStatusLookups";

export interface AddFamilyStatusModel {
    familyStatus: FamilyStatus;
    familyIncome: FamilyIncome;
    familyExpenses: FamilyExpenses;
    familyExtraDetails: FamilyExtraDetails;
    familyDetails: FamilyDetails[];
    familyPatient: FamilyPatientGroup[];
    familyNeeds: FamilyNeeds[];
}

export interface FamilyStatus {
    id: number;
    statusTypeId: number;
    categoryId: number;
    nationalityId: number;
    code: number;
    name: string;
    fname: string;
    address: string;
    village: string;
    center: string;
    governorate: string;
    phone: string;
    phone1: string;
    supportingParty: string;
    nationalId: string;
    relevance: string;
    age: number | null;
    maritalStatus: string;
    education: string;
    jop: string;
    reasonOfRefuse: string;
    addedDate: string;
    insertUser: string;
    insertDate: string | null;
    updateUser: string;
    updateDate: string | null;
}
export interface FamilyIncome {
    id?: number;
    familyStatusId?: number;
    fatherJop: number | null;
    motherJop: number | null;
    childernsJop: number | null;
    affairSpension_SocialSolidarity: number | null;
    project: number | null;
    liveStock_Lands: number | null;
    organization_ZakatCommittee: number | null;
    insurancePension: number | null;
    comments: string;
    other: number;
    totalFamilyIncome: number | null;
}

export interface FamilyExpenses {
    id?: number;
    familyStatusId?: number;
    rent_Electricity_Water_Gas_Sewage: number | null;
    medicalExamination_Treatment: number | null;
    schoolExpenses: number;
    installment_debts: number | null;
    physiotherapySessions: number | null;
    analysis: number | null;
    satisfactoryTransfers: number | null;
    medicalXRays: number | null;
    isMinisterialSupply: boolean | null;
    isFoodBank: boolean | null;
    totalFamilyExpenses: number | null;
    netFamilyIncome: number | null;
    familyCount: number | null;
}

export interface FamilyExtraDetails {
    id: number;
    familyStatusId: number;
    statusDescription: string;
    housingNeedsAndStatus: string;
    researcherNotes: string;
    referencesNotes: string;
    lastVisitDate: string | null;
    personalPapers: string;
}

export interface FamilyDetails {
    id?: number;
    familyStatusId?: number;
    name: string;
    relevance: string;
    age: number | null;
    maritalStatus: string;
    education: string;
    jop: string;
    oldName: string;
    nationalId: string;
    childernsCount?: number | null;
    familyMembersCount?: number | null;
}

export interface FamilyPatient {
    id?: number;
    familyStatusId?: number;
    name: string;
    patientTypeId: number;
    patientTypeName?: string;
    patientDate: string | null;
    specialization: string;
    isMedicalReport: boolean | null;
    isNeedProcess: boolean;
}

export interface FamilyNeeds {
    id?: number;
    statusId?: number;
    needTypeId: number;
    categoryName: string;
    categoryId: number;
    name: string;
    isWaiting: boolean;
    deliveryDate: string | null;
}