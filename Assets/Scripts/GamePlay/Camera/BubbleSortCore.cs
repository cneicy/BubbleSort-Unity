using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GamePlay.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GamePlay.Camera
{
    public class BubbleSortCore : MonoBehaviour
    {
        [SerializeField] public int dataCount;
        [SerializeField] public float playSpeed;
        [SerializeField] private GameObject dataItem;
        [SerializeField] private List<ItemHelper> dataItems = new();
        [SerializeField] private bool autoSort=true;
        [SerializeField] private TMP_Text arrow;
        [SerializeField] private TMP_InputField dataCountInputField;
        [SerializeField] private Toggle autoSortToggle;
        [SerializeField] private TMP_InputField playSpeedInputField;
        [SerializeField] private List<AudioSource> audioSources = new();

        private int _currentFirstIndex;
        private int _currentSecondIndex;
        private bool _isSorting;

        private void Start()
        {
            dataCountInputField.onValueChanged.AddListener(OnDataCountChanged);
            playSpeedInputField.onValueChanged.AddListener(OnPlaySpeedChanged);
            autoSortToggle.onValueChanged.AddListener(OnAutoSortToggleChanged);

            DataGen();
        }

        private void OnDataCountChanged(string value)
        {
            if (!int.TryParse(value, out var result)) return;
            dataCount = result;
            ReGen();
        }

        private void OnPlaySpeedChanged(string value)
        {
            if (float.TryParse(value, out var result))
            {
                playSpeed = result;
            }
        }

        private void OnAutoSortToggleChanged(bool value)
        {
            autoSort = value;
        }

        public void ReGen()
        {
            foreach (var item in dataItems)
            {
                Destroy(item.gameObject);
            }

            dataItems.Clear();
            _isSorting = false;
            _currentFirstIndex = 0;
            _currentSecondIndex = 0;
            StopAllCoroutines();
            DataGen();
        }

        private void DataGen()
        {
            for (var i = 0; i < dataCount; i++)
            {
                var temp = Random.Range(5, dataCount * 1.25f);
                var newItem = Instantiate(dataItem, new Vector3(i * 0.75f, 0, 5), Quaternion.identity)
                    .AddComponent<ItemHelper>().GetComponent<ItemHelper>();
                newItem.value = temp;
                newItem.index = i;
                newItem.rend.material.color = Color.white;
                dataItems.Add(newItem);
            }
        }

        public void Click()
        {
            if (!_isSorting)
            {
                StartCoroutine(Sort());
            }
        }

        private IEnumerator Sort()
        {
            _isSorting = true;

            if (_currentSecondIndex >= dataCount - 1 - _currentFirstIndex)
            {
                _currentFirstIndex++;
                _currentSecondIndex = 0;
            }

            if (_currentFirstIndex >= dataCount - 1)
            {
                dataItems[0].rend.material.DOColor(Color.cyan, 0.2f * playSpeed);
                yield break;
            }

            if (dataItems[_currentSecondIndex].value > dataItems[_currentSecondIndex + 1].value)
            {
                yield return SwapItems(dataItems[_currentSecondIndex], dataItems[_currentSecondIndex + 1]);
            }
            else
            {
                yield return HighlightComparison(dataItems[_currentSecondIndex], dataItems[_currentSecondIndex + 1], Color.yellow);
            }

            yield return new WaitForSeconds(0.2f * playSpeed);

            _currentSecondIndex++;
            if (_currentSecondIndex >= dataCount - 1 - _currentFirstIndex)
            {
                dataItems[_currentSecondIndex].rend.material.DOColor(Color.cyan, 0.2f * playSpeed);
            }

            _isSorting = false;
            if (autoSort)
            {
                StartCoroutine(Sort());
            }
        }

        private IEnumerator SwapItems(ItemHelper item1, ItemHelper item2)
        {
            item1.rend.material.DOColor(Color.green, 0.2f * playSpeed);
            item2.rend.material.DOColor(Color.green, 0.2f * playSpeed);

            var oriPos1 = item1.transform.position;
            var oriPos2 = item2.transform.position;
            var outPos1 = oriPos1 - new Vector3(0, 0, 0.5f);
            var outPos2 = oriPos2 - new Vector3(0, 0, 0.5f);

            PlayAudio();

            item1.transform.DOMove(outPos1, 0.2f * playSpeed);
            item2.transform.DOMove(outPos2, 0.2f * playSpeed);
            yield return new WaitForSeconds(0.2f * playSpeed);

            var startValue1 = item1.value;
            var startValue2 = item2.value;
            var duration = 0.1f * playSpeed;

            yield return StartCoroutine(AnimateValueChange(item1, startValue1, startValue2, duration));
            yield return StartCoroutine(AnimateValueChange(item2, startValue2, startValue1, duration));

            item1.rend.material.DOColor(Color.white, 0.2f * playSpeed);
            item2.rend.material.DOColor(Color.white, 0.2f * playSpeed);

            if (playSpeed > 0.5f)
            {
                item1.transform.DOMove(oriPos1, 0.2f * playSpeed);
                item2.transform.DOMove(oriPos2, 0.2f * playSpeed);
            }
            else
            {
                item1.transform.position = oriPos1;
                item2.transform.position = oriPos2;
            }
        }

        private IEnumerator HighlightComparison(ItemHelper item1, ItemHelper item2, Color color)
        {
            item1.rend.material.DOColor(color, 0.2f * playSpeed);
            item2.rend.material.DOColor(color, 0.2f * playSpeed);

            var oriPos1 = item1.transform.position;
            var oriPos2 = item2.transform.position;
            var outPos1 = oriPos1 - new Vector3(0, 0, 0.5f);
            var outPos2 = oriPos2 - new Vector3(0, 0, 0.5f);

            PlayAudio();

            item1.transform.DOMove(outPos1, 0.2f * playSpeed);
            item2.transform.DOMove(outPos2, 0.2f * playSpeed);
            yield return new WaitForSeconds(0.2f * playSpeed);

            item1.rend.material.DOColor(Color.white, 0.2f * playSpeed);
            item2.rend.material.DOColor(Color.white, 0.2f * playSpeed);

            if (playSpeed > 0.5f)
            {
                item1.transform.DOMove(oriPos1, 0.2f * playSpeed);
                item2.transform.DOMove(oriPos2, 0.2f * playSpeed);
            }
            else
            {
                item1.transform.position = oriPos1;
                item2.transform.position = oriPos2;
            }
        }

        private void PlayAudio()
        {
            foreach (var source in audioSources.Where(source => !source.isPlaying))
            {
                source.Play();
                return;
            }
        }

        private IEnumerator AnimateValueChange(ItemHelper item, float startValue, float endValue, float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
                item.value = Mathf.Round(newValue);
                yield return null;
            }

            item.value = endValue;
        }
    }
}
