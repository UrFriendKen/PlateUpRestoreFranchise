using HarmonyLib;
using Kitchen;
using KitchenData;
using System;
using System.Linq;
using System.Reflection;

namespace KitchenRestoreFranchise.Patches
{
    [HarmonyPatch]
    static class MainMenu_Patch
    {
        [HarmonyPatch(typeof(MainMenu), "Setup")]
        [HarmonyPostfix]
        static void Setup_Postfix(MainMenu __instance)
        {
            Type instanceType = __instance.GetType();
            MethodInfo m_AddButton = instanceType.GetMethod("AddButton", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo m_RequestAction = instanceType.GetMethod("RequestAction", BindingFlags.NonPublic | BindingFlags.Instance);
            try
            {
                Main.LogInfo($"GameInfo.CurrentScene == SceneType.Kitchen = {GameInfo.CurrentScene == SceneType.Kitchen}");
                Main.LogInfo($"GameInfo.IsPreparationTime = {GameInfo.IsPreparationTime}");
                Main.LogInfo($"GameInfo.CurrentDay < 3 = {GameInfo.CurrentDay < 3}");
                Main.LogInfo($"Has franchise card = {GameInfo.AllCurrentCards.Where(card => card is Unlock unlock && unlock.CardType == CardType.FranchiseTier).Any()}");
                Main.LogInfo($"Session.CurrentGameNetworkMode == GameNetworkMode.Host = {Session.CurrentGameNetworkMode == GameNetworkMode.Host}");

                if (GameInfo.CurrentScene == SceneType.Kitchen && GameInfo.IsPreparationTime && GameInfo.CurrentDay < 3 &&
                    GameInfo.AllCurrentCards.Where(card => card is Unlock unlock && unlock.CardType == CardType.FranchiseTier).Any() &&
                    Session.CurrentGameNetworkMode == GameNetworkMode.Host)
                {
                    m_AddButton.Invoke(__instance, new object[] {
                    GameData.Main.GlobalLocalisation[Main.BUTTON_TEXT_RESTORE_FRANCHISE_KEY],
                    delegate (int _) {
                        m_RequestAction.Invoke(__instance, new object[] { new MenuAction(Main.PAUSE_MENU_ACTION_RESTORE_FRANCHISE) });
                    },
                    0,
                    1f,
                    0.2f });
                }
            }
            catch (Exception ex)
            {
                Main.LogError($"{ex.Message}\n{ex.StackTrace})");
            }
        }
    }
}
