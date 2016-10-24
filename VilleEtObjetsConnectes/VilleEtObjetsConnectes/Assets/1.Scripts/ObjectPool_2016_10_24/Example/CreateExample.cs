using UnityEngine;
using System.Collections;

public class CreateExample : MonoBehaviour {


    [SerializeField]
    string exampleName;

    public void PoolExample()
    {
        EquilibreGames.ObjectPool.Instance.GetFromPool(exampleName);
    }

}
