#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EasyMenu : MonoBehaviour
{
    [MenuItem("Easy Menu/Clear All PlayerPrefs")]
    static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("PlayerPrefs Cleared !");
    }
}
#endif
