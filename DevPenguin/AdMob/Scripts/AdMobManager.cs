using GoogleMobileAds.Api;
using UnityEngine;

namespace DevPenguin.Ads
{
    /* SDK documentation: https://developers.google.com/admob/unity/quick-start */
    /* Test ad units: https://developers.google.com/admob/android/test-ads */
    public class AdMobManager : MonoBehaviour
    {
        #region Declarations
        public static AdMobManager instance;

        /* Test app id ca-app-pub-3940256099942544~3347511713 */
        /* Test interstitial ca-app-pub-3940256099942544/1033173712 */
        [SerializeField] private string interstitialUnitId = "ca-app-pub-3940256099942544/1033173712";
        /* Test rewarded ca-app-pub-3940256099942544/5224354917 */
        [SerializeField] private string rewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
        [SerializeField] private bool shouldRewardOnAdClosed = false;

        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;
        #endregion

        #region Getters
        public bool IsReady { get; private set; }
        public bool IsInterstitialAdReady { get; private set; }
        public bool IsRewardedAdReady { get; private set; }
        public bool IsShowingRewardedAd { get; private set; }

        public bool AreAdsRemoved { get; private set; }
        #endregion

        #region Events
        public delegate void ReceiveReward();
        public event ReceiveReward OnReceiveReward;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Singleton setup
            if (instance)
                Destroy(gameObject);
            else
                instance = this;

            // Make this scene persistent
            DontDestroyOnLoad(this);
        }

        // Start is called before the first frame update
        private void Start()
        {
            // Initialize google's admob connection
            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("Admob initialized sucessfully.");

                IsReady = true;

                if (interstitialUnitId != string.Empty)
                    RequestInterstititalAd();

                if (rewardedUnitId != string.Empty)
                    RequestRewardedAd();
            });

            // TODO: Request and show banner ad
        }
        #endregion

        #region Helper Methods
        public void RemoveAds()
        {
            AreAdsRemoved = true;
            IsReady = false;
        }

        #region Interstitial Ad Methods
        // A insterstitial ad ussually takes 50s to show
        public void RequestInterstititalAd()
        {
            // Uncoment to debug
            //Debug.Log("Requesting interstitial ad.");

            // Initialize an interstitial add
            interstitialAd = new InterstitialAd(interstitialUnitId);

            // Create an empty ad request
            AdRequest adRequest = new AdRequest.Builder().Build();

            // Called when an ad request has successfully loaded.
            interstitialAd.OnAdLoaded += InterstitialAd_OnAdLoaded;

            // Load the interstitial with the request
            interstitialAd.LoadAd(adRequest);
        }

        public void ShowInterstitialAd()
        {
            if (interstitialAd.IsLoaded())
            {
                interstitialAd.Show();
                IsInterstitialAdReady = false;

                RequestInterstititalAd();
            }
            else
            {
                Debug.LogError("Interstitial ad not loaded.");
            }
        }

        private void InterstitialAd_OnAdLoaded(object sender, System.EventArgs e)
        {
            IsInterstitialAdReady = true;

            // Uncoment to debug
            Debug.Log("Interstitial ad loaded.");
        }
        #endregion

        #region Rewarded Ad Methods
        // A rewarded ad ussually takes 60s to show
        public void RequestRewardedAd()
        {
            // Uncoment to debug
            //Debug.Log("Requesting rewarded add.");

            try
            {
                // Initialize an interstitial add
                rewardedAd = new RewardedAd(interstitialUnitId);

                // Create an empty ad request
                AdRequest adRequest = new AdRequest.Builder().Build();

                // Called when an ad request has successfully loaded
                rewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded;

                // Called when an ad request has been closed
                rewardedAd.OnAdClosed += RewardedAd_OnAdClosed;

                // Called when the user should be rewarded for interacting with the ad
                rewardedAd.OnAdDidRecordImpression += RewardedAd_OnAdDidRecordImpression;
                rewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward;

                // Load the interstitial with the request
                rewardedAd.LoadAd(adRequest);
            }
            catch
            {
                Debug.Log("Error loading rewarded.");
            }
        }

        private void RewardedAd_OnUserEarnedReward(object sender, Reward e)
        {
            // Uncoment to debug
            Debug.Log("Player rewarded.");

            // Trigger reward event
            IsShowingRewardedAd = false;
            OnReceiveReward();
        }

        private void RewardedAd_OnAdDidRecordImpression(object sender, System.EventArgs e)
        {
            // Uncoment to debug
            Debug.Log("Player rewarded after impression.");

            // Trigger reward event
            IsShowingRewardedAd = false;
            if (OnReceiveReward != null)
                OnReceiveReward();
            else
                Debug.Log("Empty OnReceiveReward call");
        }

        private void RewardedAd_OnAdClosed(object sender, System.EventArgs e)
        {
            // Uncoment to debug
            Debug.Log("Player closed rewarded.");

            // Trigger reward event
            if (shouldRewardOnAdClosed)
                OnReceiveReward();

            IsShowingRewardedAd = false;
            RequestRewardedAd();
        }

        public void ShowRewardedAd()
        {
            if (rewardedAd.IsLoaded())
            {
                rewardedAd.Show();
                IsRewardedAdReady = false;
                IsShowingRewardedAd = true;
            }
            else
            {
                Debug.LogError("Rewarded ad not loaded.");
            }
        }

        private void RewardedAd_OnAdLoaded(object sender, System.EventArgs e)
        {
            // Uncoment to debug
            Debug.Log("Rewarded ad loaded.");

            IsRewardedAdReady = true;
        }
        #endregion

        #endregion
    }
}
