using UnityEngine;

namespace DevPenguin.Utilities
{
    public class EnableOnDesktopAndMobile : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR || UNITY_WEBGL
            this.gameObject.SetActive(false);
#else
            this.gameObject.SetActive(true);
#endif
        }
    }
}
