using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fsm_Generator.DataObjects
{
    /// <summary>
    /// Metamodel created by parser and used by code generator
    /// </summary>
    public class ModelData
    {
        /// <summary>
        /// The command line parameters
        /// </summary>
        /// <value></value>
        public ProgramMetadata? Metadata { get; set; }

        /// <summary>
        /// Section Lambda functions
        /// </summary>
        /// <value></value>
        public StubbleFuncs? F {get;set;}

        // Known events that the framework takes care of
        public List<String> SystemEventNames = new List<string>()
            { "EVENT_START", "EVENT_TIMEOUT", "EVENT_ONENTRY", "EVENT_ONEXIT" };

        public String DiagramName { get; set; }

        public String GeneratedOn
        {
            get { return DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString(); }
        }

        /// <summary>
        /// Diagram name with spaces removed
        /// </summary>
        public String Namespace
        {
            get { return DiagramName.Replace(" ", ""); }
        }

        public List<TransitionDto> Transitions { get; set; }
        public List<StateDto> States { get; set; }

        /// <summary>
        /// All events, including system ones (start, timeout)
        /// </summary>
        public List<EventDto> Events { get; set; }
        /// <summary>
        /// All non-system events
        /// </summary>
        public List<EventDto> UserEvents { get; set; }

        public ModelData()
        {
            Transitions = new List<TransitionDto>();
            States = new List<StateDto>();
            Events = new List<EventDto>();
            UserEvents = new List<EventDto>();
            DiagramName = "Default Diagram Name";
            Events = new List<EventDto>();
            States = new List<StateDto>();
        }

        /// <summary>
        /// Adds a new state or merges into an existing state.
        /// </summary>
        /// <param name="state"></param>
        public StateDto AddMergeState(StateDto state)
        {
            // Don't store duplicate states
            StateDto? existingState = States.FirstOrDefault(p => p.StateName == state.StateName);
            if (existingState == null)
            {
                existingState = state;
                States.Add(existingState);
            }

            // Descriptions can only be specified on state creation
            // using "state" keyword, so no need to handle them here.

            existingState.Timeout = state.Timeout;

            // Make sure parser keeps reference to stored instance of the state
            return existingState;
        }

        public TransitionDto AddMergeTransition(TransitionDto transition)
        {
            TransitionDto? existingTransition = Transitions
                .FirstOrDefault(p => p.StartStateName == transition.StartStateName
                    && p.EndStateName == transition.EndStateName
                    && p.EventName == transition.EventName);

            if (existingTransition == null)
            {
                existingTransition = transition;
                Transitions.Add(existingTransition);
            }

            return existingTransition;
        }

        public void AddMergeEvent(EventDto dtoEvent)
        {
            EventDto? existingEvent = Events
                .FirstOrDefault(p => p.EventName == dtoEvent.EventName);

            if (existingEvent == null)
            {
                existingEvent = dtoEvent;
                Events.Add(existingEvent);

                if (!SystemEventNames.Contains(existingEvent.EventName)
                    && !UserEvents.Any(p => p.EventName == existingEvent.EventName))
                {
                    UserEvents.Add(existingEvent);
                    existingEvent.Index = UserEvents.Count;
                }
            }
            existingEvent.Description = dtoEvent.Description;
        }
    }
}
