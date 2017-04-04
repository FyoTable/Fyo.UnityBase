using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.Video;

public class MarqueeMenuCfg : JSONObject {
    public string FilePath = @"./Marquee";
    public string ConfigFile = @"Marquee.cfg";
    protected string ConfigPath {
        get {
            return FilePath + @"/" + ConfigFile;
        }
    }

    TextAsset ConfigAsset = null;
    JSONObject PanelList = new JSONObject();

    MarqueeMenuCfg() : base() {
        AddField("Panels", PanelList);
    }

    public List<string> ImageExtensions = new List<string> {
        ".png", ".jpg", ".jpeg" //Graphic Formats
    };
    
    public List<string> VideoExtensions = new List<string> {
        ".mp4", ".webm", ".avi",  //Video Formats
    };

    public List<string> UrlExtensions = new List<string> {
        ".url" //Urls
    };

    protected List<string> ResourcePaths = new List<string>();

    public List<Texture2D> Images = new List<Texture2D>();
    public List<VideoClip> Videos = new List<VideoClip>();

    //TODO: Implement a ResourceDownloader for URLs, download the files, then rescan. Have to in order to use URLs using unity's WWW implementation
    /*
    protected IEnumerator TryLoadUrl(string url) {
        WWW www = new WWW(url);
        //Write to Proper Folder
    }

    protected void DownloadUrls() {
        string fnLower;
        GameObject Downloader = new ResourceDownloader();
        foreach (string fn in Directory.GetFiles(FilePath)) {
            //Scan filenames in directory, Ensure File Paths are in correct case
            fnLower = fn.ToLower();
            string urlLower;
            for (int urlIdx = 0; urlIdx < UrlExtensions.Count; urlIdx++) {
                if (fnLower.EndsWith(UrlExtensions[urlIdx].ToLower())) {
                    //TODO: Read URL address and determine file extension
                    if (File.Exists(fnLower)) {
                        urlLower = string.Empty;
                        //Read URL from file
                        try {
                            //Try reading the file
                            urlLower = File.ReadAllText(fnLower).ToLowerInvariant();
                        } catch (IOException ex) {
                            //Fail Gracefully
                            Debug.LogError(ex.Message);
                        }

                        if (urlLower != string.Empty) {
                            //TODO: Implement ResourceDownloader to be used like this:
                            //Download resource to proper folder
                            Downloader.Download(urlLower, ref this);
                    } else {
                        //File Doesn't Exist somehow
                        Debug.LogWarning("File Deleted?");
                    }
                }
            }
        }
    }
    */

    protected void LoadResourceFromConfiguration() {
        //TODO: Read files as defined in config
    }

    protected void LoadResourcesFromFolder() {
        string fnLower;
        foreach (string fn in Directory.GetFiles(FilePath)) {
            //Scan filenames in directory, Ensure File Paths are in correct case
            fnLower = fn.ToLower();
            if (!TryLoadResource(fnLower)) {
                Debug.Log("Not loading resource \"" + fn + "\"");
            }
        }
    }

    protected bool TryLoadResource(string filename) {
        //Compare filename with Image Extensions
        for (int imgIdx = 0; imgIdx < ImageExtensions.Count; imgIdx++) {
            if (filename.EndsWith(ImageExtensions[imgIdx].ToLower())) {
                //Found Image Path
                Texture2D texture = null;
                try {
                    //Try loading file
                    texture = Resources.Load<Texture2D>(filename);
                } catch (UnityException ex) {
                    //Fail Gracefully
                    Debug.LogError(ex.Message);
                }

                if (texture != null) {
                    //Load the texture into Images
                    Images.Add(texture);
                    ResourcePaths.Add(filename);
                    return true;
                }

                return false;
            }
        }

        //Compare filename with Video Extensions
        for (int vidIdx = 0; vidIdx < VideoExtensions.Count; vidIdx++) {
            if (filename.EndsWith(VideoExtensions[vidIdx].ToLower())) {
                VideoClip clip = null;
                try {
                    //Try loading file
                    clip = Resources.Load<VideoClip>(filename);
                } catch (UnityException ex) {
                    //Fail Gracefully
                    Debug.LogError(ex.Message);
                }

                if (clip != null) {
                    //Load the texture into Images
                    Videos.Add(clip);
                    ResourcePaths.Add(filename);
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    protected void WriteDefaultConfig() {
        //TODO: Call Downloader first, and delete url files after replaced with resources

        //Clear and Unload current images and videos
        foreach (Texture2D tex in Images)
            Resources.UnloadAsset(tex);
        Images.Clear();

        foreach (VideoClip vc in Videos)
            Resources.UnloadAsset(vc);
        Videos.Clear();

        //Load all of the resources in the folder fresh
        LoadResourcesFromFolder();

        //Default Config should scan folder for entries and build a new Config
        try {
            //Rewrite Config from scratch
            FileStream fs = new FileStream(ConfigPath, FileMode.Truncate);
            if (fs.CanWrite) {
                //Convert JSON to String
                string strCfg = ToString(false);
                byte[] data = new UTF8Encoding(true).GetBytes(strCfg);
                fs.Write(data, 0, data.Length);
            } else {
                Debug.LogError("Unable to write to " + ConfigPath + "!\n");
            }
        } catch (System.IO.IOException ioEx) {
            Debug.LogError("Unable to write to " + ConfigPath + "!\nIOException: " + ioEx.Message);
        }
    }

    protected void Start() {
        //Try to read Config file
        if (File.Exists(ConfigPath)) {
            try {
                ConfigAsset = Resources.Load<TextAsset>(ConfigPath);
            } catch (UnityException ex) {
                Debug.LogWarning("Unable to read file \"" + ConfigPath + "\", attempting to write Default Config\n" + ex.Message);
                WriteDefaultConfig();
                if (File.Exists(ConfigPath)) {
                    ConfigAsset = Resources.Load<TextAsset>(ConfigPath);
                }
            }
        } else {
            LoadResourcesFromFolder();
            WriteDefaultConfig();
            if (File.Exists(ConfigPath)) {
                ConfigAsset = Resources.Load<TextAsset>(ConfigPath);
            }
        }
    }   
}
