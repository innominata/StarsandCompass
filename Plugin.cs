using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UltimateSurvival.StandardAssets;
using UnityEngine;
using UnityEngine.UI;

namespace StarsandCompass
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class StarsandGummy : BaseUnityPlugin
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

        private void Awake()
        {
            Logger = new ManualLogSource("Compass");
            Logger.LogInfo("Starsand Compass Mod Loading");
            Harmony.CreateAndPatchAll(typeof(StarsandGummy));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GeneralManager), "Start")]
        public static void Start(ref GeneralManager __instance)
        {
            var fpscounter = GameObject.Find("In-Game GUI/Canvas/2-HUD/").GetComponentInChildren<FPSDisplayer>().gameObject;
            var fpsTransform = fpscounter.transform;
            var compassObject = Instantiate(fpscounter, fpsTransform.parent, false);
            compassObject.name = "Compass";
            var compassRect = compassObject.GetComponent<RectTransform>();
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
            }
            Pointer = TextTemplate;
            Pointer.text = "|";
            Pointer.color = Color.gray;
            Pointer.alignment = TextAnchor.UpperCenter;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GeneralManager), "Update")]
        public static void Update(ref GeneralManager __instance)
        {
            var angle = __instance.FpPlayer.m_LookAngles.y;
            var iAngle = angle % 360;
            var differences = new float[8];
            for (var i = 0; i < 8; i++)
            {
                differences[i] = dirArray[i] - iAngle;
                if (differences[i] < -180) differences[i] += 360;
                else if (differences[i] > 180) differences[i] -= 360;
                var _x = Mathf.Clamp(differences[i], -90, 90);
                var alpha = (90f - Mathf.Abs(_x)) / 50f;
                alpha = Mathf.Clamp(alpha, 0, 1f);
                compassTexts[i].color = new Color(1, 1, 1, alpha);
            }
        }


    }
}