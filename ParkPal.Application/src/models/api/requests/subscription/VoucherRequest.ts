export default class VoucherRequest {
    code?: string;

    constructor(data: Pick<VoucherRequest, "code"> | null = null) {
        if(data != null) {
            this.code = data.code;
        }

    }
}