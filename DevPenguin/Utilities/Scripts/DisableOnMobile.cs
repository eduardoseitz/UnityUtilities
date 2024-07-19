using UnityEngine;

public class DisableOnMobile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID || UNITY_IOS
                    this.gameObject.SetActive(false);
        #else
                this.gameObject.SetActive(true);
        #endif   
    }
}
