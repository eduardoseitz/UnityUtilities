using UnityEngine;

namespace DevPenguin.Utilities
{
    public class SlowMotionController : MonoBehaviour
    {
        [Tooltip("Time scale when you hit play.")]
        [SerializeField] float startTimeSpeed = 1;

        // Start is called before the first frame update
        void Start()
        {
            // Set start time speed
            Time.timeScale = startTimeSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            // Update time speed on runtime
            if (Input.GetKeyDown(KeyCode.KeypadPlus) && Time.timeScale <= 1.9f)
                Time.timeScale += 0.1f;
            else if (Input.GetKeyDown(KeyCode.KeypadMinus) && Time.timeScale >= 0.1f)
                Time.timeScale -= 0.1f;
            else if (Input.GetKeyDown(KeyCode.Keypad0) && Time.timeScale != 0)
                Time.timeScale = 0;
            else if (Input.GetKeyDown(KeyCode.Keypad0) && Time.timeScale == 0)
                Time.timeScale = 1;

        }
    }
}
