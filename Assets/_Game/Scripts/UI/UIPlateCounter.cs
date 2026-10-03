using TMPro;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class UIPlateCounter : MonoBehaviour
{
    [SerializeField] PlatePoolManager poolManager;
    [SerializeField] TextMeshPro plateCounterTxt;
    [SerializeField, Range(0, 0.5f)] float counterTxtForce = 0.2f;

    Vector3 defaultPos;
    [SerializeField] Vector3 offset = new Vector3(0.05f, 0.05f, 0.05f);

    private void OnEnable() 
    {
        GameEvent.OnPlateCountChanged += HandlePlateCount;
        GameEvent.OnPlateCountEmpty += HandlePlateCountEmpty;
    } 
    private void OnDisable()
    {
        GameEvent.OnPlateCountChanged -= HandlePlateCount;
        GameEvent.OnPlateCountEmpty -= HandlePlateCountEmpty;
    }

    private void Start()
    {
        defaultPos = transform.localPosition;
    }

    void HandlePlateCount() => ShowPlateCount();

    void ShowPlateCount()
    {
        plateCounterTxt.SetText("{0}/{1}", poolManager.CurrentPlate, poolManager.MaxPlate);
        Animation();
    }

    public void HandlePlateCountEmpty()
    {
        transform.DOKill();
        transform.localPosition = defaultPos;
        plateCounterTxt.color = Color.red;

        transform.DOPunchPosition(Vector3.right, counterTxtForce)
            .OnComplete(() =>
            {
                transform.localPosition = defaultPos;
                plateCounterTxt.color = Color.white;
            });
    }

    void Animation()
    {
        transform.DOKill(true);
        transform.localScale = Vector3.one;
        transform.DOPunchScale(offset, GameTags.Animation.DOTWEEN_ANIM_DURATION, 5, 1);
    }
}
