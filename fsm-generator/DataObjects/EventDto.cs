using System;
using System.Collections.Generic;
using System.Text;

namespace Fsm_Generator.DataObjects
{
    /// <summary>
    /// This model assumes that if the event exists multiple
    /// times on a diagram that it always targets the same
    /// state.
    /// </summary>
    public class EventDto : BaseDto
    {
        public String EventName { get; set; }

        public int Index { get; set; }

        // All the States that are valid sources for the event
        public List<StateDto> SourceStates { get; set; }

        // The target state for the event
        public StateDto TargetState { get; set; }
        public String TargetStateName
        {
            get { return TargetState.StateName; }
        }

        /// <summary>
        /// Combined descriptions of all transitions using this event
        /// </summary>
        /// <value></value>
        public List<DescriptionFragment> TransitionDescriptions { get; set; }

        public EventDto()
        {
            TransitionDescriptions = new List<DescriptionFragment>();
            SourceStates = new List<StateDto>();
            TargetState = new StateDto();
            EventName = "Default Event Name";
        }
    }
}
