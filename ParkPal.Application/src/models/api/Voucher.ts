export default class Voucher {
    code?: string;

    constructor(data: Pick<Voucher, "code"> | null = null) {
        if(data != null) {
            this.code = data.code;
        }
    }
}