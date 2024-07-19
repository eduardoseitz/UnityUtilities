using UnityEngine;

namespace DevPenguin.Utilities
{
    public class SetTargetFramerate : MonoBehaviour
    {
        [SerializeField] int targetFrame = 60;

        void Awake()
        {
            Application.targetFrameRate = targetFrame;
        }
    }
}
