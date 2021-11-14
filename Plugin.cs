using System.Net.Mime;
using System.Runtime.InteropServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;
using TMPro;
using UltimateSurvival.AI;
using UltimateSurvival.GUISystem;
using UltimateSurvival.StandardAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StarsandCompass
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class StarsandGummy : BaseUnityPlugin
    {
        public new static ManualLogSource Logger;

        private void Awake()
        {
            Logger = new ManualLogSource("Compass");
            Logger.LogInfo("Starsand Compass Mod Loading");
            Harmony.CreateAndPatchAll(typeof(StarsandGummy));
        }

        public static void Log(string message)
        {
            Logger.LogInfo(message);
        }
        public static Text[] compassTexts;
        public static Text debugText;
        public const float _S = 90f;
        public const float _SW = 135f;
        public const float _W = 180f;
        public const float _NW = 225f;
        public const float _N = 270f;
        public const float _NE = 315f;
        public const float _E = 0f;
        public const float _SE = 45f;
        public static float _y, _z;
        public static float[] dirArray = { _E , _SE, _S, _SW, _W, _NW, _N, _NE };
        public static string[] cardArray = { "E" , "SE", "S", "SW", "W", "NW", "N", "NE" };
        [HarmonyPostfix, HarmonyPatch(typeof(GeneralManager), "Start")]
        public static void Start(ref GeneralManager __instance)
        {
            var fpscounter = GameObject.Find("In-Game GUI/Canvas/2-HUD/").GetComponentInChildren<FPSDisplayer>().gameObject;
            var fpsTransform = fpscounter.transform;
            var compassObject = Object.Instantiate(fpscounter, fpsTransform.parent, false);
            compassObject.name = "Compass";
            var compassRect = compassObject.GetComponent<RectTransform>();
            _y = compassRect.localPosition.y;
            _z = compassRect.localPosition.z;
            compassRect.localPosition = new Vector3(0,_y, _z);
            Object.DestroyImmediate(compassRect.GetComponent<FPSDisplayer>());
            compassTexts = new Text[8];
            var TextTemplate = compassRect.GetComponent<Text>();
            for (int i = 0; i < 8; i++)
            {
                compassTexts[i] = Object.Instantiate(compassObject, compassRect.parent, false).GetComponent<Text>();
                compassTexts[i].text = cardArray[i];
            }
            Object.DestroyImmediate(TextTemplate);
            // debugText = TextTemplate;
            // var dRect = debugText.GetComponent<RectTransform>();
            // dRect.localPosition = new Vector3(dRect.localPosition.x, dRect.localPosition.y - 50, dRect.localPosition.z);
            Log("Working");
        }          
        [HarmonyPostfix, HarmonyPatch(typeof(GeneralManager), "Update")]
        public static void Update(ref GeneralManager __instance)
        {
            var angle =  __instance.FpPlayer.m_LookAngles.y;
            var iAngle = angle % 360;
            Log(iAngle.ToString());
            // debugText.text = Mathf.RoundToInt(iAngle).ToString();
            // if (iAngle > 180f) iAngle -= 360f;
            // if (iAngle < -180f) iAngle += 360f;
            // compassText.text = ""; // iAngle.ToString();
            
            // if (iAngle > -22.5f && iAngle <= 22.5f) compassText.text += "E";
            // if (iAngle > 22.5f && iAngle <= 67.5f) compassText.text += "SE";
            // if (iAngle > 67.5f && iAngle <= 112.5f ) compassText.text += "S";
            // if (iAngle > 112.5f && iAngle <= 157.5f) compassText.text += "SW";
            // if (iAngle > 157.5f || iAngle <= -157.5f) compassText.text += "W";
            // if (iAngle > -157.5f && iAngle <= -112.5f) compassText.text += "NW";
            // if (iAngle > -112.5f && iAngle <= -67.5f) compassText.text += "N";
            // if (iAngle > -67.5f && iAngle <= -22.5f) compassText.text += "NE";

            float[] differences = new float[8];
            
            for (int i = 0; i < 8; i++)
            {
                differences[i] = dirArray[i] - iAngle;
                // if (Mathf.Abs(differences[i]) < 22.5 )
                // {
                //     compassTexts[i].color = new Color(1, 1, 1, 1);
                //     // compassTexts[i].text = differences[i].ToString();
                //     compassTexts[i].GetComponent<RectTransform>().localPosition = new Vector3(differences[i], _y, _z);
                // }
                // else if (Mathf.Abs(differences[i] - 360) < 22.5)
                // {
                //     compassTexts[i].color = new Color(0, 1, 1, 1);
                //     // compassTexts[i].text = differences[i].ToString();
                //     compassTexts[i].GetComponent<RectTransform>().localPosition = new Vector3(differences[i]-360, _y, _z);
                // }
                // else
                // {
                    float _x = Mathf.Clamp(differences[i], -90, 90);
                    float alpha = (90f - Mathf.Abs(_x))/50f;
                    alpha = Mathf.Clamp(alpha, 0, 1f);
                    compassTexts[i].color = new Color(1, 1, 1, alpha);
                    compassTexts[i].GetComponent<RectTransform>().localPosition = new Vector3(_x, _y, _z);
                // }
            }
            

        }       

    }
}
