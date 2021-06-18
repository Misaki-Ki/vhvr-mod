using System.Reflection;
using HarmonyLib;
using ValheimVRMod.Scripts;
using ValheimVRMod.Utilities;
using ValheimVRMod.VRCore;

namespace ValheimVRMod.Patches
{
    
    [HarmonyPatch(typeof(Humanoid), "HideHandItems")]
    class PatchHideHandItems
    {
        
        private static MethodInfo setupVisEquipmentMethod = AccessTools.Method(typeof(Humanoid), "SetupVisEquipment");
        
        static bool Prefix(ref Humanoid __instance,
            ref ItemDrop.ItemData ___m_leftItem, ref ItemDrop.ItemData ___m_rightItem, 
            ref ItemDrop.ItemData ___m_hiddenLeftItem, ref ItemDrop.ItemData ___m_hiddenRightItem, 
            ref VisEquipment ___m_visEquipment, ref ZSyncAnimation ___m_zanim) {

            if (__instance != Player.m_localPlayer || !VHVRConfig.UseVrControls()) {
                return true;
            }
            
            bool leftHand = VRPlayer.toggleShowLeftHand;
            bool rightHand = VRPlayer.toggleShowRightHand;
            
            VRPlayer.toggleShowRightHand = true;
            VRPlayer.toggleShowLeftHand = true;
    
            if (___m_leftItem == null && ___m_rightItem == null) {
                return false;   
            }
    
            if (leftHand) {
                ItemDrop.ItemData leftItem = ___m_leftItem;
                __instance.UnequipItem(___m_leftItem);
                ___m_hiddenLeftItem = leftItem;
            }
    
            if (rightHand) {
                ItemDrop.ItemData rightItem = ___m_rightItem;
                __instance.UnequipItem(___m_rightItem);
                ___m_hiddenRightItem = rightItem;                
            }
            
            setupVisEquipmentMethod.Invoke(__instance, new object[]{___m_visEquipment, false});
            ___m_zanim.SetTrigger("equip_hip");
    
            return false;
        }
    }
    
     
    [HarmonyPatch(typeof(Humanoid), "ShowHandItems")]
    class PatchShowHandItems
    {
    
        static bool Prefix(ref Humanoid __instance,
            ref ItemDrop.ItemData ___m_hiddenLeftItem, ref ItemDrop.ItemData ___m_hiddenRightItem,
            ref ZSyncAnimation ___m_zanim)
        {
            
            if (__instance != Player.m_localPlayer || !VHVRConfig.UseVrControls()) {
                return true;
            }
    
            bool leftHand = VRPlayer.toggleShowLeftHand;
            bool rightHand = VRPlayer.toggleShowRightHand;
            
            VRPlayer.toggleShowRightHand = true;
            VRPlayer.toggleShowLeftHand = true;
            
            if (___m_hiddenLeftItem == null && ___m_hiddenRightItem == null) {
                return false;   
            }
    
            if (leftHand) {
                ItemDrop.ItemData hiddenLeftItem =___m_hiddenLeftItem;
                ___m_hiddenLeftItem = null;
                if (hiddenLeftItem != null) {
                    __instance.EquipItem(hiddenLeftItem);
                }
            }
    
            if (rightHand)
            {
                ItemDrop.ItemData hiddenRightItem = ___m_hiddenRightItem;
                ___m_hiddenRightItem = null;
                if (hiddenRightItem != null) {
                    __instance.EquipItem(hiddenRightItem);    
                }
            }
            
            ___m_zanim.SetTrigger("equip_hip");
            
            return false;
        }
    }

    [HarmonyPatch(typeof(Inventory), "Changed")]
    class PatchInventoryChanged {

        static void Postfix() {
            if (StaticObjects.quickSwitch != null && VHVRConfig.UseVrControls()) {
                QuickSwitch.refreshItems();
            }
        }
    }
    
    [HarmonyPatch(typeof(InventoryGui), "OnSelectedItem")]
    class PatchOnSelectedItem {

        static void Postfix() {
            if (StaticObjects.quickSwitch != null && VHVRConfig.UseVrControls()) {
                QuickSwitch.refreshItems();
            }
        }
    }

    [HarmonyPatch(typeof(Humanoid), "UnequipItem")]
    class PatchUnEquipItem {

        static void Postfix(Humanoid __instance) {

            if (__instance != Player.m_localPlayer || !VHVRConfig.UseVrControls()) {
                return;
            }
            
            if (StaticObjects.quickSwitch != null) {
                QuickSwitch.refreshItems();
            }
        }
    }
        
    [HarmonyPatch(typeof(Player), "SetGuardianPower")]
    class PatchSetGuardianPower {

        static void Postfix(Humanoid __instance) {

            if (__instance != Player.m_localPlayer || !VHVRConfig.UseVrControls()) {
                return;
            }
            
            if (StaticObjects.quickActions != null) {
                QuickActions.refreshItems();
            }
        }
    }
}