export default class SettingsTheme {
    settingBackground = '#FFFFFF';
    settingText = '#404040';
    settingArrow = '#c0c0c0';
    settingBorder = '#c8c7cc';

    constructor(data: Pick<SettingsTheme, "settingBackground" | "settingText" | "settingArrow" | "settingBorder"> | null = null) {
        if(data) {
            this.settingBackground = data.settingBackground;
            this.settingText = data.settingText;
            this.settingArrow = data.settingArrow;
            this.settingBorder = data.settingBorder;
        }
    }

    setLightTheme() {
        this.settingBackground = '#FFFFFF';
        this.settingText = '#404040';
        this.settingArrow = '#c0c0c0';
        this.settingBorder = '#c8c7cc';
    }

    setDarkTheme() {
        this.settingBackground = '#1a1c1f';
        this.settingText = '#FFFFFF';
        this.settingArrow = '#FFFFFF';
        this.settingBorder = '#2d2d2d';
    }
}