using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace DevPenguin.Ads
{
    /* SDK documentation: https://developers.google.com/admob/unity/quick-start */
    /* Test ad units: https://developers.google.com/admob/android/test-ads */
    public class AdMobManager : MonoBehaviour
    {
        
        #region Declarations

        private const string TAG = "AdMobManager";
        public static AdMobManager Instance;

        [Header("Debug Settings")]
        [SerializeField] private bool areLogsEnabled;
        /* Test app id ca-app-pub-3940256099942544~3347511713 */
        
        [Header("Admob Settings")]
        [SerializeField] private bool shouldShowAds = true;
        /* Test Interstitial ca-app-pub-3940256099942544/1033173712 */
        [SerializeField] private string interstitialUnitId = "ca-app-pub-3940256099942544/1033173712";
        /* Test rewarded ca-app-pub-3940256099942544/5224354917 */
        [SerializeField] private string rewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
        [SerializeField] private bool shouldRewardOnAdClosedBeforeFinish;
        [Range(1, 600)]
        [SerializeField] private float showRewardAfterSeconds = 300;
        
        // Other private properties.
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;
        private float _rewardStartTime;
        
        #endregion

        #region Getters and setters

        public bool ShouldShowAds => shouldShowAds;
        public bool IsReady { get; private set; }
        public bool IsInterstitialAdReady { get; private set; }
        public bool IsRewardedAdReady { get; private set; }
        public bool IsShowingRewardedAd { get; private set; }
        public bool AreAdsRemoved { get; private set; }
        // TODO:
        public bool IsTimeToShowAds => Time.time >= _rewardStartTime + showRewardAfterSeconds;
        
        #endregion

        #region Events
        
        public delegate void ReceiveReward();
        public event ReceiveReward OnReceiveReward;
        
        #endregion

        #region MonoBehaviour Methods
        
        private void Awake()
        {
            ResetRewardStartTime();
            
            // Singleton pattern.
            if (Instance)
                Destroy(gameObject);
            else
                Instance = this;

            // Make this scene persistent.
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Start()
        {
            // Initialize google's admob connection.
            MobileAds.Initialize(initStatus =>
            {
                IsReady = true;
                
                // Request ads.
                if (interstitialUnitId != string.Empty)
                    RequestInterstitialAd();
                if (rewardedUnitId != string.Empty)
                    RequestRewardedAd();
                    
                // Debug.
                if (areLogsEnabled)
                    Debug.Log($"{TAG}: Admob initialized successfully.");
            });

            // Test rewarded ad.
            //ShowRewardedAd();
            
            // Test Interstitial ad.
            //ShowInterstitialAd();
            
            // TODO: Request and show banner ad
        }
        
        #endregion

        #region Helper Methods
        
        public void RemoveAds()
        {
            AreAdsRemoved = true;
            IsReady = false;
        }
        
        public void ResetRewardStartTime()
        {
            // TODO:
            _rewardStartTime = Time.time;
        }

        #region Interstitial Ad Methods
        
        /// <summary>
        /// An interstitial ad usually takes 50 seconds to load.
        /// </summary>
        public void RequestInterstitialAd()
        {
            // Debug.
            if (areLogsEnabled)
                Debug.Log($"{TAG}: Requesting Interstitial ad.");

            // Initialize an Interstitial add.
            _interstitialAd = new InterstitialAd(interstitialUnitId);

            // Create an empty ad request
            AdRequest adRequest = new AdRequest.Builder().Build();

            // Called when an ad request has successfully loaded.
            _interstitialAd.OnAdLoaded += InterstitialAd_OnAdLoaded;

            // Load the Interstitial with the request.
            _interstitialAd.LoadAd(adRequest);
        }

        public void ShowInterstitialAd()
        {
            if (_interstitialAd.IsLoaded())
            {
                _interstitialAd.Show();
                IsInterstitialAdReady = false;
            }
            else
                Debug.LogError($"{TAG}: Interstitial ad not loaded.");
            RequestInterstitialAd();
        }

        private void InterstitialAd_OnAdLoaded(object sender, System.EventArgs e)
        {
            IsInterstitialAdReady = true;

            // Debug.
            if (areLogsEnabled)
                Debug.Log($"{TAG}: Interstitial ad loaded.");
        }
        
        #endregion

        #region Rewarded Ad Methods
        
        /// <summary>
        /// A rewarded ad usually takes 60s to show.
        /// </summary>
        public void RequestRewardedAd()
        {
            // Debug.
            if (areLogsEnabled)
                Debug.Log($"{TAG}: Requesting rewarded add.");

            try
            {
                // Initialize an Interstitial add
                _rewardedAd = new RewardedAd(interstitialUnitId);

                // Create an empty ad request
                AdRequest adRequest = new AdRequest.Builder().Build();

                // Called when an ad request has successfully loaded
                _rewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded;

                // Called when an ad request has been closed
                _rewardedAd.OnAdClosed += RewardedAd_OnAdClosed;

                // Called when the user should be rewarded for interacting with the ad
                _rewardedAd.OnAdDidRecordImpression += RewardedAd_OnAdDidRecordImpression;
                _rewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward;

                // Load the Interstitial with the request
                _rewardedAd.LoadAd(adRequest);
            }
            catch
            {
                Debug.LogError($"{TAG}: Error loading rewarded ad.");
            }
        }

        private void RewardedAd_OnUserEarnedReward(object sender, Reward e)
        {
            // Debug.
            if (areLogsEnabled)
                Debug.Log($"{TAG}: Player rewarded.");

            // Trigger reward event
            IsShowingRewardedAd = false;
            OnReceiveReward();
        }

        private void RewardedAd_OnAdDidRecordImpression(object sender, System.EventArgs e)
        {
            // Debug.
            if (areLogsEnabled)
                Debug.Log($"{TAG}: Player rewarded after impression.");

            // Trigger reward event
            IsShowingRewardedAd = false;
            if (OnReceiveReward != null)
                OnReceiveReward();
            else
                Debug.LogError($"{TAG}: Empty OnReceiveReward call");
        }

        private void RewardedAd_OnAdClosed(object sender, System.EventArgs e)
        {
            // Debug.
            if (areLogsEnabled)
                Debug.Log($"{TAG}: Player closed rewarded.");

            // Trigger reward event
            if (shouldRewardOnAdClosedBeforeFinish)
                OnReceiveReward();
            IsShowingRewardedAd = false;
            RequestRewardedAd();
        }

        public void ShowRewardedAd()
        {
            if (_rewardedAd.IsLoaded())
            {
                _rewardedAd.Show();
                IsRewardedAdReady = false;
                IsShowingRewardedAd = true;
                ResetRewardStartTime();
            }
            else
                Debug.LogError($"{TAG}: Rewarded ad not loaded.");
        }

        private void RewardedAd_OnAdLoaded(object sender, System.EventArgs e)
        {
            IsRewardedAdReady = true;
            
            // Debug.
            if (areLogsEnabled)
                Debug.Log($"{TAG}: Rewarded ad loaded.");
        }
        
        #endregion

        #endregion
        
    }
}
