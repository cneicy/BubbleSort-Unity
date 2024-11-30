using UnityEngine;

namespace UI
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] private GameObject timeText;

        private void OnEnable()
        {
            timeText.SetActive(false);
        }

        private void OnDisable()
        {
            timeText.SetActive(true);
        }
    }
}
