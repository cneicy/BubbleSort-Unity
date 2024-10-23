using UnityEngine;

namespace GamePlay.Helper
{
    public class ItemHelper : MonoBehaviour
    {
        public float value;
        public int index;
        public Renderer rend;

        private void Awake()
        {
            rend = GetComponent<Renderer>();
        }

        private void Update()
        {
            transform.localScale = new Vector3(0.5f, value*0.75f, 0.5f);
        }
    }
}