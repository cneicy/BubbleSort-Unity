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
    public class InsertionSortCore : MonoBehaviour
    {
        [SerializeField] public int dataCount;
        [SerializeField] public float playSpeed;
        [SerializeField] private GameObject dataItem;
        [SerializeField] private List<ItemHelper> dataItems = new();
        [SerializeField] private bool autoSort;
        [SerializeField] private TMP_InputField dataCountInputField;
        [SerializeField] private TMP_InputField playSpeedInputField;
        [SerializeField] private Toggle autoSortToggle;
        [SerializeField] private List<AudioSource> audioSources = new();
        
        [SerializeField] private TMP_Text timeText;
        public float timeCost;
        public int compTime;
        private float _startTime;
        public int swapTime;
        
        private int _currentIndex;
        private int _comparisonIndex;
        private bool _isSorting;

        private void UpdateTimeText()
        {
            timeText.text = timeCost == 0 ? $"TimeCost:\nWaiting\nCompTime:\n{compTime}\nSwapTime:\n{swapTime}" : $"TimeCost:\n{timeCost:F2}\nCompTime:\n{compTime}\nSwapTime:\n{swapTime}";
        }

        public void OnEnable()
        {
            dataCount = int.Parse(dataCountInputField.text);
            autoSort = autoSortToggle.isOn;
            playSpeed = float.Parse(playSpeedInputField.text);
            UpdateTimeText();
            ReGen();
        }
        
        private void Start()
        {
            dataCountInputField.onValueChanged.AddListener(OnDataCountChanged);
            playSpeedInputField.onValueChanged.AddListener(OnPlaySpeedChanged);
            autoSortToggle.onValueChanged.AddListener(OnAutoSortToggleChanged);
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
            foreach (var item in dataItems.Where(item => item != null))
            {
                Destroy(item.gameObject);
            }

            var temp = GameObject.FindGameObjectsWithTag("Data");
            foreach (var item in temp)
            {
                Destroy(item);
            }

            dataItems.Clear();
            _isSorting = false;
            _currentIndex = 1;
            timeCost = 0;
            compTime = 0;
            _startTime = Time.time;
            swapTime = 0;
            DataGen();
        }

        private void DataGen()
        {
            for (var i = 0; i < dataCount; i++)
            {
                var temp = Random.Range(5, dataCount * 1.25f);
                var newItem = Instantiate(dataItem, new Vector3(i * 0.75f, 0, 5), Quaternion.identity);
                newItem.AddComponent<ItemHelper>();
                var tmp = newItem.GetComponent<ItemHelper>();
                tmp.value = temp;
                tmp.index = i;
                tmp.rend.material.color = Color.white;
                dataItems.Add(tmp);
            }
        }

        public void Click()
        {
            if (_isSorting) return;
            timeCost = 0;
            compTime = 0;
            _startTime = Time.time;
            swapTime = 0;
            StartCoroutine(Sort());
        }

        private IEnumerator Sort()
        {
            _isSorting = true;

            if (_currentIndex >= dataCount)
            {
                foreach (var item in dataItems)
                {
                    item.rend.material.DOColor(Color.cyan, 0.2f * playSpeed);
                }
                timeCost = Time.time - _startTime;
                UpdateTimeText();
                yield break;
            }

            var keyValue = dataItems[_currentIndex].value;
            _comparisonIndex = _currentIndex - 1;

            while (_comparisonIndex >= 0 && dataItems[_comparisonIndex].value > keyValue)
            {
                compTime++;
                UpdateTimeText();
                yield return HighlightComparison(dataItems[_comparisonIndex], dataItems[_comparisonIndex + 1], Color.yellow);
                dataItems[_comparisonIndex + 1].value = dataItems[_comparisonIndex].value;
                
                _comparisonIndex--;
            }
            
            dataItems[_comparisonIndex + 1].value = keyValue;
            swapTime++;
            UpdateTimeText();
            yield return new WaitForSeconds(0.2f * playSpeed);

            _currentIndex++;

            _isSorting = false;
            if (autoSort)
            {
                StartCoroutine(Sort());
            }
        }

        private IEnumerator HighlightComparison(ItemHelper item1, ItemHelper item2, Color color)
        {
            item1.rend.material.DOColor(color, 0.2f * playSpeed);
            item2.rend.material.DOColor(color, 0.2f * playSpeed);
            PlayAudio();
            yield return new WaitForSeconds(0.2f * playSpeed);
            item1.rend.material.DOColor(Color.white, 0.2f * playSpeed);
            item2.rend.material.DOColor(Color.white, 0.2f * playSpeed);
        }

        private void PlayAudio()
        {
            var availableSource = audioSources.FirstOrDefault(source => !source.isPlaying);
            if (availableSource)
            {
                availableSource.Play();
            }
        }

        private void OnDisable()
        {
            foreach (var obj in dataItems)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}
