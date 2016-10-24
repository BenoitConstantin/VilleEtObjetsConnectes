using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Used By state, you can have more than one subState active */

namespace EquilibreGames
{

    public class SubState : MonoBehaviour
    {
        public virtual void OnActivation(string info = "")
        {

        }

        public virtual void OnDesactivation()
        {

        }
    }
}
