using System;
public static class GameEvent
{
    public static event Action OnGameRestart;
    public static event Action OnGameLose;

    public static void TriggerGameRestart() => OnGameRestart?.Invoke();
    public static void TriggerGameLose() => OnGameLose?.Invoke();
}
