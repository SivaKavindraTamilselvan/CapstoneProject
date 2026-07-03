export class NotificationFilterModel {
    constructor(
        public notificationTypeId?: number,
        public isRead?: boolean,
        public minCreatedAt?: Date,
        public maxCreatedAt?: Date,
        public pageNumber:number=1,
        public pageSize:number=10
    ) { }
}