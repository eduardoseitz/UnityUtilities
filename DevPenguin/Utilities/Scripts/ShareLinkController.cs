using UnityEngine;

namespace DevPenguin.Utilities
{
    public class ShareLinkController : MonoBehaviour
    {
        [SerializeField] string message = "Play now";
        [SerializeField] string http = "https";
        [SerializeField] string link = "walrusvsjujitsu.com";
        [SerializeField] string subLink = "walrus";

        public string Message { get => message; set => message = value; }
        public string Http { get => http; set => http = value; }
        public string Link { get => link; set => link = value; }
        public string SubLink { get => subLink; set => subLink = value; }

        public void OpenLink(string platformName)
        {
            // Copy link to the clipboard.
            string _clipboard = $"{Message} {Http}://www.{Link}/{SubLink}";
            CopyToClipboard(_clipboard);

            // Open link.
            switch (platformName)
            {
                // Reference: https://developers.facebook.com/docs/plugins/share-button/
                case "facebook":
                    //#if UNITY_IOS
                    // Application.OpenURL($"https://www.facebook.com/dialog/share?app_id=87741124305&href={Http}%3A%2F%2F{Link}%2F{SubLink}%3Fsi%3Dh0RZRvZdG3yP5LBw&display=popup"); // Didn't work on iOS
                    Application.OpenURL($"https://www.facebook.com/sharer/sharer.php?u={Http}%3A%2F%2F{Link}%2F{SubLink}");
                    //#else
                    //                    Application.OpenURL($"https://www.facebook.com/sharer/sharer.php?u={Http}%3A%2F%2F{Link}%2F{SubLink}&amp;src=sdkpreparse"); // Didn't work on iOS
                    //#endif
                    break;
                case "whatsapp":
//#if UNITY_IOS
                    //Application.OpenURL($"https://api.whatsapp.com/send/?text={Http}%3A%2F%2F{Link}%2F{SubLink}%3Fsi%3D5wlfxPwsxhgejJY9&type=custom_url&app_absent=0"); // Did not work on whatssapp
                    Application.OpenURL($"https://api.whatsapp.com/send?text={Message} {Http}://www.{Link}/{SubLink}");
                    //#else
                    //                    Application.OpenURL($"https://api.whatsapp.com/send/?text={Message} {Http}%3A%2F%2F{Link}%2F{SubLink}&type=custom_url&app_absent=0"); // Didn't work on iOS
                    //#endif
                    break;
                // Reference: https://developer.twitter.com/en/docs/twitter-for-websites/tweet-button/overview
                case "twitter":
                    //#if UNITY_IOS
                    //Application.OpenURL($"https://twitter.com/i/flow/login?redirect_after_login=%2Fintent%2Ftweet%3Furl%3D{Http}%253A%2F%2F{Link}%2F{SubLink}%253Fsi%253DKVg91Oddq3MKkbsI%26text%3D{Message}"); // Didn't work on iOS
                    //Application.OpenURL($"https://twitter.com/intent/tweet?text={Message} {Http}://www.{Link}/{SubLink}"); // Didn't work on iOS
                    //Application.OpenURL($"https://www.linkedin.com/sharing/share-offsite/?mini=true&url={Http}%3A%2F%2F{Link}%2F{SubLink}%2F{Message}"); // NOT TESTED YET
                    Application.OpenURL($"https://twitter.com/intent/tweet?text={Message}&url={Http}%3A%2F%2F{Link}%2F{SubLink}");
                    //#else
                    //                    Application.OpenURL($"https://twitter.com/intent/tweet?text={Message} {Http}://www.{Link}/{SubLink}");
                    //#endif
                    break;
                case "reddit":
//#if UNITY_IOS
                    Application.OpenURL($"https://www.reddit.com/submit?url={Http}%3A//{Link}/{SubLink}%3Fsi%3D7D0LhZVmxF5SieVS&title={Message}"); // Worked on iOS
//#else
//                    Application.OpenURL($"https://www.reddit.com/submit?url={Http}%3A%2F%2F{Link}%2F{SubLink}&title={Message}");
//#endif
                    break;
                case "linkedin":
//#if UNITY_IOS
                    Application.OpenURL($"https://www.linkedin.com/sharing/share-offsite/?url={Http}%3A%2F%2F{Link}%2F{SubLink}&title={Message}&source={Message}");
                          //#else
                          //                    Application.OpenURL($"https://www.linkedin.com/sharing/share-offsite/?url={Http}%3A%2F%2F{Link}%2F{SubLink}");
                          //#endif
                    break;
                case "threads":
                    Application.OpenURL($"https://www.threads.net/");
                    break;
                case "tiktok":
                    Application.OpenURL($"https://www.tiktok.com/");
                    break;
                case "instagram":
                    Application.OpenURL($"https://www.instagram.com/");
                    break;
                case "clipboard":
                    break;
                default:
                    Debug.LogError($"{platformName} platform not implemented!");
                    break;
            }
        }
        
        public void CopyToClipboard(string text) 
        {
            //#if UNITY_IOS
            //GUIUtility.systemCopyBuffer = text; // This did not work on mobile.
            //UniClipboard.SetText(text); // This does not work on WebGL
            //System.Windows.Forms.Clipboard.SetText(text); //This did not work with unity.

#if UNITY_WEBGL && !UNITY_EDITOR
            GameObject.Find("Bridge").GetComponent<Bridge>().CopyToClipboard(text);
#else
            // This did not work on iOS.
            TextEditor _textEditor = new TextEditor();
            _textEditor.text = text;
            _textEditor.SelectAll();
            _textEditor.Copy();
#endif

            Debug.Log($"Copied {text} to clipboard.");
        }
    }
}
