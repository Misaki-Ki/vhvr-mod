using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ValheimVRMod.VRCore;
using ValheimVRMod.VRCore.UI;
using Valve.VR;

namespace ValheimVRMod.Utilities {
    public class SettingCallback : MonoBehaviour {

        private static SteamVR_Input_Sources inputHand = SteamVR_Input_Sources.RightHand;
        private static SteamVR_Action_Boolean inputAction = SteamVR_Actions.valheim_Use;
        private static UnityAction<Vector3, Quaternion> action;
        private static Transform target;
        private static Transform sourceHand;
        private static GameObject notification;

        public static bool configRunning;
        
        /**
         * called by ConfigSettings.createTransformButton()
         */
        public static bool LeftWrist(UnityAction<Vector3, Quaternion> pAction) {
            action = pAction;
            return createSettingObj(VHVRConfig.LeftWristPos(), VHVRConfig.LeftWristRot(), "Left Wrist", false);
        }

        /**
         * called by ConfigSettings.createTransformButton()
         */
        public static bool RightWrist(UnityAction<Vector3, Quaternion> pAction) {
            action = pAction;
            return createSettingObj(VHVRConfig.RightWristPos(), VHVRConfig.RightWristRot(), "Right Wrist", true);
        }
        
        private static bool createSettingObj(Vector3 pos, Quaternion rot, string panel, bool isRightWrist) {

            if (configRunning) {
                //TODO show message ?
                return false;
            }

            if (isRightWrist) {
                inputAction = SteamVR_Actions.valheim_UseLeft;
                inputHand = SteamVR_Input_Sources.LeftHand;
                target = VRPlayer.rightHand.transform;
                sourceHand = VRPlayer.leftHand.transform;
            }
            else {
                inputAction = SteamVR_Actions.valheim_Use;
                inputHand = SteamVR_Input_Sources.RightHand;
                target = VRPlayer.leftHand.transform;
                sourceHand = VRPlayer.rightHand.transform;
            }
            
            VHVRConfig.config.SaveOnConfigSet = false;
            var settingObj = new GameObject();
            settingObj.AddComponent<SettingCallback>();
            settingObj.transform.SetParent(target, false);
            settingObj.transform.localPosition = pos;
            settingObj.transform.localRotation = rot;
            configRunning = true;
            showNotification(panel);
            
            return true;
        }

        private static void showNotification(string panel) {
            
            var rect = new Vector2(400, 70);
            notification = new GameObject();
            var image = notification.AddComponent<Image>();
            Color black = Color.black;
            image.color = black;
            image.transform.SetParent(Hud.instance.m_rootObject.transform, false);
            image.rectTransform.sizeDelta = rect;

            var text = new GameObject().AddComponent<Text>();
            text.text = "Configuring " + panel + ". Hold Front Trigger of other Hand to position panel, Press Jump to save.";
            text.fontSize = 20;
            text.color = Color.red;
            text.font =  Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.gameObject.AddComponent<ContentSizeFitter>();
            text.transform.SetParent(notification.transform, false);
            text.rectTransform.sizeDelta = rect;

        }
        
        private void OnRenderObject() {
            
            if (SteamVR_Actions.valheim_Jump.GetState(SteamVR_Input_Sources.Any)) {
                VHVRConfig.config.Save();
                VHVRConfig.config.SaveOnConfigSet = true;
                configRunning = false;
                Destroy(notification);
                Destroy(gameObject);
                return;
            }
            
            if (inputAction.GetStateUp(inputHand)) {
                transform.SetParent(target);
            }

            if (! inputAction.GetState(inputHand)) {
                return;
            }
            
            transform.SetParent(target);
            action(transform.localPosition, transform.localRotation);
            transform.SetParent(sourceHand);
        }
    }
}