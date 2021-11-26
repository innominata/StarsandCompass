using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UltimateSurvival.StandardAssets;
using UnityEngine;
using UnityEngine.UI;

namespace StarsandCompass
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class StarsandCompass : BaseUnityPlugin
    {
        public const float _S = 90f;
        public const float _SW = 135f;
        public const float _W = 180f;
        public const float _NW = 225f;
        public const float _N = 270f;
        public const float _NE = 315f;
        public const float _E = 0f;
        public const float _SE = 45f;
        public new static ManualLogSource Logger;
        public static Text[] compassTexts;
        public static Text Pointer;
        public static float _y, _z;
        public static float[] dirArray = { _E, _SE, _S, _SW, _W, _NW, _N, _NE };
        public static string[] cardArray = { "E", "SE", "S", "SW", "W", "NW", "N", "NE" };
        public static GeneralManager gm;
        public static AssetBundle ab = AssetBundle.LoadFromFile(Path.GetDirectoryName(Assembly.GetAssembly(typeof(StarsandCompass)).Location) + "\\font");
        public static Font font;
        private void Awake()
        {
            Logger = new ManualLogSource("Compass");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            Logger.LogInfo("Starsand Compass Mod Loading");
            Harmony.CreateAndPatchAll(typeof(StarsandCompass));
            if(ab != null)
            {
                font = ab.LoadAsset<Font>("Assets/Komon.ttf");
                
            }
            else
            {
                Debug.Log("FAILED TO LOAD ASSET BUNDLE");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GeneralManager), "Start")]
        public static void Start(ref GeneralManager __instance)
        {
            gm = __instance;
            var fpscounter = GameObject.Find("In-Game GUI/Canvas/2-HUD/").GetComponentInChildren<FPSDisplayer>().gameObject;
            var fpsTransform = fpscounter.transform;
            var compassObject = Instantiate(fpscounter, fpsTransform.parent, false);
            
            compassObject.name = "Compass";
            var compassRect = compassObject.GetComponent<RectTransform>();
            compassRect.pivot = new Vector2(0.5f, 1f);
            _y = compassRect.localPosition.y;
            _z = compassRect.localPosition.z;
            compassRect.localPosition = new Vector3(0, _y, _z);
            DestroyImmediate(compassRect.GetComponent<FPSDisplayer>());
            compassTexts = new Text[8];
            var TextTemplate = compassRect.GetComponent<Text>();
            for (var i = 0; i < 8; i++)
            {
                compassTexts[i] = Instantiate(compassObject, compassRect.parent, false).GetComponent<Text>();
                compassTexts[i].text = cardArray[i];
                compassTexts[i].alignment = TextAnchor.UpperCenter;
                compassTexts[i].name = "CompassText_" + cardArray[i];
                if(font != null)
                {
                    fpscounter.GetComponent<Text>().font = font;
                    compassTexts[i].font = font;
                }
                else
                {
                    Debug.Log("FAILED TO LOAD FONT FROM ASSET BUNDLE");
                }
                // compassTexts[i].transform.SetParent(TextTemplate.transform);
            }
            Pointer = TextTemplate;
            Pointer.text = "|";
            Pointer.color = Color.gray;
            Pointer.alignment = TextAnchor.UpperCenter;

            
            
            // if (gm == null) gm = GameObject.FindGameObjectWithTag("manager").GetComponent<GeneralManager>();
            
            // Logger.LogInfo("Start");
            // Logger.LogInfo((gm == null).ToString());
            // Logger.LogInfo((gm.MainPlayer == null).ToString());
            // MapManager mm = GameObject.FindGameObjectWithTag("manager").GetComponent<MapManager>();
            // mm.PlaceMarker(gm.MainPlayer);
            // Logger.LogInfo("1");
            // var marker = mm.markersPlaced[mm.markersPlaced.Count() - 1];
            // marker.GetComponent<Image>().color = Color.blue;
            
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GeneralManager), "Update")]
        public static void Update(ref GeneralManager __instance)
        {
            var angle = __instance.FpPlayer.m_LookAngles.y;
            SetCompassAngle(angle % 360);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DromaderFPScontroller), "Update")]
        public static void CamelUpdate(ref DromaderFPScontroller __instance)
        {
            var Vangle = __instance.m_Camera.transform.eulerAngles;
            Logger.LogInfo(Vangle);
            SetCompassAngle(Vangle.y % 360);
        }

        public static void SetCompassAngle(float angleInDegrees)
        {
            var differences = new float[8];
            for (var i = 0; i < 8; i++)
            {
                differences[i] = dirArray[i] - angleInDegrees;
                if (differences[i] < -180) differences[i] += 360;
                else if (differences[i] > 180) differences[i] -= 360;
                var _x = Mathf.Clamp(differences[i], -90, 90);
                var alpha = (90f - Mathf.Abs(_x)) / 50f;
                alpha = Mathf.Clamp(alpha, 0, 1f);
                compassTexts[i].color = new Color(1, 1, 1, alpha);
                compassTexts[i].GetComponent<RectTransform>().localPosition = new Vector3(_x, _y, _z);
            }
        }
    }
}