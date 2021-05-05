using BepInEx;
using BepInEx.Configuration;
using Cinemachine;
using KKAPI;
using System.Collections;
using UnityEngine;

namespace HS2MultiAngleRotation
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    [BepInProcess("StudioNEOV2")]
    public partial class HS2MultiAngleRotation : BaseUnityPlugin
    {
        //thanks to 雨宮様 http://playclubphotographs.blog.fc2.com/blog-entry-474.html and RM50様 http://rm50.blog.fc2.com/blog-entry-2.html
        public const string GUID = "hs2.multianglerotation";
        public const string PluginName = "HS2 MultiAngleRotation";
        public const string Version = "1.0.0";

        public static ConfigEntry<KeyboardShortcut> ConfigKey1 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKey2 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKey3 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKeyC1 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKeyC2 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKeyC3 { get; private set; }
        public static ConfigEntry<float> RollAngle { get; private set; }
        public static ConfigEntry<float> YawAngle { get; private set; }
        public static ConfigEntry<float> PitchAngle { get; private set; }

        private const KeyCode Ctrl = KeyCode.LeftControl;
        private const KeyCode F4 = KeyCode.F4;
        private const KeyCode F6 = KeyCode.F6;
        private const KeyCode F7 = KeyCode.F7;
        private const float Key1Min = 0f;
        private const float Key1Max = 360f;
        private const float Key1Default = 90f;
        private const float Key2Min = 0f;
        private const float Key2Max = 180f;
        private const float Key2Default = 180f;
        private const float Key3Min = 0f;
        private const float Key3Max = 360f;
        private const float Key3Default = 90f;

        private Vector3 cameraAngle = new Vector3(0f, 0f, 0f);
        private IEnumerator Start()
        {
            yield return new WaitUntil(() =>
            {
                switch (KoikatuAPI.GetCurrentGameMode())
                {
                    case GameMode.Studio:
                        return KKAPI.Studio.StudioAPI.StudioLoaded;
                    case GameMode.Unknown: case GameMode.Maker: case GameMode.MainGame:
                        return false;
                    default:
                        return false;
                }
            });
            ConfigKey1 = Config.Bind("Config", "1. Roll (clockwise)", new KeyboardShortcut(F4), new ConfigDescription("Keyboard Shortcut"));
            ConfigKey2 = Config.Bind("Config", "2. Yaw (clockwise)", new KeyboardShortcut(F6), new ConfigDescription("Keyboard Shortcut"));
            ConfigKey3 = Config.Bind("Config", "3. Pitch (clockwise)", new KeyboardShortcut(F7), new ConfigDescription("Keyboard Shortcut"));
            
            ConfigKeyC1 = Config.Bind("Config", "1. Roll (counter-clockwise)", new KeyboardShortcut(F4, Ctrl), new ConfigDescription("Keyboard Shortcut"));
            ConfigKeyC2 = Config.Bind("Config", "2. Yaw (counter-clockwise)", new KeyboardShortcut(F6, Ctrl), new ConfigDescription("Keyboard Shortcut"));
            ConfigKeyC3 = Config.Bind("Config", "3. Pitch (counter-clockwise)", new KeyboardShortcut(F7, Ctrl), new ConfigDescription("Keyboard Shortcut"));
            
            RollAngle = Config.Bind("Config", "1. Roll Angle", Key1Default, new ConfigDescription("Angle", new AcceptableValueRange<float>(Key1Min, Key1Max)));
            YawAngle = Config.Bind("Config", "2. Yaw Angle", Key2Default, new ConfigDescription("Angle", new AcceptableValueRange<float>(Key2Min, Key2Max)));
            PitchAngle = Config.Bind("Config", "3. Pitch Angle", Key3Default, new ConfigDescription("Angle", new AcceptableValueRange<float>(Key3Min, Key3Max)));
        }

        private void Update()
        {
            if (ConfigKey1.Value.IsDown())
            {
                Roll();
            }
            else if (ConfigKeyC1.Value.IsDown())
            {
                Roll(false);
            }
            else if (ConfigKey2.Value.IsDown())
            {
                Yaw();
            }
            else if (ConfigKeyC2.Value.IsDown())
            {
                Yaw(false);
            }
            else if (ConfigKey3.Value.IsDown())
            {
                Pitch();
            }
            else if (ConfigKeyC3.Value.IsDown())
            {
                Pitch(false);
            }
        }

        private void Roll(bool clockwise = true)
        {
            float z = RollAngle.Value;
            if (!clockwise)
            {
                z = 360 - z;
            }
            Vector3 angle = new Vector3(CameraAngle.x, 180f, CameraAngle.z + z);
            CameraAngle = angle;
        }

        private void Yaw(bool clockwise = true)
        {
            float y = YawAngle.Value;
            if (!clockwise)
            {
                y = 180 - y;
            }
            Vector3 angle = new Vector3(CameraAngle.x, CameraAngle.y + y, CameraAngle.z);
            CameraAngle = angle;
        }

        private void Pitch(bool clockwise = true)
        {
            float y = PitchAngle.Value;
            if (!clockwise)
            {
                y = 360 - y;
            }
            Vector3 angle = new Vector3(CameraAngle.x, CameraAngle.y + y, CameraAngle.z);
            CameraAngle = angle;
        }
        
        private Studio.CameraControl CameraControl => (Studio.CameraControl)Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

        private Vector3 CameraAngle
        {
            get => cameraAngle;
            set
            {
                if (value == cameraAngle) return;
                CameraControl.cameraAngle = value;
                cameraAngle = value;
            }
        }
    }
}
