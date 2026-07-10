export class AdminAttributeModel{
    constructor(
        public attributeMasterId : number = 0,
        public attributeName : string = "",
        public isActive : boolean = true,
        public addedByAdminId : number = 0 ,
        public addedUserName : string = "",
        public createdAt: Date = new Date()
    ){}
}