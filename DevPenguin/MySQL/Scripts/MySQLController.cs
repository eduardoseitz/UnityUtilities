using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using DevPenguin.Utilities;
using System.Net;

namespace DevPenguin.Utilities
{
    public static class MySQLController
    {
        static bool HasErrorMessage(string message) => int.TryParse(message, out var error);

        public static async Task<bool> WriteIntoSQLDatabase(string url, Dictionary<string, string> data)
        {
            return (await SendPostRequest(url, data)).sucess;
        }

        static async Task<(bool sucess, string result)> SendPostRequest(string url, Dictionary<string, string> data)
        {
            using(UnityWebRequest _webRequest = UnityWebRequest.Post(url, data))
            {
                _webRequest.SendWebRequest();

                while (!_webRequest.isDone) await Task.Delay(100);

                // When the task is done check for errors.
                if (_webRequest.error != null 
                    || !string.IsNullOrWhiteSpace(_webRequest.error)
                    || HasErrorMessage(_webRequest.downloadHandler.text))
                    return (false, _webRequest.downloadHandler.text);

                // Sucess.
                return (true, _webRequest.downloadHandler.text);
            }
        }
    }

    public class Data
    {
        public string id;
        public string DidWalrusWin;
        public string DidJujitsuWin;
    }
}
