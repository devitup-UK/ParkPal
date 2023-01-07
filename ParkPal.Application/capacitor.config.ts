import {CapacitorConfig} from '@capacitor/cli';
import {KeyboardResize} from '@capacitor/keyboard';

const config: CapacitorConfig = {
  appId: 'DevItUp.ParkPal.Application',
  appName: 'ParkPal',
  webDir: 'dist',
  bundledWebRuntime: false,
  plugins: {
    PushNotifications: {
      "presentationOptions": ["badge", "sound", "alert"]
    },
    Keyboard: {
      resize: KeyboardResize.None,
      resizeOnFullScreen: false
    }
  }
};

export default config;
