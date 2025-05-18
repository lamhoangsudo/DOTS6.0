using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button mainmenuButton;

    private void Start()
    {
        mainmenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        });
        DOTSEventsManager.Instance.OnHQDead += DOTSEventsManager_OnHQDead;
        Hide();
    }

    private void DOTSEventsManager_OnHQDead(object sender, EventArgs e)
    {
        Show();
        Time.timeScale = 0;
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
