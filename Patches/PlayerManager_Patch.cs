using Controllers;
using HarmonyLib;
using Kitchen;
using System.Reflection;

namespace KitchenRestoreFranchise.Patches
{
    [HarmonyPatch]
    static class PlayerManager_Patch
    {
        static PropertyInfo p_PopupUtilities = typeof(PlayerManager).GetProperty("PopupUtilities", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(PlayerManager), "HandleRequest")]
        [HarmonyPrefix]
        static void PerformAction_Prefix(PlayerManager __instance, GameStateRequest request)
        {
            if (request == Main.GAME_STATE_REQUEST_RESTORE_FRANCHISE)
            {
                PopupUtilities popupUtilities = (PopupUtilities)p_PopupUtilities?.GetValue(__instance);
                if (popupUtilities != null)
                {
                    popupUtilities.RequestManagedPopup(Main.POPUP_TYPE_RESTORE_FRANCHISE);
                }
            }
        }
    }
}
