using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TransitionScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float transitionDuration = 1f;
    [SerializeField] float transitionSpeed = 0.25f;

    public void ToggleTransitionScreen()
    {
        StartCoroutine(TransitionScreenCoroutine());
    }

    IEnumerator TransitionScreenCoroutine()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, transitionSpeed);

        yield return new WaitForSeconds(transitionDuration);

        canvasGroup.DOFade(0f, transitionSpeed);
        canvasGroup.blocksRaycasts = false;
    }
}
