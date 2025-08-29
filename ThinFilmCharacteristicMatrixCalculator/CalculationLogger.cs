using System;
using System.Collections.ObjectModel;
using System.Text;
using ThinFilmCharacteristicMatrixCalculator.Models;

namespace ThinFilmCharacteristicMatrixCalculator
{
    public class CalculationLogger
    {
        private StringBuilder logBuilder;
        private const string SEPARATOR = "============================================================";
        private const string LINE = "----------------------------------------";
        
        // 結構化計算步驟數據
        public ObservableCollection<CalculationStep> Steps { get; private set; }
        private CalculationStep? currentStep;
        private CalculationStep? rootStep;

        public CalculationLogger()
        {
            logBuilder = new StringBuilder();
            Steps = new ObservableCollection<CalculationStep>();
        }

        public void LogHeader(string title)
        {
            // 現有的文本日誌
            logBuilder.AppendLine(SEPARATOR);
            logBuilder.AppendLine($"  {title}");
            logBuilder.AppendLine(SEPARATOR);
            logBuilder.AppendLine();
            
            // 創建根步驟
            rootStep = new CalculationStep
            {
                Title = title,
                Type = CalculationStepType.Header,
                Status = CalculationStepStatus.Running
            };
            rootStep.UpdateStatus(CalculationStepStatus.Running);
            Steps.Add(rootStep);
            currentStep = rootStep;
        }

        public void LogInputParameters(Complex incidentIndex, Complex filmIndex, double thickness,
            Complex substrateIndex, double wavelength, double incidentAngle, bool isSPolarization)
        {
            LogHeader("輸入參數");
            logBuilder.AppendLine($"入射介質折射率:    {FormatComplex(incidentIndex)}");
            logBuilder.AppendLine($"薄膜折射率:        {FormatComplex(filmIndex)}");
            logBuilder.AppendLine($"薄膜厚度:          {thickness:F2} nm");
            logBuilder.AppendLine($"基板折射率:        {FormatComplex(substrateIndex)}");
            logBuilder.AppendLine($"入射波長:          {wavelength:F2} nm");
            logBuilder.AppendLine($"入射角度:          {incidentAngle * 180 / Math.PI:F2}°");
            logBuilder.AppendLine($"偏振類型:          {(isSPolarization ? "S偏振" : "P偏振")}");
            logBuilder.AppendLine();
        }

        public void LogStepHeader(string stepName)
        {
            // 現有的文本日誌
            logBuilder.AppendLine(LINE);
            logBuilder.AppendLine($"{stepName}:");
            logBuilder.AppendLine(LINE);
            
            // 創建新的計算步驟
            var stepType = DetermineStepType(stepName);
            var newStep = new CalculationStep
            {
                Title = stepName,
                Type = stepType,
                Status = CalculationStepStatus.Running
            };
            newStep.UpdateStatus(CalculationStepStatus.Running);
            
            // 添加到當前步驟（如果有的話）或根級別
            if (rootStep != null)
            {
                rootStep.AddSubStep(newStep);
            }
            else
            {
                Steps.Add(newStep);
            }
            
            currentStep = newStep;
        }
        
        private CalculationStepType DetermineStepType(string stepName)
        {
            return stepName switch
            {
                var s when s.Contains("輸入參數") => CalculationStepType.InputParameters,
                var s when s.Contains("相位厚度") => CalculationStepType.PhaseThickness,
                var s when s.Contains("光學導納") => CalculationStepType.OpticalAdmittance,
                var s when s.Contains("特徵矩陣") => CalculationStepType.CharacteristicMatrix,
                var s when s.Contains("邊界條件") => CalculationStepType.BoundaryCondition,
                var s when s.Contains("反射和透射") || s.Contains("TRA") => CalculationStepType.TRACalculation,
                var s when s.Contains("驗證") => CalculationStepType.Validation,
                var s when s.Contains("比較") => CalculationStepType.Comparison,
                var s when s.Contains("最終結果") => CalculationStepType.FinalResults,
                _ => CalculationStepType.Header
            };
        }

        public void LogCalculation(string description, Complex value)
        {
            // 現有的文本日誌
            logBuilder.AppendLine($"{description,-25} = {FormatComplex(value)}");
            
            // 添加到當前步驟的結果
            currentStep?.AddResult(description, value, "", "");
        }

        public void LogCalculation(string description, double value)
        {
            // 現有的文本日誌
            logBuilder.AppendLine($"{description,-25} = {value:F6}");
            
            // 添加到當前步驟的結果
            currentStep?.AddResult(description, value, "", "");
        }

        public void LogCalculation(string description, object value)
        {
            if (value is Complex complex)
            {
                LogCalculation(description, complex);
            }
            else if (value is double doubleValue)
            {
                LogCalculation(description, doubleValue);
            }
            else
            {
                // 現有的文本日誌
                logBuilder.AppendLine($"{description,-25} = {value}");
                
                // 添加到當前步驟的結果
                currentStep?.AddResult(description, value ?? "", "", "");
            }
        }

        public void LogMatrix(string name, ComplexMatrix matrix)
        {
            // 現有的文本日誌
            logBuilder.AppendLine($"{name}:");
            logBuilder.AppendLine(matrix.ToFormattedString());
            logBuilder.AppendLine();
            
            // 添加矩陣結果到當前步驟
            if (currentStep != null)
            {
                currentStep.Results.Add(new CalculationResult
                {
                    Name = name,
                    Value = matrix,
                    ResultType = CalculationResultType.Matrix,
                    ParentStep = currentStep
                });
            }
        }

        public void LogFinalResults(double R, double T, double A)
        {
            LogHeader("最終計算結果");
            logBuilder.AppendLine($"反射率 (R) = {R * 100:F4}%");
            logBuilder.AppendLine($"穿透率 (T) = {T * 100:F4}%");
            logBuilder.AppendLine($"吸收率 (A) = {A * 100:F4}%");
            logBuilder.AppendLine($"總和       = {(R + T + A) * 100:F4}%");
            
            bool isConserved = Math.Abs(R + T + A - 1.0) < 1e-6;
            logBuilder.AppendLine($"能量守恆   = {(isConserved ? "✓" : "✗")}");
            logBuilder.AppendLine();
        }

        public void LogEnergyConservation(bool isConserved, double total)
        {
            logBuilder.AppendLine($"能量守恆檢驗: R + T + A = {total * 100:F4}%");
            logBuilder.AppendLine($"誤差: {Math.Abs(total - 1.0) * 100:F6}%");
            logBuilder.AppendLine($"結果: {(isConserved ? "通過 ✓" : "失敗 ✗")}");
            logBuilder.AppendLine();
        }

        public void LogValidationResult(string testName, double expectedR, double expectedT, double expectedA,
            double actualR, double actualT, double actualA)
        {
            LogStepHeader($"驗證測試: {testName}");
            logBuilder.AppendLine($"期望值: R={expectedR:F4}%, T={expectedT:F4}%, A={expectedA:F4}%");
            logBuilder.AppendLine($"計算值: R={actualR*100:F4}%, T={actualT*100:F4}%, A={actualA*100:F4}%");
            
            double errorR = Math.Abs(actualR * 100 - expectedR);
            double errorT = Math.Abs(actualT * 100 - expectedT);
            double errorA = Math.Abs(actualA * 100 - expectedA);
            
            logBuilder.AppendLine($"誤差:   R={errorR:F4}%, T={errorT:F4}%, A={errorA:F4}%");
            
            bool isPassed = errorR < 0.01 && errorT < 0.01 && errorA < 0.01;
            logBuilder.AppendLine($"驗證結果: {(isPassed ? "通過 ✓" : "失敗 ✗")}");
            logBuilder.AppendLine();
        }

        private string FormatComplex(Complex c)
        {
            if (Math.Abs(c.Imaginary) < 1e-10)
                return $"{c.Real:F4}";
            else if (c.Imaginary > 0)
                return $"{c.Real:F4} + {c.Imaginary:F4}i";
            else
                return $"{c.Real:F4} - {Math.Abs(c.Imaginary):F4}i";
        }

        public string GetFullLog()
        {
            return logBuilder.ToString();
        }

        public void Clear()
        {
            logBuilder.Clear();
            Steps.Clear();
            currentStep = null;
            rootStep = null;
        }
        
        /// <summary>
        /// 完成當前步驟
        /// </summary>
        public void CompleteCurrentStep()
        {
            if (currentStep != null)
            {
                currentStep.UpdateStatus(CalculationStepStatus.Completed);
            }
        }
        
        /// <summary>
        /// 標記當前步驟失敗
        /// </summary>
        public void FailCurrentStep()
        {
            if (currentStep != null)
            {
                currentStep.UpdateStatus(CalculationStepStatus.Failed);
            }
        }
        
        /// <summary>
        /// 完成所有計算
        /// </summary>
        public void CompleteCalculation()
        {
            CompleteCurrentStep();
            if (rootStep != null)
            {
                rootStep.UpdateStatus(CalculationStepStatus.Completed);
            }
        }
    }
}