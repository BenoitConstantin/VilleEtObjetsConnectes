using UnityEngine;
using System.Collections;


namespace EquilibreGames
{
    /// <summary>
    /// Transition class only here to minimize networking sending for transition behaviour.
    /// It's work like a state but automaticaly go on the nextState without calling networking.
    /// </summary>
    public abstract class StateTransition : MonoBehaviour
    {

        [SerializeField]
        string named;
        public string Named
        {
            get { return named; }
            set { named = value; }
        }

        [SerializeField]
        State nextState;

        [HideInInspector]
        public StateMachine stateMachine;

        [SerializeField]
        float timeBeforeTransition = 0;

        public void Transit()
        {
            OnTransition();

            Invoke("ChangeState", timeBeforeTransition);
        }


        /// <summary>
        /// Code run before a transition
        /// </summary>
        protected abstract void OnTransition();


        private void ChangeState()
        {
            stateMachine.LocalChangeState(nextState.Named);
        }
    }
}
