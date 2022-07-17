using System;
using System.Collections.Generic;
using System.Text;

namespace Fsm_Generator.DataObjects
{
    public class BaseDto
    {
        public List<DescriptionFragment> Description { get; set; }

        public BaseDto()
        {
            Description = new List<DescriptionFragment>();
        }

        /// <summary>
        /// If comment is not empty or null it's
        /// added to the description collection.
        /// </summary>
        /// <param name="comment"></param>
        public void AddCommentLine(String comment)
        {
            if (String.IsNullOrWhiteSpace(comment))
                return;

            DescriptionFragment df = new DescriptionFragment
            {
                DescriptionLine = comment
            };
            Description.Add(df);
        }
    }
}
