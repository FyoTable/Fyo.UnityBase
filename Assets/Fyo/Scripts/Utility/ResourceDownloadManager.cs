using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fyo {
    public class ResourceDownloadManager : MonoBehaviour {
        public bool FinishedLoading = false;
        protected JSONObject Config;

        public Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
        public Dictionary<string, MovieTexture> MovieTextures = new Dictionary<string, MovieTexture>();

        public static string[] ImageFileExtensions = new string[] {
        ".jpg",
        ".jpeg",
        ".png"
    };

        public static string[] VideoFileExtensions = new string[] {
        ".webm",
        ".mp4"
    };

        protected virtual IEnumerator WaitForFilesToLoad() {
            while (FileCount > 0)
                yield return null;

            if (FileCount == 0) {
                Debug.Log("Files Loaded");
            }
        }

        protected IEnumerator ImportFromURL(string Url) {
            Debug.Log("Importing " + Url);
            WWW www = new WWW(Url);
            yield return www;
            if (www.isDone) {
                string FileName = Path.GetFileName(Url);

                if (ImageFileExtensions.Any(ext => FileName.EndsWith(ext))) {
                    Textures.Add(FileName, www.texture);
                    Debug.Log("Imported Texture " + FileName);
                } else if (VideoFileExtensions.Any(ext => FileName.EndsWith(ext))) {
                    MovieTexture mt = www.GetMovieTexture();
                    if (mt.duration > 0) {
                        MovieTextures.Add(FileName, mt);
                        Debug.Log("Imported Video " + FileName);
                    } else {
                        Debug.LogWarning("Unable to Import Video " + FileName);
                    }
                }

                PanelFilenames.Add(FileName);
                FileCount--;
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
                            FileCount--;
                            Debug.LogError("Unable to Write File \"" + Folder + FileName + "\"");
                        }
                    } else {
                        FileCount--;
                        Debug.LogWarning(Url + " download failed or Invalid URL");
                    }
                }
            } else {
                FileCount--;
                Debug.Log("Invalid Url File? " + UrlFile);
            }
        }

        int FileCount = 0;
        public List<string> PanelFilenames = new List<string>();
        public void Load(bool WriteDefuaultConfig) {
            FinishedLoading = false;
            PanelFilenames.Clear();
            //Read MarqueeData folder for URLs and Files
            string[] Filenames = Directory.GetFiles(Fyo.DefaultPaths.MarqueeData);
            FileCount = Filenames.Length;
            foreach (string Filename in Filenames) {
                if (Filename.EndsWith(".url", true, System.Globalization.CultureInfo.CurrentCulture)) {
                    StartCoroutine(DownloadFromURL(Filename, Fyo.DefaultPaths.MarqueeData));
                } else {
                    StartCoroutine(ImportFromURL(@"file://" + Filename));
                }
            }

            StartCoroutine(WaitForFilesToLoad());
        }
    }
}
