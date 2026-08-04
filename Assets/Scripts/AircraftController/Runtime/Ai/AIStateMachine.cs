using System.Collections.Generic;

namespace AircraftController
{
    namespace AircraftAI
    {
        public class AIStateMachine
        {
            public AIState currentState { get; private set; }

            private Dictionary<AIState, List<AITransition>> transitionsByState = new Dictionary<AIState, List<AITransition>>();
            private static readonly List<AITransition> noTransitions = new List<AITransition>();

            public void Initialize(AIState initState)
            {
                currentState = initState;
                currentState.Enter();
            }

            /// <summary>
            /// Registers a transition that is evaluated while <paramref name="from"/> is the current state.
            /// Transitions are evaluated in the order they are added, so the one added first wins.
            /// </summary>
            public void AddTransition(AIState from, AITransition transition)
            {
                List<AITransition> transitions;
                if (transitionsByState.TryGetValue(from, out transitions) == false)
                {
                    transitions = new List<AITransition>();
                    transitionsByState.Add(from, transitions);
                }

                transitions.Add(transition);
            }

            public void ChangeState(AIState newState)
            {
                currentState?.Exit();

                currentState = newState;
                currentState.Enter();
            }

            /// <summary>
            /// Updates the current state and then takes the first triggered transition out of it.
            /// The transitions are evaluated after the update, so the state always gets to act
            /// on the current situation before it is left.
            /// </summary>
            public void Update(float simulationDeltaTime)
            {
                currentState.Update(simulationDeltaTime);

                AITransition triggeredTransition = GetTriggeredTransition();
                if (triggeredTransition != null)
                {
                    ChangeState(triggeredTransition.To);
                }
            }

            private AITransition GetTriggeredTransition()
            {
                List<AITransition> transitions = GetTransitions(currentState);
                for (int i = 0; i < transitions.Count; i++)
                {
                    if (transitions[i].IsTriggered())
                    {
                        return transitions[i];
                    }
                }

                return null;
            }

            private List<AITransition> GetTransitions(AIState state)
            {
                List<AITransition> transitions;
                if (transitionsByState.TryGetValue(state, out transitions))
                {
                    return transitions;
                }

                return noTransitions;
            }
        }
    }
}
