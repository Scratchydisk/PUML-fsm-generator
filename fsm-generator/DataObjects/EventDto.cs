using System;
using System.Collections.Generic;
using System.Text;

namespace Fsm_Generator.DataObjects
{
    public class EventDto : BaseDto
    {
        public String EventName { get; set; }

        public int Index { get; set; }

        public EventDto()
        {
            EventName = "Default Event Name";
        }
    }
}
