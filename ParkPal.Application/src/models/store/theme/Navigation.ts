export default class Navigation {
    background = '#F2F2F2';
    border = '#363636';
    icons = '#999999';
    text = '#999999';
    activeText = '#363636';
    activeIcon = '#363636';

    constructor(data: Pick<Navigation, "background" | "border" | "icons" | "text" | "activeText" | "activeIcon"> | null = null) {
        if(data) {
            this.background = data.background;
            this.border = data.border;
            this.icons = data.icons;
            this.text = data.text;
            this.activeText = data.activeText;
            this.activeIcon = data.activeIcon;
        }
    }

    setLightTheme() {
        this.background = '#F2F2F2';
        this.border = '#363636';
        this.icons = '#999999';
        this.text = '#999999';
        this.activeText = '#363636';
        this.activeIcon = '#363636';
    }

    setDarkTheme() {
        this.background = '#282828';
        this.border = '#FFFFFF';
        this.icons = '#eeeeee';
        this.text = '#d7d8da';
        this.activeText = '#FFFFFF';
        this.activeIcon = '#FFFFFF';
    }
}