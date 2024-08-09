using Controllers;
using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenMods;
using KitchenRestoreFranchise.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenRestoreFranchise
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "Restore Franchise";
        public const string MOD_VERSION = "0.1.2";

        private static readonly int RESTORE_FRANCHISE_ID = HashUtils.GetID($"{MOD_GUID}:restoreFranchise");
        internal static readonly GameStateRequest GAME_STATE_REQUEST_RESTORE_FRANCHISE = (GameStateRequest)RESTORE_FRANCHISE_ID;
        internal static readonly PopupType POPUP_TYPE_RESTORE_FRANCHISE = (PopupType)RESTORE_FRANCHISE_ID;
        internal static readonly PauseMenuAction PAUSE_MENU_ACTION_RESTORE_FRANCHISE = (PauseMenuAction)RESTORE_FRANCHISE_ID;

        private readonly Dictionary<Locale, PopupDetails> RestoreFranchisePopupDetails = new Dictionary<Locale, PopupDetails>()
        {
            { Locale.English, new PopupDetails() { Title = "Restore Franchise", Description = "Are you sure you want to convert this restaurant back into a franchise?" } },
            { Locale.BlankText, new PopupDetails() { Title = "XXX", Description = "XXX" } },
            { Locale.French, new PopupDetails() { Title = "Restaurer la Franchise", Description = "Êtes-vous sûr de vouloir reconvertir ce restaurant en franchise?" } },
            { Locale.German, new PopupDetails() { Title = "Franchise Wiederherstellen", Description = "Sind Sie sicher, dass Sie dieses Restaurant wieder in ein Franchise-Restaurant umwandeln möchten?" } },
            { Locale.Spanish, new PopupDetails() { Title = "Restaurar Franquicia", Description = "Estás seguro de que quieres volver a convertir este restaurante en una franquicia?" } },
            { Locale.Polish, new PopupDetails() { Title = "Przywróć franczyzę", Description = "Czy na pewno chcesz ponownie przekształcić tę restaurację w franczyzę?" } },
            { Locale.Russian, new PopupDetails() { Title = "Восстановить франшизу", Description = "Вы уверены, что хотите снова превратить этот ресторан во франшизу?" } },
            { Locale.PortugueseBrazil, new PopupDetails() { Title = "Restaurar Franquia", Description = "Tem certeza de que deseja converter este restaurante novamente em uma franquia?" } },
            { Locale.Japanese, new PopupDetails() { Title = "復元フランチャイズ", Description = "このレストランをフランチャイズに戻してもよろしいですか?" } },
            { Locale.ChineseSimplified, new PopupDetails() { Title = "恢复特许经营权", Description = "您确定要将这家餐厅重新转变为特许经营餐厅吗？" } },
            { Locale.ChineseTraditional, new PopupDetails() { Title = "恢復特許經營權", Description = "您確定要將這家餐廳重新轉變為特許經營餐廳嗎？" } },
            { Locale.Korean, new PopupDetails() { Title = "프랜차이즈 복원", Description = "이 레스토랑을 다시 프랜차이즈로 전환하시겠습니까?" } },
            { Locale.Turkish, new PopupDetails() { Title = "Franchise'ı Geri Yükle", Description = "Bu restoranı tekrar franchise'a dönüştürmek istediğinizden emin misiniz?" } }
        };

        internal const string BUTTON_TEXT_RESTORE_FRANCHISE_KEY = "MENU_RESTORE_FRANCHISE";
        private readonly Dictionary<Locale, string> RestoreFranchiseButtonText = new Dictionary<Locale, string>()
        {
            { Locale.English, "Restore Franchise" },
            { Locale.BlankText, "Restore Franchise" },
            { Locale.French, "Restaurer la Franchise" },
            { Locale.German, "Franchise Wiederherstellen" },
            { Locale.Spanish, "Restaurar Franquicia" },
            { Locale.Polish, "Przywróć franczyzę" },
            { Locale.Russian, "Восстановить франшизу" },
            { Locale.PortugueseBrazil, "Restaurar Franquia" },
            { Locale.Japanese, "復元フランチャイズ" },
            { Locale.ChineseSimplified, "恢复特许经营权" },
            { Locale.ChineseTraditional, "恢復特許經營權" },
            { Locale.Korean, "프랜차이즈 복원" },
            { Locale.Turkish, "Franchise'ı Geri Yükle" },
        };

        public Main()
        {
            Harmony harmony = new Harmony(MOD_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        public void PreInject()
        {
            bool shouldRelocalise = false;

            LocalisationObject<DictionaryInfo> globalLocalisationInfo = GameData.Main?.GlobalLocalisation?.Info;
            if (globalLocalisationInfo != null)
            {
                foreach (Locale locale in globalLocalisationInfo.GetLocales())
                {
                    DictionaryInfo dictionaryInfo = globalLocalisationInfo.Get(locale);
                    if (dictionaryInfo?.Text == null || dictionaryInfo.Text.ContainsKey(BUTTON_TEXT_RESTORE_FRANCHISE_KEY))
                        continue;
                    if (!RestoreFranchiseButtonText.TryGetValue(locale, out string buttonText))
                        continue;
                    dictionaryInfo.Text[BUTTON_TEXT_RESTORE_FRANCHISE_KEY] = buttonText;
                    globalLocalisationInfo.Add(locale, dictionaryInfo);
                    shouldRelocalise = true;
                }
            }

            LocalisationObject<PopupText> popupTextLocalisationInfo = GameData.Main?.GlobalLocalisation?.PopupTextLocalisation?.LocalisationInfo;
            if (popupTextLocalisationInfo != null)
            {
                foreach (Locale locale in popupTextLocalisationInfo.GetLocales())
                {
                    PopupText popupText = popupTextLocalisationInfo.Get(locale);
                    if (popupText?.Text == null || popupText.Text.ContainsKey(POPUP_TYPE_RESTORE_FRANCHISE))
                        continue;
                    if (!RestoreFranchisePopupDetails.TryGetValue(locale, out PopupDetails popupDetails))
                        continue;
                    popupText.Text[POPUP_TYPE_RESTORE_FRANCHISE] = popupDetails;
                    popupTextLocalisationInfo.Add(locale, popupText);
                    shouldRelocalise = true;
                }
            }

            if (shouldRelocalise)
                GameData.Main.ReLocalise(Localisation.CurrentLocale);
        }

        public void PostInject()
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
