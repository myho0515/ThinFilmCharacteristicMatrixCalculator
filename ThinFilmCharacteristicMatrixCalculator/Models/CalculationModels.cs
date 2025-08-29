using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ThinFilmCharacteristicMatrixCalculator.Models
{
    /// <summary>
    /// 計算步驟的數據模型
    /// </summary>
    public class CalculationStep
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public CalculationStepType Type { get; set; }
        public CalculationStepStatus Status { get; set; } = CalculationStepStatus.Pending;
        
        // 計算結果
        public ObservableCollection<CalculationResult> Results { get; set; } = new();
        
        // 子步驟
        public ObservableCollection<CalculationStep> SubSteps { get; set; } = new();
        
        // 父步驟
        public CalculationStep? Parent { get; set; }
        
        // 進度（0-100）
        public double Progress { get; set; } = 0;
        
        // 是否展開
        public bool IsExpanded { get; set; } = true;
        
        // 計算時間
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        
        
        // 添加子步驟
        public void AddSubStep(CalculationStep subStep)
        {
            subStep.Parent = this;
            SubSteps.Add(subStep);
        }
        
        // 添加計算結果
        public void AddResult(string name, object value, string unit = "", string formula = "")
        {
            Results.Add(new CalculationResult
            {
                Name = name,
                Value = value,
                Unit = unit,
                Formula = formula,
                ParentStep = this
            });
        }
        
        // 更新狀態
        public void UpdateStatus(CalculationStepStatus status)
        {
            Status = status;
            if (status == CalculationStepStatus.Running)
            {
                StartTime = DateTime.Now;
            }
            else if (status == CalculationStepStatus.Completed || status == CalculationStepStatus.Failed)
            {
                EndTime = DateTime.Now;
                Progress = status == CalculationStepStatus.Completed ? 100 : 0;
            }
        }
    }
    
    /// <summary>
    /// 計算結果數據模型
    /// </summary>
    public class CalculationResult
    {
        public string Name { get; set; } = "";
        public object? Value { get; set; }
        public string Unit { get; set; } = "";
        public string Formula { get; set; } = "";
        public string Description { get; set; } = "";
        public CalculationResultType ResultType { get; set; } = CalculationResultType.Scalar;
        public CalculationStep? ParentStep { get; set; }
        
        // 格式化顯示值
        public string FormattedValue
        {
            get
            {
                return Value switch
                {
                    Complex c => FormatComplex(c),
                    double d => $"{d:F6}",
                    ComplexMatrix m => $"[{m.Rows}×{m.Columns} Matrix]",
                    _ => Value?.ToString() ?? ""
                };
            }
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
    }
    
    /// <summary>
    /// 計算步驟類型
    /// </summary>
    public enum CalculationStepType
    {
        Header,          // 標題步驟
        InputParameters, // 輸入參數
        PhaseThickness,  // 相位厚度計算
        OpticalAdmittance, // 光學導納計算
        CharacteristicMatrix, // 特徵矩陣計算
        BoundaryCondition,    // 邊界條件參數
        ReflectionTransmission, // 反射透射係數
        TRACalculation,       // TRA計算
        Validation,          // 驗證測試
        Comparison,          // 結果比較
        FinalResults         // 最終結果
    }
    
    /// <summary>
    /// 計算步驟狀態
    /// </summary>
    public enum CalculationStepStatus
    {
        Pending,   // 等待中
        Running,   // 運行中
        Completed, // 已完成
        Failed,    // 失敗
        Warning    // 警告
    }
    
    /// <summary>
    /// 計算結果類型
    /// </summary>
    public enum CalculationResultType
    {
        Scalar,    // 標量
        Complex,   // 複數
        Matrix,    // 矩陣
        Formula,   // 公式
        Text       // 文本
    }
    
    /// <summary>
    /// 視圖模式
    /// </summary>
    public enum ViewMode
    {
        Simple,   // 簡潔模式
        Detailed, // 詳細模式
        Formula,  // 公式模式
        Chart     // 圖表模式
    }
}