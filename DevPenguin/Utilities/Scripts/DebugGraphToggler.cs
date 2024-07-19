using DevPenguin.VehicleGameEngine.PreferenceSystem;
using UnityEngine;

namespace DevPenguin.Utilities
{
    [RequireComponent(typeof(DebugGraphToggler))]
    public class DebugGraphToggler : MonoBehaviour
    {
        bool isGraphVisible;
        DebugGraphOverlay graphOverlay;

        // Start is called before the first frame update
        void Start()
        {
            #if !UNITY_EDITOR
                Destroy(this);
            #endif

            graphOverlay = GetComponent<DebugGraphOverlay>();
            graphOverlay.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(PreferenceManager.instance.torqueGraphShortcut))
            {
                graphOverlay.ResetData();

                isGraphVisible = true;
                graphOverlay.enabled = isGraphVisible;
            }
        }
    }
}
