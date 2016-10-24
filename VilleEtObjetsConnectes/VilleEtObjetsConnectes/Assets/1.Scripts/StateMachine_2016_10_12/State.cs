using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace EquilibreGames
{
    /* Only One state have to be Active at the same time */
    public abstract class State : MonoBehaviour
    {


        [HideInInspector]
        public StateMachine stateMachine;

        [SerializeField]
        string named;
        public string Named
        {
            get { return named; }
            set { named = value; }
        }

        [SerializeField]
        List<SubState> subStatesList;

        string lastInfo = "";
        public string LastInfo
        {
            get{ return lastInfo; }
        }

        /// <summary>
        /// Function called when the state is enabled. The parameter is filled with the previous state name.
        /// </summary>
        /// <param name="previousState"></param>
        public virtual void OnActivation(string previousState, string info = "")
        {
            lastInfo = info;
        }
        /// <summary>
        /// Function called when the state is disabled. The parameter is filled with the next state name.
        /// </summary>
        /// <param name="nextState"></param>
        public virtual void OnDesactivation(string nextState, string info = "")
        {
            lastInfo = info;
        }



        public T GetSubstate<T>() where T : SubState
        {
            SubState sub = null;


            foreach (SubState i in subStatesList)
            {

                if (i.GetType().Equals(typeof(T)))
                {
                    sub = i;
                    break;
                }
            }

            if (sub != null)
            {
                object send = sub;
                send = System.Convert.ChangeType(send, typeof(T));

                return (T)send;
            }
            else
            {
                return default(T);
            }
        }


        public SubState GetSubstate(string substateName)
        {
            SubState sub = null;

            foreach (SubState i in subStatesList)
            {
                if (i.GetType().ToString().Equals(substateName))
                {
                    sub = i;
                    break;
                }
            }
            return sub;
        }

    }



}

