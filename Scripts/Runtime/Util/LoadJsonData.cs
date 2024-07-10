
using UnityEngine;

public static class LoadJsonData
{
    public static MailData Load(string path)
    {
        var jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile != null)
        {
            var data = JsonUtility.FromJson<MailData>(jsonFile.text);
            return data;
        }
        else
        {
            Debug.LogError("JSON file not found" + jsonFile);
            return null;
        }
    }
}
