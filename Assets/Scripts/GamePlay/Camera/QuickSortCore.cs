using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GamePlay.Helper;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlay.Camera
{
    public class QuickSortCore : MonoBehaviour
    {
        [SerializeField] public int dataCount;
        [SerializeField] public float playSpeed;
        [SerializeField] private GameObject dataItem;
        [SerializeField] private List<ItemHelper> dataItems = new();
        [SerializeField] private TMP_InputField dataCountInputField;
        [SerializeField] private TMP_InputField playSpeedInputField;
        [SerializeField] private GameObject autoSortToggle;
        [SerializeField] private List<AudioSource> audioSources = new();

        [SerializeField] private TMP_Text timeText;
        public float timeCost;
        public int compTime;
        private float _startTime;
        public int swapTime;
        
        private bool _isSorting;
        private int _partitionResult;

        private void Start()
        {
            dataCountInputField.onValueChanged.AddListener(OnDataCountChanged);
            playSpeedInputField.onValueChanged.AddListener(OnPlaySpeedChanged);
            DataGen();
        }

        public void OnEnable()
        {
            dataCount = int.Parse(dataCountInputField.text);
            playSpeed = float.Parse(playSpeedInputField.text);
            autoSortToggle.SetActive(false);
            UpdateTimeText();
            ReGen();
        }

        private void UpdateTimeText()
        {
            timeText.text = timeCost == 0 ? $"TimeCost:\nWaiting\nCompTime:\n{compTime}\nSwapTime:\n{swapTime}" : $"TimeCost:\n{timeCost:F2}\nCompTime:\n{compTime}\nSwapTime:\n{swapTime}";
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
                if(dataItems.Count > dataCount) ReGen();
            }
        }

        public void Click()
        {
            if (_isSorting) return;
            timeCost = 0;
            compTime = 0;
            swapTime = 0;
            _startTime = Time.time;
            StartCoroutine(Sort(0, dataCount - 1));
        }

        private IEnumerator Sort(int low, int high)
        {
            _isSorting = true;

            if (low < high)
            {
                compTime++;
                UpdateTimeText();
                yield return Partition(low, high);
                var pivotIndex = _partitionResult;
                
                yield return Sort(low, pivotIndex - 1);
                yield return Sort(pivotIndex + 1, high);
            }
            
            if (low == 0 && high == dataCount - 1)
            {
                foreach (var item in dataItems)
                {
                    item.rend.material.DOColor(Color.cyan, 0.2f * playSpeed);
                }
                timeCost = Time.time - _startTime;
                UpdateTimeText();
            }
            
            _isSorting = false;
        }

        private IEnumerator Partition(int low, int high)
        {
            var pivot = dataItems[high];
            PlayAudio();
            pivot.rend.material.DOColor(Color.red, 0.2f * playSpeed);

            var i = low - 1;

            for (var j = low; j < high; j++)
            {
                PlayAudio();
                dataItems[j].rend.material.DOColor(Color.yellow, 0.2f * playSpeed);
                yield return new WaitForSeconds(0.2f * playSpeed);

                if (dataItems[j].value < pivot.value)
                {
                    compTime++;
                    UpdateTimeText();
                    i++;
                    yield return SwapValues(dataItems[i], dataItems[j]);
                }

                dataItems[j].rend.material.DOColor(Color.white, 0.2f * playSpeed);
            }

            yield return SwapValues(dataItems[i + 1], pivot);
            pivot.rend.material.DOColor(Color.cyan, 0.2f * playSpeed);

            _partitionResult = i + 1;
            yield return null;
        }

        private IEnumerator SwapValues(ItemHelper item1, ItemHelper item2)
        {
            swapTime++;
            UpdateTimeText();
            PlayAudio();
            (item1.value, item2.value) = (item2.value, item1.value);
            
            item1.rend.material.DOColor(Color.green, 0.2f * playSpeed);
            item2.rend.material.DOColor(Color.green, 0.2f * playSpeed);
            yield return new WaitForSeconds(0.2f * playSpeed);
            
            item1.rend.material.DOColor(Color.white, 0.2f * playSpeed);
            item2.rend.material.DOColor(Color.white, 0.2f * playSpeed);
        }

        private void OnDisable()
        {
            autoSortToggle.SetActive(true);
            foreach (var obj in dataItems)
            {
                Destroy(obj.gameObject);
            }
        }

        private void PlayAudio()
        {
            var availableSource = audioSources.FirstOrDefault(source => !source.isPlaying);
            if (availableSource)
            {
                availableSource.Play();
            }
        }
    }
}
