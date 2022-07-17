using System;
using System.Collections.Generic;
using System.Text;

namespace Fsm_Generator.DataObjects
{
    public class StateDto : BaseDto
    {
        public String StateName { get; set; }
        public int Timeout { get; set; }

        public String? OnEntryAction { get; set; }
        public String? OnExitAction { get; set; }

        public List<DescriptionFragment> OnEntryDescription { get; set; }
        public List<DescriptionFragment> OnExitDescription { get; set; }

        public StateDto()
        {
            OnEntryDescription = new List<DescriptionFragment>();
            OnExitDescription = new List<DescriptionFragment>();
            StateName = "Default State Name";
        }
    }
}
