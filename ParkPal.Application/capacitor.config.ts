import { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'DevItUp.ParkPal.Application',
  appName: 'ParkPal',
  webDir: 'dist',
  bundledWebRuntime: false,
  plugins: {
    PushNotifications: {
      "presentationOptions": ["badge", "sound", "alert"]
    }
  }
};

export default config;
