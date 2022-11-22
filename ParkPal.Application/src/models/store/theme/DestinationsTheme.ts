export default class DestinationsTheme {
    text = '#b7b7b7';
    title = '#000000';
    location = '#000000';
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
        this.title = '#000000';
        this.location = '#000000';
        this.buttonBackground = '#F2F2F2';
        this.buttonText = '#6E6E6E';
    }

    setDarkTheme() {
        this.text = '#b7b7b7';
        this.title = '#000000';
        this.location = '#000000';
        this.buttonBackground = '#3f3f3f';
        this.buttonText = '#F2F2F2';
    }
}