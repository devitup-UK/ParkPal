import Navigation from "@/models/store/theme/Navigation";
import SettingsTheme from "@/models/store/theme/SettingsTheme";
import Header from "@/models/store/theme/Header";
import DestinationsTheme from "@/models/store/theme/DestinationsTheme";

export default class Theme {
    background = '#e3e3e3';
    text = '#000000';
    loadingIcon = '#000000';
    actionButtonBackground = '#000000';
    actionButtonText = '#FFFFFF';
    resetButtonBackground = '#BABABA';
    resetButtonText = '#FFFFFF';
    selectionBoxBackground = '#FFFFFF';
    selectionBoxText = '#9d9d9d';
    selectionBoxBorder = '#d5d5d5';
    searchBoxBackground = '#d3d3d3';
    searchBoxText = '#747474';
    searchBoxIcons = '#666666';
    header = new Header();
    navigation = new Navigation();
    destinations = new DestinationsTheme();
    settings = new SettingsTheme();
    darkMode = false;

    constructor(data: Pick<Theme, "background" | "text" | "loadingIcon" | "actionButtonBackground" | "actionButtonText" | "resetButtonBackground" | "resetButtonText" | "selectionBoxBackground" | "selectionBoxBorder" | "selectionBoxText" | "searchBoxBackground" | "searchBoxText" | "searchBoxIcons" | "header" | "navigation" | "destinations" | "settings" | "darkMode"> | null = null) {
        if(data) {
            this.background = data.background;
            this.text = data.text;
            this.loadingIcon = data.loadingIcon;
            this.actionButtonBackground = data.actionButtonBackground;
            this.actionButtonText = data.actionButtonText;
            this.resetButtonBackground = data.resetButtonBackground;
            this.resetButtonText = data.resetButtonText;
            this.selectionBoxBackground = data.selectionBoxBackground;
            this.selectionBoxText = data.selectionBoxText;
            this.selectionBoxBorder = data.selectionBoxBorder;
            this.searchBoxBackground = data.searchBoxBackground;
            this.searchBoxText = data.searchBoxText;
            this.searchBoxIcons = data.searchBoxIcons;
            this.header = new Header(data.header);
            this.navigation = new Navigation(data.navigation);
            this.destinations = new DestinationsTheme(data.destinations);
            this.settings = new SettingsTheme(data.settings);
            this.darkMode = data.darkMode;
        }
    }

    setLightTheme() {
        this.background = '#e3e3e3';
        this.text = '#000000';
        this.loadingIcon = '#000000';
        this.actionButtonBackground = '#000000';
        this.actionButtonText = '#FFFFFF';
        this.resetButtonBackground = '#BABABA';
        this.resetButtonText = '#FFFFFF';
        this.selectionBoxBackground = '#FFFFFF';
        this.selectionBoxText = '#9d9d9d';
        this.selectionBoxBorder = '#d5d5d5';
        this.searchBoxBackground = '#d3d3d3';
        this.searchBoxText = '#747474';
        this.searchBoxIcons = '#666666';
        this.header.setLightTheme();
        this.navigation.setLightTheme();
        this.destinations.setLightTheme();
        this.settings.setLightTheme();
    }

    setDarkTheme() {
        this.background = '#3c3c3c';
        this.text = '#FFFFFF';
        this.loadingIcon = '#FFFFFF';
        this.actionButtonBackground = '#FFFFFF';
        this.actionButtonText = '#000000';
        this.resetButtonBackground = '#BABABA';
        this.resetButtonText = '#FFFFFF';
        this.selectionBoxBackground = '#000000';
        this.selectionBoxText = '#FFFFFF';
        this.selectionBoxBorder = '#464646';
        this.searchBoxBackground = '#575757';
        this.searchBoxText = '#FFFFFF';
        this.searchBoxIcons = '#FFFFFF';
        this.header.setDarkTheme();
        this.navigation.setDarkTheme();
        this.destinations.setDarkTheme();
        this.settings.setDarkTheme();
    }
}