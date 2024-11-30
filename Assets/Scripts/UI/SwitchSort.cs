using GamePlay.Camera;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class SwitchSort : MonoBehaviour
    {
        [SerializeField] private InsertionSortCore inspectorSort;
        [SerializeField] private BubbleSortCore bubbleSortCore;
        [SerializeField] private QuickSortCore quickSortCore;
        public int flag;
        public TMP_Text text;
        public InputAction input;
        public Button reGen;

        private void Start()
        {
            Switch();
            input = InputSystem.actions.FindAction("Interact");
            input.started += Click;
            reGen.onClick.AddListener(ReGen);
        }

        private void ReGen()
        {
            switch (flag)
            {
                case 0:
                    bubbleSortCore.OnEnable();
                    break;
                case 1:
                    inspectorSort.OnEnable();
                    break;
                default:
                    quickSortCore.OnEnable();
                    break;
            }
        }
        private void Click(InputAction.CallbackContext obj)
        {
            switch (flag)
            {
                case 1:
                    bubbleSortCore.Click();
                    break;
                case 2:
                    inspectorSort.Click();
                    break;
                case 0:
                    quickSortCore.Click();
                    break;
            }
        }
        

        public void Switch()
        {
            switch (flag)
            {
                case 0:
                    inspectorSort.gameObject.SetActive(false);
                    quickSortCore.gameObject.SetActive(false);
                    bubbleSortCore.gameObject.SetActive(true);
                    text.text = "冒泡";
                    flag = 1;
                    break;
                case 1:
                    quickSortCore.gameObject.SetActive(false);
                    bubbleSortCore.gameObject.SetActive(false);
                    inspectorSort.gameObject.SetActive(true);
                    text.text = "插入";
                    flag = 2;
                    break;
                case 2:
                    inspectorSort.gameObject.SetActive(false);
                    bubbleSortCore.gameObject.SetActive(false);
                    quickSortCore.gameObject.SetActive(true);
                    quickSortCore.ReGen();
                    text.text = "快速";
                    flag = 0;
                    break;
            }
        }
    }
}