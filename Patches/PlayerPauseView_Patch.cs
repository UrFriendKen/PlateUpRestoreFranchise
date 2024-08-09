using Controllers;
using HarmonyLib;
using Kitchen;
using KitchenData;
using System.Linq;

namespace KitchenRestoreFranchise.Patches
{
    [HarmonyPatch]
    static class PlayerPauseView_Patch
    {
        [HarmonyPatch(typeof(PlayerPauseView), "PerformAction")]
        [HarmonyPrefix]
        static void PerformAction_Prefix(PlayerPauseView __instance, int ___ActivePlayer, MenuAction action)
        {
            if (action.PauseAction == Main.PAUSE_MENU_ACTION_RESTORE_FRANCHISE)
            {
                if (GameInfo.CurrentScene == SceneType.Kitchen && GameInfo.IsPreparationTime && GameInfo.CurrentDay < 3 &&
                    GameInfo.AllCurrentCards.Where(card => card is Unlock unlock && unlock.CardType == CardType.FranchiseTier).Any() &&
                    Session.CurrentGameNetworkMode == GameNetworkMode.Host)
                {
                    InputSourceIdentifier.DefaultInputSource.MakeRequest(___ActivePlayer, Main.GAME_STATE_REQUEST_RESTORE_FRANCHISE);
                }
                __instance.Hide();
            }
        }
    }
}
