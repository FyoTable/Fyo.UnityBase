using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

//NOTE: Don't forget to add WRITE_EXTERNAL_STORAGE to your Android manifest for this to work
// <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

public class UrlDownloader : MonoBehaviour {
    //TODO: Implement a ResourceDownloader for URLs, download the files, then rescan. Have to in order to use URLs using unity's WWW implementation
    public string TargetFolder = "Downloads";
    public List<string> Urls = new List<string>();

    protected Coroutine DownloadCoroutine = null;

    public delegate void DownloadCallback();
    public DownloadCallback FinishedCallback = null;

    #region Statics
    public static Regex DiegoPeriniRegex = new Regex(@"^(?:(?:https?|ftp)://)(?:\S+(?::\S*)?@)?(?:(?!10(?:\.\d{1,3}){3})(?!127(?:\.\d{1,3}){3})(?!169\.254(?:\.\d{1,3}){2})(?!192\.168(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]+-?)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]+-?)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:/[^\s]*)?$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    public static bool UrlIsValid(string Url) {
        return DiegoPeriniRegex.IsMatch(Url);
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    //TODO: Determine if this should be a string literal?
    public static bool FilePathIsValid(string FilePath) {
        return FilePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
    }
    public static bool FilenameIsValid(string FullPath) {
        return FullPath.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
    }
#else
    public static Regex UnixRegex = new Regex(@"\([^\0 !$`&*()+]\|\\\(\ |\!|\$|\`|\&|\*|\(|\)|\+\)\)\+", RegexOptions.Multiline);
    public static bool FilePathIsValid(string FilePath) {
        return WindowsRegex.IsMatch(FilePath);
    }
#endif

    public static void DownloadUrls(string DownloadPath, List<string> Urls, DownloadCallback finishedCallback) {
        GameObject gameObject = new GameObject("Downloader");
        UrlDownloader dl;
        for (int u = 0; u < Urls.Count; u++) {
            if (UrlIsValid(Urls[u])) {
                Debug.Log("Attempting Download from: " + Urls[u]);
                dl = gameObject.AddComponent<UrlDownloader>();
                dl.FinishedCallback = finishedCallback;
                dl.StartDownload(Urls[u], Fyo.Paths.MarqueeData + @"/" + DownloadPath + @"File" + u.ToString() + ".download");
            } else {
                Debug.LogWarning("Invalid URL: " + Urls[u]);
            }
        }
    }

#endregion
    private void Start() {
        if (Urls.Count > 0) {
            Debug.Log("Attempting to download from " + Urls.Count.ToString() + " url" + ((Urls.Count > 1) ? "s." : "."));
            DownloadUrls(TargetFolder, Urls, FinishedCallback);
            Destroy(gameObject);
        }
    }

    private IEnumerator Download(string Url, string DestinationPath, DownloadCallback finishedCallback) {
        if (FinishedCallback == null)
            Debug.Log("Callback is Null");
        string strBasePath = Fyo.Paths.Configuration + TargetFolder;
        string strPath = strBasePath + @"/" + Path.GetFileName(Url);
        
        if (!Directory.Exists(Fyo.Paths.Configuration + TargetFolder)) {
            if (!Directory.CreateDirectory(strBasePath).Exists) {
                Debug.LogError("Could not create path \"" + strBasePath);
                yield return null;
                Destroy(gameObject);
            } else {
                Debug.Log("Created folder \"" + strBasePath + "\"");
            }
        }

        Debug.Log("Downloading \"" + Url + "\" to \"" + strPath + "\"");
        WWW www = new WWW(Url);
        yield return www;
        //Debug.Log("Download " + (www.progress * 100).ToString("G4") + @"%");
        if (www.isDone) {
            if (File.Exists(strPath))
                Debug.LogWarning("Overwiting file \"" + strPath + "\"");

            if (www.progress == 1.0f) {
                if (www.bytes.Length > 0) {
                    File.WriteAllBytes(strPath, www.bytes);
                    Debug.Log("Wrote \"" + strPath + "\" " + www.bytes.Length + " bytes");
                } else {
                    Debug.Log("Download Failed for + \"" + strPath + "\"");
                }

                if (finishedCallback != null)
                    finishedCallback();

            } else {
                Debug.LogWarning("Download failed for \"" + strPath + "\"");
            }
            Destroy(this);
        }
    }

    public void StartDownload(string Url, string DestinationPath) {
        Debug.Log("Attempting to download to " + DestinationPath);
        if (UrlIsValid(Url)) {
            if (FilePathIsValid(DestinationPath)) {
                if (DownloadCoroutine == null) {
                    Debug.Log("Downloading From \"" + Url + "\" to Path: \"" + DestinationPath + "\"");
                    DownloadCoroutine = StartCoroutine(Download(Url, DestinationPath, FinishedCallback));
                }
            } else {
                Debug.LogError("Invalid File Path: \"" + DestinationPath + "\"");
            }
        } else {
            Debug.LogError("Invalid Url: \"" + Url + "\"");
        }
    }
}
