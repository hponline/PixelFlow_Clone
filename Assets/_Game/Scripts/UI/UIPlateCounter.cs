using TMPro;
using UnityEngine;
using DG.Tweening;

public class UIPlateCounter : MonoBehaviour
{
    [SerializeField] PlatePoolManager poolManager;
    [SerializeField] TextMeshPro plateCounterTxt;

    [SerializeField] Vector3 offset = new Vector3(0.05f, 0.05f, 0.05f);

    private void OnEnable() => GameEvent.OnPlateCountChanged += HandlePlateCount;
    private void OnDisable() => GameEvent.OnPlateCountChanged -= HandlePlateCount;

    void HandlePlateCount() => ShowPlateCount();

    void ShowPlateCount()
    {
        plateCounterTxt.SetText("{0}/{1}", poolManager.CurrentPlate, poolManager.MaxPlate);
        Animation();
    }

    void Animation()
    {
        transform.DOKill(true);
        transform.localScale = Vector3.one;
        transform.DOPunchScale(offset, GameTags.Animation.DOTWEEN_ANIM_DURATION, 5, 1);
    }
}
