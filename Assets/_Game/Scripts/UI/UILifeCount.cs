using UnityEngine;
using System;
using TMPro;
using System.Collections;

public class UILifeCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lifeAmountText;
    [SerializeField] private TextMeshProUGUI lifeTimerCountTxt;
    [SerializeField] private GameObject lifeTimerFullTxt;

    [SerializeField] private GameObject unlimitedIcon;
    [SerializeField] private TextMeshProUGUI unlimitedTimerTxt;

    Coroutine _unlimitedCoroutine;

    void Start()
    {
        LifeManager.Instance.OnLifeChanged += UpdateLifeText;
        LifeManager.Instance.OnUnLimitedLivesActivited += StartUnlimitedUI;
        LifeManager.Instance.OnUnlimitedLivesExpired += StopUnlimitedUI;

        UpdateLifeText(LifeManager.Instance.CurrentLife);

        if (LifeManager.Instance.IsUnLimitedLives)
            StartUnlimitedUI(LifeManager.Instance.UnlimitedLivesExpiry);
        else
            StopUnlimitedUI();
    }

    void OnDestroy()
    {
        LifeManager.Instance.OnLifeChanged -= UpdateLifeText;
        LifeManager.Instance.OnUnLimitedLivesActivited -= StartUnlimitedUI;
        LifeManager.Instance.OnUnlimitedLivesExpired -= StopUnlimitedUI;
    }

    private void UpdateLifeText(int currentLife)
    {
        lifeAmountText.SetText("{0}", currentLife);
        PunchPanel();
    }

    void Update()
    {
        // Unlimited aktifken normal timer çalışmaz
        if (LifeManager.Instance.IsUnLimitedLives)
        {
            lifeAmountText.gameObject.SetActive(false);
            lifeTimerFullTxt.SetActive(false);
            lifeTimerCountTxt.gameObject.SetActive(false);
            return;
        }

        if (LifeManager.Instance.IsFull)
        {
            lifeTimerFullTxt.SetActive(true);
            lifeTimerCountTxt.gameObject.SetActive(false);
            return;
        }

        lifeAmountText.gameObject.SetActive(true);
        lifeTimerFullTxt.SetActive(false);
        lifeTimerCountTxt.gameObject.SetActive(true);

        TimeSpan remaining = LifeManager.Instance.NextLifeTime - DateTime.UtcNow;
        if (remaining.TotalSeconds < 0) remaining = TimeSpan.Zero;

        lifeTimerCountTxt.SetText("{0}:{1:00}", remaining.Minutes, remaining.Seconds);
    }

    // ── Unlimited Lives UI ─────────────────────────────────────────────────────

    private void StartUnlimitedUI(DateTime expiry)
    {
        if (_unlimitedCoroutine != null)
            StopCoroutine(_unlimitedCoroutine);

        unlimitedIcon.SetActive(true);
        unlimitedTimerTxt.gameObject.SetActive(true);
        _unlimitedCoroutine = StartCoroutine(UnlimitedCountdownCoroutine(expiry));
    }

    private void StopUnlimitedUI()
    {
        if (_unlimitedCoroutine != null)
            StopCoroutine(_unlimitedCoroutine);

        unlimitedIcon.SetActive(false);
        unlimitedTimerTxt.gameObject.SetActive(false);
    }

    private IEnumerator UnlimitedCountdownCoroutine(DateTime expiry)
    {
        while (DateTime.UtcNow < expiry)
        {
            TimeSpan remaining = expiry - DateTime.UtcNow;
            string fmt = remaining.TotalHours >= 1 ? @"h\:mm\:ss" : @"mm\:ss";
            unlimitedTimerTxt.SetText(remaining.ToString(fmt));
            yield return new WaitForSeconds(1f);
        }
        unlimitedTimerTxt.SetText("00:00");
    }

    void PunchPanel()
    {
        UIManager.Instance.PanelPunchAnimation(gameObject);
    }
}
