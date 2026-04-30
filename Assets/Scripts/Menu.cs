using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public delegate void DelegateOnClick();

    public DelegateOnClick OnClick;

    public GameObject topPanel;
    public GameObject bottomPanel;
    public GameObject gameCompletionPanel;

    public TextMeshProUGUI txtTime;
    public TextMeshProUGUI txtTotalTiles;
    public TextMeshProUGUI txtTilesInPlace;

    private void Start()
    {
        SetEnableBottomPanel(false, 0);
        SetEnableTopPanel(false, 0);
        SetEnableGameCompletionPanel(false, 0);
    }

    private static IEnumerator FadeInUi(GameObject go, float fadeInDuration = 2.0f)
    {
        go.SetActive(true);
        var graphics = go.GetComponentsInChildren<Graphic>();
        foreach (var graphic in graphics)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0);
        }

        var timer = 0.0f;
        while(timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            var alpha = Mathf.Clamp01(timer / fadeInDuration);
            foreach (var graphic in graphics)
            {
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
            }
            yield return null;
        }

        foreach (var graphic in graphics)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f);
        }
    }

    private static IEnumerator FadeOutUi(GameObject go, float fadeOutDuration = 2.0f)
    {
        var graphics = go.GetComponentsInChildren<Graphic>();
        foreach (var graphic in graphics)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f);
        }

        var timer = 0.0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            var alpha = 1.0f - Mathf.Clamp01(timer / fadeOutDuration);
            foreach (var graphic in graphics)
            {
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
            }

            yield return null;
        }

        foreach (var graphic in graphics)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);
        }

        go.SetActive(false);
    }

    public void SetEnableBottomPanel(bool enable, float fadeDuration = 2.0f)
    {
        StartCoroutine(enable ? FadeInUi(bottomPanel, fadeDuration) : FadeOutUi(bottomPanel, fadeDuration));
    }

    public void SetEnableTopPanel(bool enable, float fadeDuration = 2.0f)
    {
        StartCoroutine(enable ? FadeInUi(topPanel, fadeDuration) : FadeOutUi(topPanel, fadeDuration));
    }

    public void SetEnableGameCompletionPanel(bool enable, float fadeDuration = 2.0f)
    {
        StartCoroutine(enable ? FadeInUi(gameCompletionPanel, fadeDuration) : FadeOutUi(gameCompletionPanel, fadeDuration));
    }

    public void OnClickPlay()
    {
        OnClick?.Invoke();
    }

    public void SetTimeInSeconds(double seconds)
    {
        var time = TimeSpan.FromSeconds(seconds);
        var strTime = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";

        txtTime.text = $"{strTime}";
    }

    public void SetTotalTiles(int count)
    {
        txtTotalTiles.text = $"{count}"; // Total Tiles:
    }

    public void SetTilesInPlace(int count)
    {
        txtTilesInPlace.text = $"Tiles in place: {count}";
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickPlayAgain()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}