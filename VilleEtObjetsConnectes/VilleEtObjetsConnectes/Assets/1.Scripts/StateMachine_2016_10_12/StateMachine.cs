using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace EquilibreGames
{
    public class StateMachine : MonoBehaviour
    {

        /* A char array for automatic name state */

        [SerializeField]
        bool useAutomaticNaming = true;

        [SerializeField]
        char[] stateNameSplit = { '_' };

        [SerializeField]
        List<State> statesList;

        [SerializeField]
        State initialState;

        [SerializeField]
        State anyState;


        [SerializeField]
        int historicLength = 0;

        [Space(10)]
        [SerializeField]
        List<SubState> subStatesAvailablesForAllStates;

        public delegate void ChangeStateDelegate(string s, string info = "", bool historic = false);
        /// <summary>
        /// Change state and give you the possibility to save this on an historic
        /// </summary>
        public ChangeStateDelegate ChangeState;

        public Action ReturnToPreviousState;

        public delegate bool ChangeSubStateDelegate(string s, string info = "");

        public ChangeSubStateDelegate ActiveSubstateInCurrentState;



        private State currentState;
        public State CurrentState
        {
            get { return currentState; }
        }

        private string previousState;
        public string PreviousState
        {
            get { return previousState; }
        }


        private Dictionary<string, State> statesDictionnary;


        /// <summary>
        /// Keep a State Historic
        /// </summary>
        private State[] historic;
        /// <summary>
        /// The index in the state historic
        /// </summary>
        private int currentHistoricIndex = 0;
        /// <summary>
        /// The deep of the current historic, to not loop on the historic
        /// </summary>
        private int historicDeep = 0;


        private void Awake()
        {
            historic = new State[historicLength];

            ChangeState = LocalChangeState;
            ReturnToPreviousState = LocalReturnToPreviousState;
            ActiveSubstateInCurrentState = LocalActiveSubstateInCurrentState;

            statesDictionnary = new Dictionary<string, State>();



            if (useAutomaticNaming)
            {
                foreach (State i in statesList)
                {
                    i.Named = i.GetType().Name.Split(stateNameSplit)[1];
                }
            }


            foreach (State i in statesList)
            {
                i.enabled = false;
                statesDictionnary.Add(i.Named, i);
                i.stateMachine = this;
            }

        }

        void Start()
        {
            if (currentState == null)
            {
                currentState = initialState;

                currentState.enabled = true;
                currentState.OnActivation(null);

                previousState = currentState.Named;
            }

            if (anyState != null)
            {
                anyState.stateMachine = this;
                anyState.enabled = true;
                anyState.OnActivation(null);
            }
        }

        public void LocalChangeState(string nextState, string info = "", bool historicSave = true)
        {
            if (currentState != null && currentState.Named == nextState)
                return;

            State findState;
            statesDictionnary.TryGetValue(nextState, out findState);

            if (findState != null)
            {
                previousState = currentState.Named;

                if (historicSave && historicLength != 0)
                {
                    historic[currentHistoricIndex] = currentState;
                    currentHistoricIndex = (currentHistoricIndex + 1) % historicLength;
                    historicDeep = historicDeep > historicLength ? historicDeep : historicDeep + 1;
                }

                currentState.OnDesactivation(nextState, info);
                currentState.enabled = false;

                currentState = findState;

                currentState.enabled = true;
                currentState.OnActivation(previousState, info);
            }
            else
            {
#if EQUILIBRE_GAMES_DEBUG
                Debug.LogWarning("StateNotFound : " + nextState);
#endif
            }
        }


        public void LocalReturnToPreviousState()
        {
            if (currentState.Named == previousState)
                return;

            State findState;
            statesDictionnary.TryGetValue(previousState, out findState);

            if (historicLength != 0 && historicDeep != 0)
            {
                if (currentHistoricIndex != 0)
                {
                    currentHistoricIndex--;
                    previousState = historic[currentHistoricIndex].Named;
                }
                else
                {
                    currentHistoricIndex = historicLength - 1;
                    previousState = historic[currentHistoricIndex].Named;
                }

                historicDeep--;
                if (historicDeep < 0)
                    historicDeep = 0;
            }

            currentState.enabled = false;
            currentState.OnDesactivation(previousState);

            State memory = currentState;
            currentState = findState;

            currentState.enabled = true;
            currentState.OnActivation(memory.name);
        }


        public T GetSubstateInCurrentState<T>() where T : SubState
        {

            foreach (SubState i in subStatesAvailablesForAllStates)
            {

                if (i.GetType().Equals(typeof(T)))
                {
                    object send = i;
                    send = System.Convert.ChangeType(send, typeof(T));

                    return (T)send;
                }
            }

            return currentState.GetSubstate<T>();
        }

        /// <summary>
        /// Active a subState in the current state and return if it has be done (the substate was found)
        /// </summary>
        /// <param name="subStateName"></param>
        /// <returns></returns>
        public bool LocalActiveSubstateInCurrentState(string subStateName, string info = "")
        {
            SubState sub = currentState.GetSubstate(subStateName);


            foreach (SubState i in subStatesAvailablesForAllStates)
            {
                if (i.GetType().ToString().Equals(subStateName))
                {
                    sub = i;
                    break;
                }
            }

            if (sub != null)
            {
                sub.enabled = true;
                sub.OnActivation(info);
                return true;
            }
            else
            {

                sub = currentState.GetSubstate(subStateName);
                if (sub != null)
                {
                    sub.enabled = true;
                    sub.OnActivation(info);
                    return true;
                }
                else
                {
#if EQUILIBRE_GAMES_DEBUG
                    Debug.LogWarning("Substate not found on : " + currentState.Named);
#endif
                    return false;
                }
            }

        }
    }
}
