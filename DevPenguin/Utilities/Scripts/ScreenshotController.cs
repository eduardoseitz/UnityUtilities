using UnityEngine;

public class ScreenshotController : MonoBehaviour
{
    public string filepath;
    public string filename = "Screenshot";
    public int endIndex = 1;
    public int scaling = 1;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            string screenshotName = "MiniRacers" + filename + endIndex + ".png";

            Cursor.visible = false;
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\MiniRacers\\", screenshotName), scaling);
            Cursor.visible = true;

            string savedPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)) + "\\MiniRacers\\" + screenshotName;

            Debug.Log("Screenshot saved to " + savedPath);

            endIndex++;
        }
    }
}
