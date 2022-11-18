export default class DestinationsTheme {
    text = '#b7b7b7';
    title = '#FFFFFF';
    location = '#FFFFFF';
    buttonBackground = '#F2F2F2';
    buttonText = '#6E6E6E';

    constructor(data: Pick<DestinationsTheme, "text" | "title" | "location" | "buttonBackground" | "buttonText"> | null = null) {
        if(data) {
            this.text = data.text;
            this.title = data.title;
            this.location = data.location;
            this.buttonBackground = data.buttonBackground;
            this.buttonText = data.buttonText;
        }
    }

    setLightTheme() {
        this.text = '#b7b7b7';
        this.title = '#FFFFFF';
        this.location = '#FFFFFF';
        this.buttonBackground = '#F2F2F2';
        this.buttonText = '#6E6E6E';
    }

    setDarkTheme() {
        this.text = '#b7b7b7';
        this.title = '#FFFFFF';
        this.location = '#FFFFFF';
        this.buttonBackground = '#3f3f3f';
        this.buttonText = '#F2F2F2';
    }
}