using System.Collections.Generic;
using System.Reflection;

namespace Get_Requests_From_Client_For_Project_Test.DataSetsClasses
{
    /// <summary>
    /// This class uses as parent for dataSets classes.
    /// </summary>
    public class DataSetBase
    {
        /// <summary>
        /// Gets the dataset type.
        /// </summary>
        /// <value>
        /// The dataset type.
        /// </value>
        public DataSetTypes? DataSetType
        {
            get
            {
                return dataSetType;
            }
        }
        private DataSetTypes? dataSetType;
        /// <summary>
        /// Method uses to set dataset type.
        /// </summary>
        /// <param name="dataSetType">The dataset type.</param>
        internal void SetAlghorithmType(DataSetTypes? dataSetType)
        {
            this.dataSetType = dataSetType;
        }
        /// <summary>
        /// Method checks for attributes were not null.
        /// </summary>
        /// <param name="nullProps">The out list of properties with null value.</param>
        /// <returns><c>true</c> if OK; otherwise <c>false</c></returns>
        public bool CheckAttributes(out List<string> nullProps)
        {
            PropertyInfo[] props = this.GetType().GetProperties();
            nullProps = new();
            foreach(PropertyInfo prop in props)
            {
                if (prop.GetValue(this) == null)
                {
                    nullProps.Add(prop.Name);
                }
            }
            return nullProps.Count == 0;
        }
    }
}
