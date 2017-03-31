using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GameLauncher : MonoBehaviour {

    public void Launch(string Path, string AppName) {
#if UNITY_ANDROID
        bool fail = false;
        string bundleId = Path + "." + AppName; //"com.company" + "." + "appname"; // your target bundle id
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
        } catch (System.Exception e) {
            Application.OpenURL("https://google.com");
        }
        
        if(launchIntent != null) {
            ca.Call("startActivity", launchIntent);
            up.Dispose();
            ca.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();
        }

#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Process foo = new Process();
        foo.StartInfo.FileName = AppName;
        foo.StartInfo.Arguments = Path;
        foo.Start();
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
#elif UNITY_STANDALONE_LINUX // || UNITY_EDITOR_LINUX Maybe eventually?
#endif
    }
}
