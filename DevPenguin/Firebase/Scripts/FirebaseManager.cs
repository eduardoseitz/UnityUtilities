using UnityEngine;
using Firebase.Database;
using System;
using System.Globalization;

public class FirebaseManager : MonoBehaviour
{
    #region Declarations
    public static FirebaseManager instance;

    public bool IsReady { get; private set; }

    public UserData CurrentUser { get; private set; }
    public UserData PartnerUser { get; private set; }

    private Firebase.FirebaseApp _app;
    private DatabaseReference _reference;
    private DataSnapshot _snapshot;
    private bool _fetchingForTheFistTime = true;
    private int _kissStreak;
    private string _lastTimeKissed;
    private DateTime _formatedLastTimeKissed;
    #endregion

    #region Main Methods
    private void Awake()
    {
        // Singleton setup
        if (instance)
            Destroy(this);
        else
            instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Reset instances
        CurrentUser = null;
        PartnerUser = null;

        // Check firebase dependencies
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    #endregion

    #region Helper Methods

    // Write or update new user sign in data
    public void WriteUserDataToDatabase(string userName, string partnerName, bool isOnline)
    {
        // Fetch data that needs to be reused
        _kissStreak = (FetchFieldDataFromDatabase(userName, "kissStreak") == null) ? 0 : int.Parse(FetchFieldDataFromDatabase(userName, "kissStreak"));
        _lastTimeKissed = (FetchFieldDataFromDatabase(userName, "lastTimeKissed") == null) ? DateTime.MinValue.ToString(CultureInfo.InvariantCulture) : FetchFieldDataFromDatabase(userName, "lastTimeKissed");
        _formatedLastTimeKissed = DateTime.Parse(_lastTimeKissed, CultureInfo.InvariantCulture);
        //if user hasn't kissed yesterday
        if (_formatedLastTimeKissed.AddDays(1).Day < DateTime.Now.Day)
        {
            // Reset streak
            _kissStreak = 0;
        }

        // Construct data to be added
        CurrentUser = new UserData(userName, partnerName, isOnline, false, _kissStreak, _lastTimeKissed, OneSignalManager.instance.ID, OneSignalManager.instance.Token);

        // Convert data class into json
        string jsonData = JsonUtility.ToJson(CurrentUser);

        // Modify database stack
        _reference.Child("users").Child(userName).SetRawJsonValueAsync(jsonData);
        
        // Push modified data
        _reference.Push();

        Debug.Log("Saved to database sucessfully.");
    }

    public void UpdateKissStreak()
    {
        // Format date string to a date format
        _formatedLastTimeKissed = DateTime.Parse(FetchFieldDataFromDatabase(CurrentUser.userName, "lastTimeKissed"), CultureInfo.InvariantCulture);
        _kissStreak = int.Parse(FetchFieldDataFromDatabase(CurrentUser.userName, "kissStreak"));

        // if user hasn't kissed yesterday
        if (_formatedLastTimeKissed.AddDays(1).Day < DateTime.Now.Day)
        {
            // Reset streak
            _kissStreak = 1;
        }
        // if user has kissed yesterday
        else if (_formatedLastTimeKissed.AddDays(1).Day == DateTime.Now.Day)
        {
            // Increase streak
            _kissStreak++;
        }

        _lastTimeKissed = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        WriteFieldDataToDatabase("kissStreak", _kissStreak.ToString());
        WriteFieldDataToDatabase("lastTimeKissed", _lastTimeKissed);
    }

    public void UpdateUserOnlineStatusToDatabase(bool isOnline)
    {
        if (CurrentUser != null)
        {
            WriteFieldDataToDatabase("isOnline", isOnline.ToString());
        }
    }

    public void WriteFieldDataToDatabase(string field, string value)
    {
        // Update database field
        _reference.Child("users").Child(CurrentUser.userName).Child(field).SetValueAsync(value);
    }

    public string FetchFieldDataFromDatabase(string userName, string field)
    {
        string _fetchedData = null;

        // Find user in latest snapshot
        if (_snapshot != null)
        {
            foreach (DataSnapshot data in _snapshot.Children)
            {
                // Check for user
                if (data.Child("userName").Value.ToString().Equals(userName))
                {
                    // Get user info
                    _fetchedData = data.Child(field).Value.ToString();

                    //Debug.Log("Found user in the database.");
                }
            }
        }

        return _fetchedData;
    }

    private void InitializeFirebase()
    {
        // Create and hold a reference to your FirebaseApp,
        // where app is a Firebase.FirebaseApp property of your application class.
        _app = Firebase.FirebaseApp.DefaultInstance;

        // Set a flag here to indicate whether Firebase is ready to use by your app
        Debug.Log("Connected to firebase.");

        // Get the root reference location of the database.
        _reference = FirebaseDatabase.DefaultInstance.RootReference;

        // Subscribe for database changes
        FirebaseDatabase.DefaultInstance.GetReference("users").ValueChanged += HandleValueChanged;

        // Set manager as ready for other scripts knowledge
        IsReady = true;

        Debug.Log("Firebase Manager Initialized");
    }

    // This is trigged when new data is available in the database
    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        // Check for erros
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // Get new firebase values
        Firebase.Database.FirebaseDatabase _dbInstance = Firebase.Database.FirebaseDatabase.DefaultInstance;
        _dbInstance.GetReference("users").GetValueAsync().ContinueWith
        (
            task => 
            {
                if (task.IsFaulted)
                {
                    // Handle error
                    Debug.LogError("Error retrieving data.");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Retrieved data sucessfully.");

                    // Get data changes
                    _snapshot = task.Result;

                    // Find partner in latest snapshot
                    foreach (DataSnapshot user in _snapshot.Children)
                    {
                        // Check for partner
                        if (user.Child("userName").Value.ToString().Equals(CurrentUser.partnerName))
                        {
                            // Get oartner info
                            PartnerUser = new UserData
                            (
                                user.Child("userName").Value.ToString(),
                                user.Child("partnerName").Value.ToString(),
                                bool.Parse(user.Child("isOnline").Value.ToString()),
                                bool.Parse(user.Child("isHoldingKiss").Value.ToString()),
                                int.Parse(user.Child("kissStreak").Value.ToString()),
                                user.Child("lastTimeKissed").Value.ToString(),
                                user.Child("notificationId").Value.ToString(),
                                user.Child("notificationToken").Value.ToString()
                            );

                            Debug.Log("Found the partner in the database.");
                            Debug.Log("Partner is " + ((PartnerUser.isOnline) ? "online" : "ofline"));
                        }
                    }
                }
            }   
        );

        Debug.Log("New userdata fetched from database.");
    }
    #endregion
}
