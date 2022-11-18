export default class Header {
    background = '#F2F2F2';
    border = '#dbdbdb';
    icons = '#363636';
    text = '#000000';
    darkMode = false;

    constructor(data: Pick<Header, "background" | "border" | "icons" | "text" | "darkMode"> | null = null) {
        if(data) {
            this.background = data.background;
            this.border = data.border;
            this.icons = data.icons;
            this.text = data.text;
            this.darkMode = data.darkMode;
        }
    }

    setLightTheme() {
        this.background = '#F2F2F2';
        this.border = '#dbdbdb';
        this.icons = '#363636';
        this.text = '#000000';
    }

    setDarkTheme() {
        this.background = '#282828';
        this.border = '#343434';
        this.icons = '#eeeeee';
        this.text = '#FFFFFF';
    }
}