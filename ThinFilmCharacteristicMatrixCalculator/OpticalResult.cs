namespace ThinFilmCharacteristicMatrixCalculator
{
    /// <summary>
    /// 光學計算結果，支援單一偏振和AVG偏振模式
    /// </summary>
    public class OpticalResult
    {
        public double Reflectance { get; set; }
        public double Transmittance { get; set; }
        public double Absorbance { get; set; }
        public Complex ReflectionCoefficient { get; set; }
        public Complex TransmissionCoefficient { get; set; }
        public double Wavelength { get; set; }
        public double IncidentAngle { get; set; }
        public bool IsEnergyConserved { get; set; }
        public PolarizationType PolarizationType { get; set; }
        
        // AVG偏振模式的詳細結果
        public OpticalResult? SPolarizationResult { get; set; }
        public OpticalResult? PPolarizationResult { get; set; }
        
        public OpticalResult()
        {
            ReflectionCoefficient = new Complex(0, 0);
            TransmissionCoefficient = new Complex(0, 0);
            PolarizationType = PolarizationType.S;
        }
        
        /// <summary>
        /// 檢查是否為AVG偏振模式
        /// </summary>
        public bool IsAVGPolarization => PolarizationType == PolarizationType.AVG;
        
        /// <summary>
        /// 取得格式化的偏振類型描述
        /// </summary>
        public string PolarizationDescription => PolarizationType switch
        {
            PolarizationType.S => "S偏振",
            PolarizationType.P => "P偏振", 
            PolarizationType.AVG => "AVG偏振 (S+P平均)",
            _ => "未知"
        };
    }
    
    /// <summary>
    /// 偏振類型枚舉
    /// </summary>
    public enum PolarizationType
    {
        S,      // S偏振
        P,      // P偏振
        AVG     // AVG偏振（S和P的平均值）
    }
}