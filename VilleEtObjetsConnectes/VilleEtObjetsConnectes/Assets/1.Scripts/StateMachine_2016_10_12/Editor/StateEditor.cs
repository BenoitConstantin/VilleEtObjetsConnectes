using UnityEngine;
using System.Collections;
using UnityEditor;

namespace EquilibreGames
{
    [CustomEditor(typeof(State),true)]
    public class StateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);

            if (((State)target).enabled)
            {
                GUIStyle activeStyle = new GUIStyle();
                activeStyle.fontSize = 20;
                activeStyle.normal.textColor = Color.green;

                EditorGUILayout.LabelField("ACTIVE", activeStyle);       
            }
            else
            {
                GUIStyle inactiveStyle = new GUIStyle();
                inactiveStyle.fontSize = 8;
                inactiveStyle.normal.textColor = Color.red;

                EditorGUILayout.LabelField("INACTIVE", inactiveStyle);
            }
            GUILayout.Space(10);

            base.OnInspectorGUI();
        }
    }
}
