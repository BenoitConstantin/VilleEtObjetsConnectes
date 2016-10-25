using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TeamScriptableObject : ScriptableObject {

    [System.Serializable]
    public class TeamInfo
    {
        public Material color;
        public string flower;
    }


    public TeamInfo[] teamInfos;



#if UNITY_EDITOR
    [MenuItem("Window/TeamScriptableObject/Create new TeamScriptableObject")]
    public static void LoadDataStatic()
    {
        TeamScriptableObject scriptableObject = ScriptableObject.CreateInstance<TeamScriptableObject>();
        AssetDatabase.CreateAsset(scriptableObject, "Assets/Resources/TeamScriptableObject" + ".asset");
        AssetDatabase.SaveAssets();
    }
#endif

}
