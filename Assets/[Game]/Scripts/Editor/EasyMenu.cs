#if UNITY_EDITOR
#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

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
