using UnityEngine;

namespace DevPenguin.Notifications
{
    public class OneSignalManager : MonoBehaviour
    {
        public static OneSignalManager instance;

        [SerializeField] private string appId = "";
        [SerializeField] private bool requiresUserPrivacyConsent = true;

        public string ID { get; private set; }
        public string Token { get; private set; }

        #region Start Methods
        private void Awake()
        {
            // Singleton setup
            if (instance)
                Destroy(gameObject);
            else
                instance = this;

            DontDestroyOnLoad(this);
        }

        public void Start()
        {
            // Uncomment this method to enable OneSignal Debugging log output 
            OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.NONE);

            // Replace 'YOUR_ONESIGNAL_APP_ID' with your OneSignal App ID.
            OneSignal.StartInit(appId)
              .EndInit();

            OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;

            var pushState = OneSignal.GetPermissionSubscriptionState();

            // The promptForPushNotifications function code will show the iOS push notification prompt. We recommend removing the following code and instead using an In-App Message to prompt for notification permission.
            OneSignal.PromptForPushNotificationsWithUserResponse(OneSignal_promptForPushNotificationsResponse);

            void OneSignal_promptForPushNotificationsResponse(bool accepted)
            {
                Debug.Log("OneSignal_promptForPushNotificationsResponse: " + accepted);
            }

            // Fetch notification credentials
            ID = "0";
            Token = "0";
            OneSignal.IdsAvailable((userId, pushToken) =>
            {
                ID = userId;
                Token = pushToken;
            });

            Debug.Log("OneSignal manager initialized");
            Debug.Log("OneSignal UserID: " + ID + " PushToken: " + Token);
        }
        #endregion

    }
}
