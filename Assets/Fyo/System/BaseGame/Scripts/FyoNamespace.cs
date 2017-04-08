using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    public static class Files {
        public static string BaseController {
            get {
                return "BaseController.zip";
            }
        }
    }

    public static class Paths {
        public static string Configuration {
            get {
#if UNITY_EDITOR
                return Application.dataPath + "/Config~/";
#else
                return Application.persistentDataPath + "/Config/";
#endif
            }
        }

        public static string Controllers {
            get {
#if UNITY_EDITOR
                return Application.dataPath + "/Controllers~/";
#else
                return Application.dataPath + "/Controllers/";
#endif
            }
        }

        public static string MarqueeData {
            get {
                return Configuration + "Marquee/";
            }
        }
        public static string Downloads {
            get {
                return Configuration + "Downloads/";
            }
        }

    }
}