namespace ThinFilmCharacteristicMatrixCalculator
{
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
        
        public OpticalResult()
        {
            ReflectionCoefficient = new Complex(0, 0);
            TransmissionCoefficient = new Complex(0, 0);
        }
    }
}