export class NotificationResponseModel {
  constructor(
    public notificationId: number = 0,
    public userId: number = 0,
    public title: string = '',
    public message: string = '',
    public notificationTypeId: number = 0,
    public notificationTypeName: string = '',
    public referenceType: string | null,
    public referenceId: number | null,
    public isRead: boolean = false,
    public createdAt: string = '',
    public readAt: string | null
  ) {}
}