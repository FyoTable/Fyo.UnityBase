using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MarqueeConfiguration : MonoBehaviour {
    public string ConfigFileName = "Config.cfg";
    protected JSONObject Configuration;

    public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
    public Dictionary<string, MovieTexture> Movies = new Dictionary<string, MovieTexture>();

    protected string[] ImageFileExtensions = new string[] {
        ".jpg",
        ".jpeg",
        ".png"
    };

    protected string[] VideoFileExtensions = new string[] {
        ".webm",
        ".mp4"
    };

    protected IEnumerator ImportFromURL(string Url) {
        Debug.Log("Importing " + Url);
        WWW www = new WWW(Url);
        yield return www;
        if (www.isDone) {
            string FileName = Path.GetFileName(Url);

            if (ImageFileExtensions.Any(ext => FileName.EndsWith(ext)) ) {
                Textures.Add(FileName, www.texture);
                Debug.Log("Imported Texture " + FileName);
            } else if (VideoFileExtensions.Any(ext => FileName.EndsWith(ext)) ) {
                MovieTexture mt = www.GetMovieTexture();
                Movies.Add(FileName, mt);
                Debug.Log("Imported Video " + FileName);
            }
        }
    }

    protected IEnumerator DownloadFromURL(string UrlFile, string Folder) {
        Debug.Log("Reading Url File \"" + UrlFile + "\"");
        string[] UrlLines = File.ReadAllLines(UrlFile);
        if (UrlLines.Length > 1) {
            string Url = UrlLines[1].Substring(4);
            Debug.Log("Downloading " + Url + " to " + Folder);
            WWW www = new WWW(Url);
            yield return www;
            if (www.isDone) {
                if (www.bytes.Length > 0) {
                    string FileName = Path.GetFileName(Url);
                    Debug.Log("Finished Downloading \"" + FileName + "\" " + www.bytes.Length.ToString() + " bytes");
                    File.WriteAllBytes(Folder + FileName, www.bytes);
                    if (File.Exists(Folder + FileName)) {
                        Debug.Log("Wrote " + Folder + FileName);
                        //Check if we wrote the file
                        if (File.Exists(UrlFile)) {
                            //Delete URL since we now have the source file
                            Debug.Log("Replaced " + UrlFile + " with " + Folder + FileName);
                            File.Delete(UrlFile);
                        }
                        //Import into Resource System
                        StartCoroutine(ImportFromURL(@"file://" + Folder + FileName));
                    } else {
                        Debug.LogError("Unable to Write File \"" + Folder + FileName + "\"");
                    }
                } else {
                    Debug.LogWarning(Url + " download failed or Invalid URL");
                }
            }
        } else {
            Debug.Log("Invalid Url File? " + UrlFile);
        }
    }

    public void Load() {
        //Read MarqueeData folder for URLs and Files
        foreach (string Filename in Directory.GetFiles(Fyo.Paths.MarqueeData)) {
            if (Filename.EndsWith(".url", true, System.Globalization.CultureInfo.CurrentCulture)) {
                StartCoroutine(DownloadFromURL(Filename, Fyo.Paths.MarqueeData));
            } else {
                StartCoroutine(ImportFromURL(@"file://" + Filename));
            }
        }
    }

    private void Start() {
        Load();
    }

}
