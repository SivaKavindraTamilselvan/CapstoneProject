export class ChangePasswordModel {
    constructor(
        public currentPassword : string = '',
        public newPassword : string = '',
    ) { }

}