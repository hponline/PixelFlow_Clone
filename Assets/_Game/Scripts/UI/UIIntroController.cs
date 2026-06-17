using DG.Tweening;
using UnityEngine;

public class UIIntroController : MonoBehaviour, IUIScreen
{
    [SerializeField] RectTransform topSide;
    [SerializeField] RectTransform botSide;

    [SerializeField] Vector3 topSideTargetY = new Vector3(0, -80, 0);
    [SerializeField] Vector3 botSideTargetY = new Vector3(0, 60, 0);

    [SerializeField, Range(0,1)] float duration = 0.5f;
    [SerializeField] Ease ease = Ease.OutCubic;

    Vector3 startTopSidePos;
    Vector3 startBotSidePos;

    private void Awake()
    {
        startTopSidePos = transform.position;
        startBotSidePos = transform.position;
    }

    private void Start()
    {
        Show();
    }

    public void Show()
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(topSide.DOAnchorPos(topSideTargetY, duration)).SetEase(ease);
        seq.Join(botSide.DOAnchorPos(botSideTargetY, duration)).SetEase(ease);
    }

    public void Hide()
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(topSide.DOAnchorPos(startTopSidePos, duration)).SetEase(ease);
        seq.Join(topSide.DOAnchorPos(startBotSidePos, duration)).SetEase(ease);
    }

}
