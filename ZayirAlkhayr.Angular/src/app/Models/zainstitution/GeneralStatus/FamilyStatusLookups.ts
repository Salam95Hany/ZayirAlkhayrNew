
export interface FamilyStatusLookups {
    categories: FamilyCategories[];
    nationalities: FamilyNationalities[];
    familyNeeds: FamilyNeedTypes[];
    familyNeedCategories:FamilyCategories[];
    statusTypes: FamilyStatusTypes[],
    patientTypes: FamilyPatientTypes[]

}

export interface FamilyCategories {
    id: number;
    name: string;
    insertUser: string;
    insertDate: string | null;
    updateUser: string;
    updateDate: string | null;
}

export interface FamilyNationalities {
    id: number;
    name: string;
    insertUser: string;
    insertDate: string | null;
    updateUser: string;
    updateDate: string | null;
}

export interface FamilyStatusTypes {
    id: number;
    name: string;
}

export interface FamilyNeedTypes {
    id: number;
    categoryId: number;
    name: string;
    insertUser: string;
    insertDate: string | null;
    updateUser: string;
    updateDate: string | null;
}

export interface FamilyPatientTypes {
    id: number;
    name: string;
    insertUser: string;
    insertDate: string | null;
    updateUser: string;
    updateDate: string | null;
}