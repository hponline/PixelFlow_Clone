using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] int maxLife;
    [SerializeField] int currentLife;

    public event Action<int> OnLivesChanged;
    public bool HasEnoughLife() => currentLife > 0;
    public int CurrentLife => currentLife;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Setup();
    }

    void Setup()
    {
        currentLife = DataManager.Instance.currentLife;
    }

    [Button]
    public void SpendLife()
    {
        currentLife--;
        OnLivesChanged?.Invoke(currentLife);

        if (!HasEnoughLife())
        {
            GameEvent.TriggerGameLose();
        }
    }

    public void RestartGame()
    {
        if (!HasEnoughLife()) return;

        GameEvent.TriggerGameRestart();
        SceneManager.LoadScene(0); // tek sahne olursa burasý kalkacak

        Debug.Log("Restart Game");
    }
}
