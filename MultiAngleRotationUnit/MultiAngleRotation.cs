using System;
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
        public const string Version = "1.0.1";

        public static ConfigEntry<KeyboardShortcut> ConfigKey1 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKey2 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKey3 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKeyC1 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ConfigKeyC3 { get; private set; }
        public static ConfigEntry<float> RollAngle { get; private set; }
        public static ConfigEntry<float> FrontBackAngle { get; private set; }
        public static ConfigEntry<float> SideAngle { get; private set; }

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

        private Vector3 _cameraAngle = new Vector3(0f, 0f, 0f);
        
        private bool _isLoaded;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() =>
            {
                switch (KoikatuAPI.GetCurrentGameMode())
                {
                    case GameMode.Studio:
                        return KKAPI.Studio.StudioAPI.StudioLoaded;
                    case GameMode.Unknown:
                    case GameMode.Maker:
                    case GameMode.MainGame:
                        return false;
                    default:
                        return false;
                }
            });
            ConfigKey1 = Config.Bind("1. Roll", "Roll (clockwise)", new KeyboardShortcut(F4),
                new ConfigDescription("Keyboard shortcut to roll clockwise (right)"));
            ConfigKey2 = Config.Bind("2. Front/Back", "Front/Back", new KeyboardShortcut(F6),
                new ConfigDescription("Keyboard shortcut to rotate into front/back"));
            ConfigKey3 = Config.Bind("3. Side", "Side (clockwise)", new KeyboardShortcut(F7),
                new ConfigDescription("Keyboard shortcut to rotate to side in clockwise manner"));

            ConfigKeyC1 = Config.Bind("1. Roll", "Roll (counter-clockwise)", new KeyboardShortcut(F4, Ctrl),
                new ConfigDescription("Keyboard shortcut to roll counter-clockwise (left)"));
            ConfigKeyC3 = Config.Bind("3. Side", "Side (counter-clockwise)", new KeyboardShortcut(F7, Ctrl),
                new ConfigDescription("Keyboard shortcut to rotate to side in counter-clockwise manner"));

            RollAngle = Config.Bind("1. Roll", "Roll Angle", Key1Default,
                new ConfigDescription("Roll angle", new AcceptableValueRange<float>(Key1Min, Key1Max)));
            FrontBackAngle = Config.Bind("2. Front/Back", "Front/Back Angle (leave this be)", Key2Default,
                new ConfigDescription("No point adjusting this unless you don't want front/back",
                    new AcceptableValueRange<float>(Key2Min, Key2Max)));
            SideAngle = Config.Bind("3. Side", "Side Angle", Key3Default,
                new ConfigDescription("Angle to rotate to side", new AcceptableValueRange<float>(Key3Min, Key3Max)));

            _isLoaded = true;
        }

        private void Update()
        {
            if (!_isLoaded) return;
            
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
                Yaw180();
            }
            else if (ConfigKey3.Value.IsDown())
            {
                Yaw();
            }
            else if (ConfigKeyC3.Value.IsDown())
            {
                Yaw(false);
            }
        }

        private void Roll(bool clockwise = true)
        {
            float z = RollAngle.Value;
            if (!clockwise)
            {
                z = 360 - z;
            }

            Vector3 angle = new Vector3(CameraAngle.x, CameraAngle.y, CalculateAngle(CameraAngle.z + z));
            CameraAngle = angle;
        }

        private void Yaw180()
        {
            float y = FrontBackAngle.Value;
            Vector3 angle = new Vector3(CameraAngle.x, CalculateAngle(CameraAngle.y + y), CameraAngle.z);
            CameraAngle = angle;
        }

        private void Yaw(bool clockwise = true)
        {
            float y = SideAngle.Value;
            if (clockwise)
            {
                y = 360 - y;
            }

            Vector3 angle = new Vector3(CameraAngle.x, CalculateAngle(CameraAngle.y + y), CameraAngle.z);
            CameraAngle = angle;
        }

        private float CalculateAngle(float angle)
        {
            if (angle < 360f) return angle;
            return angle - 360f;
        }

        private Studio.CameraControl CameraControl =>
            (Studio.CameraControl) Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

        private Vector3 CameraAngle
        {
            get => CameraControl.cameraAngle;
            set
            {
                if (value == _cameraAngle) return;
                CameraControl.cameraAngle = _cameraAngle = value;
            }
        }
    }
}