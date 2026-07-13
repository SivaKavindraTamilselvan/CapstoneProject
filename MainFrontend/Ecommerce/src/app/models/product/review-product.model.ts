export class ReviewProductModel {
    constructor(
        public productId: number = 0,
        public approvalStatusId: number | null = null,
        public remarks: string = ""
    ) { }
}

export class ApprovalHistoryModel {
    constructor(
        public approvalHistoryId: number = 0,
        public entityType: string = "",
        public entityId: number = 0,

        public previousStatusName: string | null = null,
        public newStatusName: string | null = null,

        public reviewerType: string = "",
        public reviewerId: number = 0,

        public remarks: string | null = null,

        public reviewedAt: string = ""
    ) { }
}