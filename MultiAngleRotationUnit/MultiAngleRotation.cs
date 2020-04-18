using BepInEx;
using BepInEx.Configuration;
using Cinemachine;
using KKAPI;
using System.Collections;
using UnityEngine;

namespace AIMultiAngleRotation
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public partial class AIMultiAngleRotation : BaseUnityPlugin
    {
        //thanks to 雨宮様 http://playclubphotographs.blog.fc2.com/blog-entry-474.html and RM50様 http://rm50.blog.fc2.com/blog-entry-2.html
        public const string GUID = "ore.ai.multianglerotation";
        public const string PluginName = "AI MultiAngleRotation";
        public const string Version = "0.1.0";

        public static ConfigEntry<float> ConfigF6Angle { get; private set; }
        public static ConfigEntry<float> ConfigF7Angle { get; private set; }
        public static ConfigEntry<float> ConfigF8Angle { get; private set; }

        private const float F6Min = 0f;
        private const float F6Max = 360f;
        private const float F6Default = 90f;
        private const float F7Min = 0f;
        private const float F7Max = 180f;
        private const float F7Default = 180f;
        private const float F8Min = 0f;
        private const float F8Max = 360f;
        private const float F8Default = 90f;

        private Vector3 cameraAngle = new Vector3(0f, 0f, 0f);
        private IEnumerator Start()
        {
            yield return new WaitUntil(() =>
            {
                switch (KKAPI.KoikatuAPI.GetCurrentGameMode())
                {
                    case KKAPI.GameMode.Studio:
                        return KKAPI.Studio.StudioAPI.StudioLoaded;
                    default:
                        return false;
                }
            });
            ConfigF6Angle = Config.Bind("Config", "F6 Angle", F6Default, new ConfigDescription("F6 Angle", new AcceptableValueRange<float>(F6Min, F6Max)));
            ConfigF7Angle = Config.Bind("Config", "F7 Angle", F7Default, new ConfigDescription("F7 Angle", new AcceptableValueRange<float>(F7Min, F7Max)));
            ConfigF8Angle = Config.Bind("Config", "F8 Angle", F8Default, new ConfigDescription("F8 Angle", new AcceptableValueRange<float>(F8Min, F8Max)));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                float z = ConfigF6Angle.Value;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    z = 360 - z;
                }

                CameraAngle = new Vector3(CameraAngle.x, 180f, CameraAngle.z + z);
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                float y = ConfigF7Angle.Value;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    y = 180 - y;
                }

                CameraAngle = new Vector3(CameraAngle.x, CameraAngle.y + y, CameraAngle.z);
            }
            else if (Input.GetKeyDown(KeyCode.F8))
            {
                float y = ConfigF8Angle.Value;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    y = 360 - y;
                }

                CameraAngle = new Vector3(CameraAngle.x, CameraAngle.y + y, CameraAngle.z);
            }
        }
        private Studio.CameraControl CameraControl => (Studio.CameraControl)Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

        private Vector3 CameraAngle
        {
            get => cameraAngle;
            set
            {
                if (value != cameraAngle)
                {
                    CameraControl.cameraAngle = value;
                    cameraAngle = value;
                }
            }
        }
    }
}
