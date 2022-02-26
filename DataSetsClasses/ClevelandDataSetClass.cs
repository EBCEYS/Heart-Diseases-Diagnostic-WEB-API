namespace Get_Requests_From_Client_For_Project_Test.DataSetsClasses
{
    /// <summary>
    /// Template for element from Cleveland dataset.
    /// </summary>
    public class ClevelandDataSet : DataSetBase
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
        /// Gets or sets the chest pain type.
        /// </summary>
        /// <value>
        /// The chest pain type.
        /// </value>
        public long ChestPainType { get; set; }
        /// <summary>
        /// Gets or sets the resting blood pressure.
        /// </summary>
        /// <value>
        /// The resting blood pressure.
        /// </value>
        public long RestingBloodPressure { get; set; }
        /// <summary>
        /// Gets or sets the serum cholestoral.
        /// </summary>
        /// <value>
        /// The serum cholestoral.
        /// </value>
        public long SerumCholestoral { get; set; }
        /// <summary>
        /// Gets or sets the fasting blood sugar.
        /// </summary>
        /// <value>
        /// The fasting blood sugar.
        /// </value>
        public bool FastingBloodSugar { get; set; }
        /// <summary>
        /// Gets or sets the resting electrocardiographic results.
        /// </summary>
        /// <value>
        /// The resting electrocardiographic results.
        /// </value>
        public long RestingElectrocardiographicResults { get; set; }
        /// <summary>
        /// Gets or sets the maximum heart rate achieved.
        /// </summary>
        /// <value>
        /// The maximum heart rate achieved.
        /// </value>
        public long MaximumHeartRateAchieved { get; set; }
        /// <summary>
        /// Gets or sets the exercise induced angina.
        /// </summary>
        /// <value>
        /// The exercise induced angina.
        /// </value>
        public bool ExerciseInducedAngina { get; set; }
        /// <summary>
        /// Gets or sets the ST depression.
        /// </summary>
        /// <value>
        /// The ST depression.
        /// </value>
        public double STDepression { get; set; }
        /// <summary>
        /// Gets or sets the ST slope.
        /// </summary>
        /// <value>
        /// The ST slope.
        /// </value>
        public long STSlope { get; set; }
        /// <summary>
        /// Gets or sets the number of major vessels.
        /// </summary>
        /// <value>
        /// The number of major vessels.
        /// </value>
        public long NumberOfMajorvessels { get; set; }
        /// <summary>
        /// Gets or sets the thalassemia.
        /// </summary>
        /// <value>
        /// The thalassemia.
        /// </value>
        public long Thalassemia { get; set; }
        /// <summary>
        /// The Cleveland data set class constructor.
        /// </summary>
        public ClevelandDataSet()
        {
            SetAlghorithmType(DataSetTypes.Cleveland);
        }
    }
}
