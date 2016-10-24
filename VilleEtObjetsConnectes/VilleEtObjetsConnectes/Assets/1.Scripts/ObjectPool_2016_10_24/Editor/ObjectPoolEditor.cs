using UnityEngine;
using UnityEditor;
using System.Collections;

using EquilibreGames;

/*[CustomEditor(typeof(ObjectPool))]
  [CanEditMultipleObjects]
  public class ObjectPoolInspector : Editor
  {

      SerializedProperty poolObjectList;

      void OnEnable()
      {
          poolObjectList = serializedObject.FindProperty("poolObjectList");
      }

      public override void OnInspectorGUI()
      {
          // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
          serializedObject.Update();

          Color previousColor = GUI.color;
          //*
          EditorGUILayout.BeginHorizontal();
          GUILayout.Label("kind");
          GUILayout.Label("tag");
          //GUILayout.Label("R", GUILayout.Width(30));
          GUILayout.Label("init");
          GUILayout.Label("max");
          GUILayout.Label("----", GUILayout.Width(80));
          EditorGUILayout.EndHorizontal();

          if (poolObjectList.arraySize > 0)
          {
              for (int i = 0; i < poolObjectList.arraySize; i++)
              {
                  SerializedProperty data = poolObjectList.GetArrayElementAtIndex(i);
                  EditorGUILayout.BeginHorizontal();

                  // PoolObject : name, prefab, bufferCount, defaultParent
                  SerializedProperty kind = data.FindPropertyRelative("kind");
                  kind.stringValue = EditorGUILayout.TextField(kind.stringValue);
                  SerializedProperty tag = data.FindPropertyRelative("tag");
                  tag.stringValue = EditorGUILayout.TagField(tag.stringValue);

                  SerializedProperty prefab = data.FindPropertyRelative("prefab");
                  if (prefab.objectReferenceValue == null)
                      GUI.color = Color.red;
                  prefab.objectReferenceValue = EditorGUILayout.ObjectField(prefab.objectReferenceValue, typeof(GameObject), false, GUILayout.Width(30));
                  GUI.color = previousColor;

                  SerializedProperty bufferCount = data.FindPropertyRelative("bufferCount");
                  //float fBufferCount = (float)bufferCount.intValue;
                  bufferCount.intValue = EditorGUILayout.IntField(bufferCount.intValue);
                  SerializedProperty maxCount = data.FindPropertyRelative("maxCount");
                  maxCount.intValue = EditorGUILayout.IntField(maxCount.intValue);

                  //float fMaxCount = (float)maxCount.intValue;
                  //EditorGUILayout.MinMaxSlider(ref fBufferCount, ref fMaxCount, 1, 20);
                  //bufferCount.intValue = (int)fBufferCount;
                  //maxCount.intValue = (int)fMaxCount;

                  GUILayout.Label("on");
                  SerializedProperty defaultParent = data.FindPropertyRelative("defaultParent");
                  if (defaultParent.objectReferenceValue == null)
                      GUI.color = Color.red;
                  defaultParent.objectReferenceValue = EditorGUILayout.ObjectField(defaultParent.objectReferenceValue, typeof(Transform), true);
                  GUI.color = previousColor;

                  GUI.enabled = (i > 0);
                  if (GUILayout.Button("U", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                  {
                      // Switch with previous property
                      poolObjectList.MoveArrayElement(i, i - 1);
                  }
                  GUI.enabled = (i < poolObjectList.arraySize - 1);
                  if (GUILayout.Button("D", EditorStyles.miniButtonMid, GUILayout.Width(20)))
                  {
                      // Switch with next property
                      poolObjectList.MoveArrayElement(i, i + 1);
                  }
                  GUI.enabled = true;
                  GUI.color = Color.green;
                  if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(20)))
                  {
                      poolObjectList.InsertArrayElementAtIndex(i + 1);
                  }
                  GUI.color = Color.red;
                  if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                  {
                      // Remove property
                      poolObjectList.DeleteArrayElementAtIndex(i);
                  }
                  GUI.color = previousColor;
                  EditorGUILayout.EndHorizontal();

              }
          }
          else
          {
              GUI.color = Color.green;
              if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20)))
              {
                  poolObjectList.InsertArrayElementAtIndex(0);
              }
              GUI.color = previousColor;
          }
          // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
          serializedObject.ApplyModifiedProperties();
      }
  }*/
