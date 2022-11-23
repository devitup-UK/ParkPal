class Token {
    tokenId?: number;
    value?: string;

    constructor(data: Pick<Token, "tokenId" | "value"> | null = null) {
        if(data != null) {
            this.tokenId = data.tokenId;
            this.value = data.value;
        }
    }
}