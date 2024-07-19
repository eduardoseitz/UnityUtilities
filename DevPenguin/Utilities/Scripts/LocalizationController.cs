using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace DevPenguin.Utilities
{
    public class LocalizationController : MonoBehaviour
    {
        private const string TAG = "LocalizationController";
        
        [Header("Debug")]
        [SerializeField] private bool areLogsEnabled;
        
        private int _languageIndex;

        #region Unity Methods
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (!LocalizationSettings.InitializeSynchronously)
            {
                LocalizationSettings.InitializationOperation.WaitForCompletion();
            }

        }
        
        #endregion

        #region Helper Methods
        
        public int SelectedLanguageIndex
        {
            get
            {
                if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale("pt-BR"))
                    _languageIndex = 1;
                else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale("es-MX"))
                    _languageIndex = 2;
                else
                    _languageIndex = 0;

                return _languageIndex;
            }
        }

        public void SwitchLocalizationLanguage(int languageIndex)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
                
            if (areLogsEnabled)
                Debug.Log($"{TAG}: Switched language to {LocalizationSettings.SelectedLocale} successfully.");
        }

        public static string GetLocalizedString(string localizationTable, string localizationKey)
        {
            try
            {
                return LocalizationSettings.StringDatabase.GetLocalizedString(localizationTable, localizationKey);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                
                return localizationKey;
            }
        }
        
        #endregion
        
    }
}
