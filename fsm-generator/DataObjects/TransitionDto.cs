using System;
using System.Collections.Generic;
using System.Text;

namespace Fsm_Generator.DataObjects
{
    /// <summary>
    /// Attributes for state transitions
    /// </summary>
    public class TransitionDto : BaseDto
    {
        public String StartStateName { get; set; }
        public String EndStateName { get; set; }

        public String EventName { get; set; }


        public String? Guard { get; set; }

        public String? Effect { get; set; }

        public TransitionDto()
        {
            StartStateName = "";
            EndStateName = "";
            EventName = "";
        }
    }
}
