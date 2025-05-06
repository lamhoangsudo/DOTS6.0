using System;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    private void Start()
    {
        Hide();
        DOTSEventsManager.Instance.OnHQDead += OnHQDead;
    }

    private void OnHQDead(object sender, EventArgs e)
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
