using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace EquilibreGames
{
    [ExecuteInEditMode]
    [System.Serializable]
    public class ObjectPool : Singleton<ObjectPool>
    { 
        protected ObjectPool() { }

        //Used for pooling GameObject
        [System.Serializable]
        public class PoolGameObjectInfo
        {
            public string kind;
            public string tag;
            public GameObject prefab;
            public int bufferCount = 10;            //Min of gameObject instantiate
            public int maxCount = 10;               //Max of gameObject instantiate
            public Transform defaultParent;
        }

        //Used for pooling Scene
        [System.Serializable]
        public class PoolSceneInfo
        {
            public string kind;

            [System.Serializable]
            public class SceneInfos
            {
                public string sceneNames;
                public int bufferCount = 2;
            }

            public SceneInfos[] sceneInfos;
        }


        /* THIS WAS ADDED TO COMPENSE DESTROY(OBJ,TIME) */
        public class PoolData
        {
            public PoolData(GameObject _obj, float _time)
            {
                gameObjectToPool = _obj;
                time = _time;
            }

            public GameObject gameObjectToPool;
            public float time;
        }

        [SerializeField]
        bool InitOnAwake = true;

        /// <summary>
        /// The object prefabs which the pool can handle.
        /// </summary>
        [SerializeField]
        List<PoolGameObjectInfo> poolObjectList;

        [Space(20)]
        [SerializeField][Tooltip("Used for finding pooled scene between all scene loaded, specify this in your pooled scene name")]
        string prefixPooledScene = "Pooled_";

        [SerializeField]
        List<PoolSceneInfo> poolSceneList;

        public Action OnScenesLoadEnd;


        [NonSerialized]
        public Scene mergedScene;

        /// <summary>
        /// The pooled objects currently available.
        /// </summary>
        private Dictionary<string, List<GameObject>> pooledObjects = new Dictionary<string, List<GameObject>>();

        /// <summary>
        /// Accelerate treatement
        /// </summary>
        private Dictionary<string, PoolSceneInfo> poolSceneInfoDictionnary = new Dictionary<string, PoolSceneInfo>();


        List<PoolData> poolDatas = new List<PoolData>();
        int cptSceneLoading = 0;

       /* [SerializeField]
        string mergedSceneName = "PooledMergedScene";*/

        void Awake()
        {
            if(InitOnAwake)
                Init();

           // SceneManager.sceneLoaded += VerifyEndOfSceneLoading;

            //Doesn't work
          /*  if (Application.isPlaying)
            {
                mergedScene = SceneManager.CreateScene(mergedSceneName);    
            } */
        }


        void Update()
        {
            foreach (PoolData i in poolDatas)
            {
                if (Time.time > i.time)
                {
                    Pool(i.gameObjectToPool);
                }
            }
        }

        void VerifyEndOfSceneLoading(Scene scene, LoadSceneMode mode)
        {
            if(scene.name.StartsWith(prefixPooledScene))
            {
                cptSceneLoading--;

                //SceneManager.MergeScenes(scene, mergedScene);

                if (cptSceneLoading == 0 && OnScenesLoadEnd != null)
                    OnScenesLoadEnd();
            }
        }

        public void Init()
        {
            poolDatas.Clear();
            pooledObjects.Clear();
            poolSceneInfoDictionnary.Clear();

            // Loop through the object prefabs and make a new list for each one.
            foreach (PoolGameObjectInfo poolObject in poolObjectList)
            {
                if (!pooledObjects.ContainsKey(poolObject.kind))
                {
                    pooledObjects.Add(poolObject.kind, new List<GameObject>(poolObject.bufferCount));

                    // Init or complete the object pool


                    for (int n = 0; n < poolObject.bufferCount; n++)
                    {
                        GameObject newObj = Instantiate(poolObject.prefab) as GameObject;
                        newObj.name = poolObject.kind;
                        Pool(newObj, poolObject.kind, poolObject.defaultParent);
                    }
                }
#if EQUILIBRE_GAMES_DEBUG
                else
                {
                    Debug.LogWarning("The same kind is referenced twice in the object pool : " + poolObject.kind);
                }
#endif
            }

            foreach (PoolSceneInfo poolScene in poolSceneList)
            {
                if (!poolSceneInfoDictionnary.ContainsKey(poolScene.kind))
                {
                    poolSceneInfoDictionnary.Add(poolScene.kind, poolScene);
                }
#if EQUILIBRE_GAMES_DEBUG
                else
                {
                    Debug.LogWarning("The same kind is referenced twice in the scene pool : " + poolScene.kind);
                }
#endif
            }
        }




        /// <summary>
        /// Returns the pooled object list for a specific kind. Object Pool manipulation purpose only
        /// </summary>
        /// <returns>The pooled object list.</returns>
        /// <param name="objectKind">Object kind.</param>
        public List<GameObject> GetPooledObjectList(string objectKind)
        {
            if (pooledObjects.ContainsKey(objectKind))
                return pooledObjects[objectKind];
            return null;
        }

        /// <summary>
        /// Gets a new object for the name type provided.  If no object of that type in the pool then <c>null</c> will be returned.
        /// </summary>
        /// <returns>
        /// The object for request prefab name.
        /// </returns>
        /// <param name='objectName'>
        /// Object prefab name
        /// </param>
        public GameObject GetFromPool(string objectName)
        {
            return GetFromPool(objectName, false);
        }


        /// <summary>
        /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
        /// then null will be returned.
        /// </summary>
        /// <returns>
        /// The object for type.
        /// </returns>
        /// <param name='objectType'>
        /// Object type.
        /// </param>
        /// <param name='onlyPooled'>
        /// If true, it will only return an object if there is one currently pooled.
        /// </param>
        public GameObject GetFromPool(string objectName, bool onlyPooled)
        {
            return GetFromPool(objectName, onlyPooled, true);
        }

        GameObject InstantiateObject(PoolGameObjectInfo poolObject)
        {
            GameObject newObj = Instantiate(poolObject.prefab) as GameObject;
            //newObj.transform.parent = (poolObject.defaultParent != null ? poolObject.defaultParent : transform);
            newObj.transform.SetParent(poolObject.defaultParent != null ? poolObject.defaultParent : transform);
            // Attach to object pool by default
            if (newObj.transform.parent == null)
            {
                //newObj.transform.parent = transform;
                newObj.transform.SetParent(transform);
            }
            newObj.transform.localScale = Vector3.one;
            newObj.name = poolObject.kind;
            return newObj;
        }


        /// <summary>
        /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
        /// then null will be returned.
        /// </summary>
        /// <returns>
        /// The object for type.
        /// </returns>
        /// <param name='objectType'>
        /// Object type.
        /// </param>
        /// <param name='onlyPooled'>
        /// If true, it will only return an object if there is one currently pooled.
        /// </param>
        public GameObject GetFromPool(string objectName, bool onlyPooled, bool activate)
        {
            PoolGameObjectInfo poolObject = poolObjectList.Find(element => objectName.StartsWith(element.kind));
            bool objectPrefabExists = (poolObject != null);

            // Don't get from pool while in editor and not playing
            if (!Application.isPlaying)
                return InstantiateObject(poolObject);

            if (objectPrefabExists)
            {
                // Get from the pool if there's one left
                if (pooledObjects.ContainsKey(objectName) && pooledObjects[objectName].Count > 0)
                {
                    if (pooledObjects[objectName][0] != null)
                    {
                        GameObject pooledObject = pooledObjects[objectName][0];

                        pooledObjects[objectName].RemoveAt(0);
                        pooledObject.SetActive(activate);
                        pooledObject.transform.SetParent(poolObject.defaultParent);
                        return pooledObject;
                    }

                    return InstantiateObject(poolObject);
                }
                else if (!onlyPooled)
                {
                    if (poolObject != null)
                    {
                        return InstantiateObject(poolObject);
                    }

                    return null;
                }
            }
            // If we have gotten here either there was no object of the specified type or none were left in the pool with onlyPooled set to true
            return null;
        }


        public void LoadScenes(string kind)
        {
            PoolSceneInfo poolSceneInfo = null;
            poolSceneInfoDictionnary.TryGetValue(kind, out poolSceneInfo);

            cptSceneLoading += poolSceneInfo.sceneInfos.Length;

            for (int j = 0; j < poolSceneInfo.sceneInfos.Length; j++)
            {
                for (int i = 0; i < poolSceneInfo.sceneInfos[j].bufferCount; i++)
                {
                    if (poolSceneInfo.sceneInfos[j].sceneNames.StartsWith(prefixPooledScene))
                        SceneManager.LoadSceneAsync(poolSceneInfo.sceneInfos[j].sceneNames, LoadSceneMode.Additive);
                    else
                        Debug.LogError("The scene doesn't start with : " + prefixPooledScene + " wich is not permit");
                }
            }
        }

        public void UnloadPooledMergedScene()
        {
            //Doesn't work
            /*SceneManager.UnloadScene(mergedScene);
            mergedScene = SceneManager.CreateScene(mergedSceneName);
            SceneManager.SetActiveScene(mergedScene);*/

           /* Scene[] scenes = SceneManager.GetAllScenes();

            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].name.StartsWith(prefixPooledScene))
                    SceneManager.UnloadScene(scenes[i]);
            } */

        }


        /// <summary>
        /// Create a new pool of object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefab"></param>
        /// <param name="tag"></param>
        /// <param name="bufferCount"></param>
        /// <param name="maxCount"></param>
        /// <param name="defaultParent"></param>
        public void NewPoolKind(string name, GameObject prefab, string tag = "Default", int bufferCount = 1, int maxCount = -1, Transform defaultParent = null)
        {
            PoolGameObjectInfo poolObjectKind = poolObjectList.Find(element => name.Equals(element.kind));
            if (poolObjectKind == null)
            {
#if EQUILIBRE_GAMES_DEBUG
                Debug.Log("Create a new object kind <" + name + ">");
#endif
                poolObjectKind = new PoolGameObjectInfo();
                poolObjectKind.bufferCount = bufferCount;
                // poolObjectKind.defaultParent = defaultParent;
                poolObjectKind.defaultParent.SetParent(defaultParent);
                poolObjectKind.kind = name;
                poolObjectKind.maxCount = maxCount;
                poolObjectKind.prefab = prefab;
                poolObjectKind.tag = tag;
                poolObjectList.Add(poolObjectKind);

                GameObject newObj = Instantiate(poolObjectKind.prefab) as GameObject;
                newObj.name = poolObjectKind.kind;
#if EQUILIBRE_GAMES_DEBUG
                Debug.Log("Object kind <" + name + "> created - Pool");
#endif
                Pool(newObj, poolObjectKind.kind, poolObjectKind.defaultParent);
            }
        }

        /// <summary>
        /// Pools the object specified.  Will not be pooled if there is no prefab of that type.
        /// </summary>
        /// <param name='obj'>
        /// Object to be pooled.
        /// </param>
        /// <returns>
        /// true if object could be pooled, false otherwise
        /// </returns>
        public bool Pool(GameObject obj, string objectKind = null, Transform poolParent = null)
        {

            if (obj == null)
                return false;

            if (objectKind == null)
                objectKind = obj.name;

            PoolGameObjectInfo poolObjectKind = poolObjectList.Find(element => objectKind.StartsWith(element.kind));

            if (poolObjectKind != null)
            {
                bool objectPrefabListExists = pooledObjects.ContainsKey(poolObjectKind.kind);
                // Create a new list for this kind of pooled objects (only known ones)
                if (!objectPrefabListExists)
                    pooledObjects.Add(poolObjectKind.kind, new List<GameObject>(1));
            }
            // reset its velocity if not kinematic
            if (obj.GetComponent<Rigidbody>() && !obj.GetComponent<Rigidbody>().isKinematic)
            {
                obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            // Never pool object in the editor, while not playing
            if (!Application.isPlaying)
                DestroyImmediate(obj);
            else
            {
                if (poolObjectKind == null || pooledObjects[poolObjectKind.kind].Count > poolObjectKind.maxCount)
                {
#if EQUILIBRE_GAMES_DEBUG
                    Debug.Log("Destroying " + (poolObjectKind == null ? obj.name : (poolObjectKind.kind + " : " + pooledObjects[poolObjectKind.kind].Count + " / " + poolObjectKind.maxCount)));
#endif
                    // Destroy an object that can't be pooled
                    Destroy(obj);
                }
                else
                {
                    // Deactivate the object, reset its position and scale
                    obj.SetActive(false);
                    // - set default parent or requested one
                    // obj.transform.parent = poolObjectKind.defaultParent;
                    obj.transform.SetParent(poolObjectKind.defaultParent);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    // Put the object in the object pool
                    pooledObjects[poolObjectKind.kind].Insert(0, obj);
                }
            }
            return true;
        }

        public void PoolFromTag(string tag)
        {
            PoolGameObjectList(GameObject.FindGameObjectsWithTag(tag));
        }
        public void PoolGameObjectList(GameObject[] goList)
        {
            for (int i = 0; i < goList.Length; i++)
            {
                Pool(goList[i], null, null);
            }
        }

        public void Pool(GameObject obj, float time)
        {
            poolDatas.Add(new PoolData(obj, Time.time + time));
        }



#if UNITY_EDITOR
        [ContextMenu("Pool inactive objects")]
        public void PoolInactiveObjects()
        {
            for (int i = 0; i < poolObjectList.Count; i++)
            {
                PoolInactiveObjects(poolObjectList[i].kind, poolObjectList[i].defaultParent);
            }
        }

        void PoolInactiveObjects(string namePrefix, Transform t)
        {
            if (t == null)
                return;
            // Recursively pool children first
            if (t.childCount > 0)
            {
                foreach (Transform child in t)
                    PoolInactiveObjects(namePrefix, child);
            }
            // Then, if game object should be pooled (from its tag), Pool it
            if (t.gameObject.name.StartsWith(namePrefix))
            {
                Pool(t.gameObject);
            }
        }
#endif
    }
}