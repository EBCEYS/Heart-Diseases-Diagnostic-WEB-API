using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Get_Requests_From_Client_For_Project_Test.DataSetsClasses
{
    /// <summary>
    /// Template for element from Cleveland dataset.
    /// </summary>
    public class ClevelandDataSet
    {
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public long Age { get; set; }
        /// <summary>
        /// Gets or sets the sex.
        /// </summary>
        /// <value>
        /// The sex. If male <c>true</c>; otherwise <c>false</c>.
        /// </value>
        public bool Sex { get; set; }
        /// <summary>
        /// Gets or sets the maximum heart rate.
        /// </summary>
        /// <value>
        /// The maximum heart rate.
        /// </value>
        public long MaxHeartRate { get; set; }

        internal object ToObject()
        {
            return new { Age, Sex, MaxHeartRate };
        }
    }
}
