using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private void Start()
    {
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R");
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("RR");
            }
        }
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        if (pauseMenu.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
