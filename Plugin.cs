using System.Net.Mime;
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

        public static Text compassText;
        [HarmonyPostfix, HarmonyPatch(typeof(GeneralManager), "Start")]
        public static void Start(ref GeneralManager __instance)
        {
            var fpscounter = GameObject.Find("In-Game GUI/Canvas/2-HUD/").GetComponentInChildren<FPSDisplayer>().gameObject;
            var fpsTransform = fpscounter.transform;
            var compassObject = Object.Instantiate(fpscounter, fpsTransform.parent, false);
            compassObject.name = "Compass";
            var compassRect = compassObject.GetComponent<RectTransform>();
            compassRect.localPosition = new Vector3(0,compassRect.localPosition.y, compassRect.localPosition.z);
            Object.DestroyImmediate(compassRect.GetComponent<FPSDisplayer>());
            compassText = compassRect.GetComponent<Text>();
            compassText.text = "?";
        }          
        [HarmonyPostfix, HarmonyPatch(typeof(GeneralManager), "Update")]
        public static void Update(ref GeneralManager __instance)
        {
            var angle = 2* __instance.FpPlayer.m_LookAngles.y;
            var iAngle = (Mathf.RoundToInt(angle) % 720)/2f;
            if (iAngle > 180f) iAngle -= 360f;
            if (iAngle < -180f) iAngle += 360f;
            compassText.text = ""; // iAngle.ToString();
            
            if (iAngle > -22.5f && iAngle <= 22.5f) compassText.text += "E";
            if (iAngle > 22.5f && iAngle <= 67.5f) compassText.text += "SE";
            if (iAngle > 67.5f && iAngle <= 112.5f ) compassText.text += "S";
            if (iAngle > 112.5f && iAngle <= 157.5f) compassText.text += "SW";
            if (iAngle > 157.5f || iAngle <= -157.5f) compassText.text += "W";
            if (iAngle > -157.5f && iAngle <= -112.5f) compassText.text += "NW";
            if (iAngle > -112.5f && iAngle <= -67.5f) compassText.text += "N";
            if (iAngle > -67.5f && iAngle <= -22.5f) compassText.text += "NE";

        }       

    }
}
