public class UserData
{
    public string userName;
    public string partnerName;
    public bool isOnline;
    public bool isHoldingKiss;
    public int kissStreak;
    public string lastTimeKissed;
    public string notificationId;
    public string notificationToken;

    public UserData(string userName, string partnerName, bool isOnline, bool isHoldingKiss, int kissStreak, string lastTimeKissed, string notificationId, string notificationToken)
    {
        this.userName = userName;
        this.partnerName = partnerName;
        this.isOnline = isOnline;
        this.notificationToken = notificationToken;
        this.isHoldingKiss = isHoldingKiss;
        this.kissStreak = kissStreak;
        this.lastTimeKissed = lastTimeKissed;
        this.notificationId = notificationId;
        this.notificationToken = notificationToken;
    }
}
