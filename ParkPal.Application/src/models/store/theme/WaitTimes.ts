export default class WaitTimes {
    text = '#FFFFFF';
    background = '#BABABA';

    constructor(data: Pick<WaitTimes, "text" | "background"> | null = null) {
        if(data) {
            this.text = data.text;
            this.background = data.background;
        }
    }

    setLightTheme() {
        this.text = '#FFFFFF';
        this.background = '#BABABA';
    }

    setDarkTheme() {
        this.text = '#FFFFFF';
        this.background = '#4B4B4B';
    }
}