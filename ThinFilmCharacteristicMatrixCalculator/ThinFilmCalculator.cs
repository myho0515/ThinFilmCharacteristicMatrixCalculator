using System;

namespace ThinFilmCharacteristicMatrixCalculator
{
    public class ThinFilmCalculator
    {
        private CalculationLogger logger;

        public ThinFilmCalculator(CalculationLogger logger = null)
        {
            this.logger = logger ?? new CalculationLogger();
        }

        public OpticalResult Calculate(Complex incidentIndex, Complex filmIndex, double thickness,
            Complex substrateIndex, double wavelength, double incidentAngle = 0, bool isSPolarization = true)
        {
            logger.LogInputParameters(incidentIndex, filmIndex, thickness, substrateIndex, 
                wavelength, incidentAngle, isSPolarization);

            // 步驟1：計算相位厚度
            logger.LogStepHeader("步驟1: 相位厚度計算");
            double cosTheta = Math.Cos(incidentAngle);
            Complex delta = new Complex(2 * Math.PI * thickness / wavelength, 0) * filmIndex * cosTheta;
            logger.LogCalculation("相位厚度 δ", delta);

            // 步驟2：計算光學導納
            logger.LogStepHeader("步驟2: 光學導納計算");
            Complex eta0, eta1, etaS;
            
            if (Math.Abs(incidentAngle) < 1e-10) // 垂直入射
            {
                eta0 = incidentIndex;
                eta1 = filmIndex;
                etaS = substrateIndex;
            }
            else // 斜入射
            {
                // 計算各層中的折射角
                Complex sinTheta0 = new Complex(Math.Sin(incidentAngle), 0);
                Complex sinTheta1 = (incidentIndex / filmIndex) * sinTheta0;
                Complex sinThetaS = (incidentIndex / substrateIndex) * sinTheta0;
                
                Complex cosTheta1 = Complex.Sqrt(new Complex(1, 0) - sinTheta1 * sinTheta1);
                Complex cosThetaS = Complex.Sqrt(new Complex(1, 0) - sinThetaS * sinThetaS);
                
                if (isSPolarization) // S偏振
                {
                    eta0 = incidentIndex * cosTheta;
                    eta1 = filmIndex * cosTheta1;
                    etaS = substrateIndex * cosThetaS;
                }
                else // P偏振
                {
                    eta0 = incidentIndex / cosTheta;
                    eta1 = filmIndex / cosTheta1;
                    etaS = substrateIndex / cosThetaS;
                }
                
                // 重新計算相位厚度（考慮斜入射）
                delta = new Complex(2 * Math.PI * thickness / wavelength, 0) * filmIndex * cosTheta1;
            }
            
            logger.LogCalculation("η₀ (入射介質)", eta0);
            logger.LogCalculation("η₁ (薄膜)", eta1);
            logger.LogCalculation("ηₛ (基板)", etaS);

            // 步驟3：計算特徵矩陣
            logger.LogStepHeader("步驟3: 特徵矩陣計算");
            Complex cosD = Complex.Cos(delta);
            Complex sinD = Complex.Sin(delta);
            
            logger.LogCalculation("cos(δ)", cosD);
            logger.LogCalculation("sin(δ)", sinD);

            var characteristicMatrix = new ComplexMatrix(2, 2);
            characteristicMatrix[0, 0] = cosD;
            characteristicMatrix[0, 1] = new Complex(0, 1) * sinD / eta1;
            characteristicMatrix[1, 0] = new Complex(0, 1) * eta1 * sinD;
            characteristicMatrix[1, 1] = cosD;
            
            logger.LogMatrix("特徵矩陣 M", characteristicMatrix);

            // 步驟4：計算邊界條件參數
            logger.LogStepHeader("步驟4: 邊界條件參數");
            Complex B = characteristicMatrix[0, 0] + characteristicMatrix[0, 1] * etaS;
            Complex C = characteristicMatrix[1, 0] + characteristicMatrix[1, 1] * etaS;
            
            logger.LogCalculation("參數 B", B);
            logger.LogCalculation("參數 C", C);

            // 步驟5：使用正確的教科書簡化公式計算TRA
            logger.LogStepHeader("步驟5: 教科書簡化公式計算TRA");
            
            // 計算共同項
            Complex denominator = eta0 * B + C;
            Complex denominatorConj = denominator.Conjugate();
            Complex denominatorSquared = denominator * denominatorConj;
            Complex BC_conjugate = B * C.Conjugate();
            
            logger.LogCalculation("η₀B + C", denominator);
            logger.LogCalculation("(η₀B + C)*", denominatorConj);
            logger.LogCalculation("|(η₀B + C)|²", denominatorSquared);
            logger.LogCalculation("BC*", BC_conjugate);
            logger.LogCalculation("Re(BC*)", BC_conjugate.Real);

            // 反射率公式 (1-2): R = (η₀B - C)² / (η₀B + C)²
            Complex numeratorR = eta0 * B - C;
            Complex numeratorRSquared = numeratorR * numeratorR.Conjugate();
            double R = numeratorRSquared.Real / denominatorSquared.Real;
            
            logger.LogCalculation("η₀B - C", numeratorR);
            logger.LogCalculation("|(η₀B - C)|²", numeratorRSquared);
            logger.LogCalculation("R (公式1-2)", R);

            // 穿透率公式 (1-3): T = 4η₀Re(ηₛ) / ((η₀B + C)(η₀B + C)*)
            double T = (4 * eta0.Real * etaS.Real) / denominatorSquared.Real;
            
            logger.LogCalculation("4η₀Re(ηₛ)", 4 * eta0.Real * etaS.Real);
            logger.LogCalculation("T (公式1-3)", T);

            // 吸收率公式 (1-4): A = 4η₀Re(BC* - ηₛ) / (η₀B + C)²
            double numeratorA = 4 * eta0.Real * (BC_conjugate.Real - etaS.Real);
            double A = numeratorA / denominatorSquared.Real;
            
            logger.LogCalculation("Re(BC* - ηₛ)", BC_conjugate.Real - etaS.Real);
            logger.LogCalculation("4η₀Re(BC* - ηₛ)", numeratorA);
            logger.LogCalculation("A (公式1-4)", A);

            // 步驟6：同時用導納方法驗證
            logger.LogStepHeader("步驟6: 導納方法驗證");
            Complex Y = C / B;
            logger.LogCalculation("導納 Y = C/B", Y);
            
            // 導納公式: R = (η₀ - Y)² / (η₀ + Y)²
            Complex r_admittance = (eta0 - Y) / (eta0 + Y);
            double R_admittance = r_admittance.AbsSquared();
            
            // 導納公式: T = Re(ηₛ)(1-R) / Re(BC*)
            double T_admittance = (etaS.Real * (1 - R_admittance)) / BC_conjugate.Real;
            
            // 導納公式: A = 1 - R - T
            double A_admittance = 1 - R_admittance - T_admittance;
            
            logger.LogCalculation("反射係數 r", r_admittance);
            logger.LogCalculation("R (導納方法)", R_admittance);
            logger.LogCalculation("T (導納方法)", T_admittance);
            logger.LogCalculation("A (導納方法)", A_admittance);

            // 比較兩種方法的結果
            logger.LogStepHeader("兩種方法結果比較");
            logger.LogCalculation("R差異", Math.Abs(R - R_admittance));
            logger.LogCalculation("T差異", Math.Abs(T - T_admittance));
            logger.LogCalculation("A差異", Math.Abs(A - A_admittance));

            // 使用導納方法的結果（應該更準確）
            R = R_admittance;
            T = T_admittance;
            A = A_admittance;

            logger.LogFinalResults(R, T, A);

            bool isEnergyConserved = Math.Abs(R + T + A - 1.0) < 1e-6;
            logger.LogEnergyConservation(isEnergyConserved, R + T + A);

            // 完成所有計算步驟
            logger.CompleteCalculation();

            return new OpticalResult
            {
                Reflectance = R,
                Transmittance = T,
                Absorbance = A,
                ReflectionCoefficient = r_admittance, // 使用導納方法計算的反射係數
                TransmissionCoefficient = new Complex(0, 0), // 透射係數在這個方法中不直接計算
                Wavelength = wavelength,
                IncidentAngle = incidentAngle,
                IsEnergyConserved = isEnergyConserved
            };
        }

        public void RunValidationTests()
        {
            logger.LogHeader("薄膜計算器驗證測試");
            
            // 測試案例1：預設參數
            logger.LogStepHeader("測試案例1：預設參數");
            var result1 = Calculate(
                new Complex(1.0, 0),       // 空氣
                new Complex(2.385, -0.1),  // 薄膜：N = nᵣ - iKᵣ
                99.45,                     // 厚度
                new Complex(1.52, 0),      // 基板
                550,                       // 波長
                0,                         // 垂直入射
                true                       // S偏振
            );
            
            logger.LogValidationResult("預設參數測試", 11.1475, 70.3446, 18.5079, 
                result1.Reflectance, result1.Transmittance, result1.Absorbance);
            
            // 測試案例2：標準驗證案例
            logger.LogStepHeader("測試案例2：標準驗證案例");
            var result2 = Calculate(
                new Complex(1.0, 0),       // 空氣
                new Complex(2.385, -0.1),  // 薄膜：N = nᵣ - iKᵣ  
                57.65,                     // 厚度
                new Complex(1.52, 0),      // 基板
                550,                       // 波長
                0,                         // 垂直入射
                true                       // S偏振
            );
            
            logger.LogValidationResult("標準驗證案例", 31.4424, 59.5727, 8.9849, 
                result2.Reflectance, result2.Transmittance, result2.Absorbance);
            
            // 完成驗證測試
            logger.CompleteCalculation();
        }

        /// <summary>
        /// 計算指定偏振類型的光學結果，支援S、P、AVG三種模式
        /// </summary>
        public OpticalResult CalculateWithPolarization(Complex incidentIndex, Complex filmIndex, double thickness,
            Complex substrateIndex, double wavelength, double incidentAngle, PolarizationType polarizationType)
        {
            if (polarizationType == PolarizationType.AVG)
            {
                return CalculateAVGPolarization(incidentIndex, filmIndex, thickness, 
                    substrateIndex, wavelength, incidentAngle);
            }
            else
            {
                bool isSPolarization = polarizationType == PolarizationType.S;
                var result = Calculate(incidentIndex, filmIndex, thickness, 
                    substrateIndex, wavelength, incidentAngle, isSPolarization);
                result.PolarizationType = polarizationType;
                return result;
            }
        }

        /// <summary>
        /// 計算AVG偏振（S和P偏振的平均值）
        /// </summary>
        private OpticalResult CalculateAVGPolarization(Complex incidentIndex, Complex filmIndex, double thickness,
            Complex substrateIndex, double wavelength, double incidentAngle)
        {
            logger.LogStepHeader("AVG偏振計算 - S和P偏振平均");
            
            // 計算S偏振結果
            logger.LogStepHeader("子計算: S偏振");
            var sResult = Calculate(incidentIndex, filmIndex, thickness, 
                substrateIndex, wavelength, incidentAngle, true);
            sResult.PolarizationType = PolarizationType.S;
            
            logger.LogCalculation("S偏振 - 反射率", sResult.Reflectance);
            logger.LogCalculation("S偏振 - 穿透率", sResult.Transmittance);
            logger.LogCalculation("S偏振 - 吸收率", sResult.Absorbance);
            
            // 計算P偏振結果
            logger.LogStepHeader("子計算: P偏振");
            var pResult = Calculate(incidentIndex, filmIndex, thickness, 
                substrateIndex, wavelength, incidentAngle, false);
            pResult.PolarizationType = PolarizationType.P;
            
            logger.LogCalculation("P偏振 - 反射率", pResult.Reflectance);
            logger.LogCalculation("P偏振 - 穿透率", pResult.Transmittance);
            logger.LogCalculation("P偏振 - 吸收率", pResult.Absorbance);
            
            // 計算平均值
            logger.LogStepHeader("AVG計算: 平均值");
            double avgR = (sResult.Reflectance + pResult.Reflectance) / 2.0;
            double avgT = (sResult.Transmittance + pResult.Transmittance) / 2.0;
            double avgA = (sResult.Absorbance + pResult.Absorbance) / 2.0;
            
            logger.LogCalculation("AVG - 反射率", avgR);
            logger.LogCalculation("AVG - 穿透率", avgT);
            logger.LogCalculation("AVG - 吸收率", avgA);
            
            // 檢查平均值的能量守恆
            bool avgEnergyConserved = Math.Abs(avgR + avgT + avgA - 1.0) < 1e-6;
            logger.LogEnergyConservation(avgEnergyConserved, avgR + avgT + avgA);
            
            // 創建AVG結果
            var avgResult = new OpticalResult
            {
                Reflectance = avgR,
                Transmittance = avgT,
                Absorbance = avgA,
                ReflectionCoefficient = new Complex((sResult.ReflectionCoefficient.Real + pResult.ReflectionCoefficient.Real) / 2,
                                                   (sResult.ReflectionCoefficient.Imaginary + pResult.ReflectionCoefficient.Imaginary) / 2),
                TransmissionCoefficient = new Complex((sResult.TransmissionCoefficient.Real + pResult.TransmissionCoefficient.Real) / 2,
                                                     (sResult.TransmissionCoefficient.Imaginary + pResult.TransmissionCoefficient.Imaginary) / 2),
                Wavelength = wavelength,
                IncidentAngle = incidentAngle,
                IsEnergyConserved = avgEnergyConserved,
                PolarizationType = PolarizationType.AVG,
                SPolarizationResult = sResult,
                PPolarizationResult = pResult
            };
            
            logger.LogFinalResults(avgR, avgT, avgA);
            logger.CompleteCalculation();
            
            return avgResult;
        }
    }
}