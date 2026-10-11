using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;

        Setup();
    }

    void Setup()
    {
        //
    }

    [Button]
    public void SpendLife()
    {
        //LifeManager.Instance.o(CurrencyType.Life, 1);

        if (!CurrencyManager.Instance.Has(CurrencyType.Coin, 0))
            GameEvent.TriggerGameLose();
    }

    public void RestartGame()
    {
        if (!LifeManager.Instance.HasEnoughLife()) return;

        GameEvent.TriggerGameRestart(); // ??
        GameEvent.TriggerLevelRestart(); 
        SceneManager.LoadScene(0); // tek sahne olursa burasý kalkacak

        Debug.Log("Restart Game tek sahne olursa burasý kalkacak");
    }
}
