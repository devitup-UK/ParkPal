package DevItUp.ParkPal.Application;

import android.os.Bundle;

import com.getcapacitor.BridgeActivity;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.analytics.Analytics;
import com.microsoft.appcenter.crashes.Crashes;

public class MainActivity extends BridgeActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        AppCenter.start(getApplication(), "6d4bb92f-acea-4e66-a6ae-5b4dba300bdf", Analytics.class, Crashes.class);

        super.onCreate(savedInstanceState);
    }
}
