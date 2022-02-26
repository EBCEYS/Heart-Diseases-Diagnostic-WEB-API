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
        public DataSetTypes DataSetType
        {
            get
            {
                return dataSetType;
            }
        }
        private DataSetTypes dataSetType;
        /// <summary>
        /// Method uses to set dataset type.
        /// </summary>
        /// <param name="dataSetType">The dataset type.</param>
        internal void SetAlghorithmType(DataSetTypes dataSetType)
        {
            this.dataSetType = dataSetType;
        }
        /// <summary>
        /// Method uses to convert DataSet class to object type.
        /// </summary>
        /// <returns>The object.</returns>
        internal object ToObject()
        {
            return (object)this;
        }
    }
}
