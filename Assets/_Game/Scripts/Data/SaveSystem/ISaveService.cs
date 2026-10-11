using UnityEngine;

public interface ISaveService
{
    T Load<T>(string key) where T : new();
    void Save<T>(string key, T data);
}

public class PlayerPrefsSaveService : ISaveService
{
    public T Load<T>(string key) where T : new()
    {
        string json = PlayerPrefs.GetString(key, string.Empty);
        return string.IsNullOrEmpty(json) ? new T() : JsonUtility.FromJson<T>(json);
    }

    public void Save<T>(string key, T data)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }
}
