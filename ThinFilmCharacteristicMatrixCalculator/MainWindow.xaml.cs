using System;
using System.Windows;
using System.Windows.Controls;
using ThinFilmCharacteristicMatrixCalculator.Models;

namespace ThinFilmCharacteristicMatrixCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CalculationLogger currentLogger;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDefaultValues();
            InitializeViewModeHandlers();
        }

        private void InitializeDefaultValues()
        {
            txtWavelength.Text = "550";
            txtThickness.Text = "99.45";
            txtFilmIndexReal.Text = "2.385";
            txtFilmIndexImag.Text = "0.1";
            txtIncidentIndex.Text = "1.0";
            txtSubstrateIndex.Text = "1.52";
            txtIncidentAngle.Text = "0";
            rbSPolarization.IsChecked = true;
        }

        private void InitializeViewModeHandlers()
        {
            rbSimpleView.Checked += ViewMode_Changed;
            rbDetailedView.Checked += ViewMode_Changed;
            rbFormulaView.Checked += ViewMode_Changed;
            rbChartView.Checked += ViewMode_Changed;
        }

        private void ViewMode_Changed(object sender, RoutedEventArgs e)
        {
            if (sender == rbSimpleView)
            {
                SetSimpleView();
            }
            else if (sender == rbDetailedView)
            {
                SetDetailedView();
            }
            else if (sender == rbFormulaView)
            {
                SetFormulaView();
            }
            else if (sender == rbChartView)
            {
                SetChartView();
            }
        }

        private void SetSimpleView()
        {
            // 簡潔模式：只顯示文字結果，隱藏TreeView
            treeViewSteps.Visibility = System.Windows.Visibility.Collapsed;
            txtResults.Visibility = System.Windows.Visibility.Visible;
            
            // 只顯示摘要資訊
            if (currentLogger != null)
            {
                var simpleLog = "=== 簡潔模式計算結果 ===\n";
                simpleLog += $"反射率: {lblReflectance.Content}\n";
                simpleLog += $"穿透率: {lblTransmittance.Content}\n";
                simpleLog += $"吸收率: {lblAbsorbance.Content}\n";
                simpleLog += $"能量守恆: {lblEnergyConserved.Content}\n";
                txtResults.Text = simpleLog;
            }
            
            txtStatus.Text = "已切換到簡潔模式";
        }

        private void SetDetailedView()
        {
            // 詳細模式：顯示TreeView和完整過程
            treeViewSteps.Visibility = System.Windows.Visibility.Visible;
            txtResults.Visibility = System.Windows.Visibility.Collapsed;
            
            txtStatus.Text = "已切換到詳細模式";
        }

        private void SetFormulaView()
        {
            // 公式模式：顯示帶有數學公式的文字
            treeViewSteps.Visibility = System.Windows.Visibility.Collapsed;
            txtResults.Visibility = System.Windows.Visibility.Visible;
            
            if (currentLogger != null)
            {
                var formulaLog = "=== 公式模式 - 數學推導 ===\n\n";
                formulaLog += "特徵矩陣理論:\n";
                formulaLog += "M = [cos(δ)      i·sin(δ)/η₁]\n";
                formulaLog += "    [i·η₁·sin(δ)   cos(δ)  ]\n\n";
                formulaLog += "其中: δ = 2πnd·cos(θ)/λ (相位厚度)\n\n";
                formulaLog += "反射率公式: R = |(η₀B - C)/(η₀B + C)|²\n";
                formulaLog += "穿透率公式: T = 4η₀·Re(ηₛ)/|η₀B + C|²\n";
                formulaLog += "吸收率公式: A = 1 - R - T\n\n";
                formulaLog += "=== 計算結果 ===\n";
                formulaLog += $"反射率 R = {lblReflectance.Content}\n";
                formulaLog += $"穿透率 T = {lblTransmittance.Content}\n";
                formulaLog += $"吸收率 A = {lblAbsorbance.Content}\n";
                formulaLog += currentLogger.GetFullLog();
                txtResults.Text = formulaLog;
            }
            
            txtStatus.Text = "已切換到公式模式";
        }

        private void SetChartView()
        {
            // 圖表模式：顯示視覺化圖表
            treeViewSteps.Visibility = System.Windows.Visibility.Collapsed;
            txtResults.Visibility = System.Windows.Visibility.Visible;
            
            if (currentLogger != null)
            {
                var chartText = CreateASCIIChart();
                txtResults.Text = chartText;
            }
            
            txtStatus.Text = "已切換到圖表模式";
        }

        private string CreateASCIIChart()
        {
            var chart = "=== 圖表模式 - 視覺化結果 ===\n\n";
            
            // 解析當前結果
            double R = 0, T = 0, A = 0;
            if (double.TryParse(lblReflectance.Content?.ToString()?.Replace("%", ""), out double r))
                R = r;
            if (double.TryParse(lblTransmittance.Content?.ToString()?.Replace("%", ""), out double t))
                T = t;
            if (double.TryParse(lblAbsorbance.Content?.ToString()?.Replace("%", ""), out double a))
                A = a;

            chart += "反射率/穿透率/吸收率 柱狀圖:\n\n";
            
            // 創建ASCII柱狀圖
            int maxWidth = 50;
            double maxValue = Math.Max(R, Math.Max(T, A));
            if (maxValue > 0)
            {
                int rWidth = (int)(R / maxValue * maxWidth);
                int tWidth = (int)(T / maxValue * maxWidth);
                int aWidth = (int)(A / maxValue * maxWidth);

                chart += $"反射率 {R:F1}% |{new string('█', rWidth)}{new string('░', maxWidth - rWidth)}|\n";
                chart += $"穿透率 {T:F1}% |{new string('█', tWidth)}{new string('░', maxWidth - tWidth)}|\n";
                chart += $"吸收率 {A:F1}% |{new string('█', aWidth)}{new string('░', maxWidth - aWidth)}|\n";
            }
            
            chart += $"\n能量守恆檢驗: {lblEnergyConserved.Content}\n";
            chart += $"總和: {R + T + A:F1}%\n\n";
            
            // 添加薄膜結構圖
            chart += "薄膜結構示意圖:\n\n";
            chart += "空氣 → 薄膜 → 基板\n";
            chart += "  |     |     |\n";
            chart += "  n₀    n₁    nₛ\n";
            chart += $"  {txtIncidentIndex.Text}   {txtFilmIndexReal.Text}-i{txtFilmIndexImag.Text}   {txtSubstrateIndex.Text}\n\n";
            chart += "光線路徑:\n";
            chart += "入射光 ──→ ╔═══════╗ ──→ 透射光\n";
            chart += $"   ↗     ║ 薄膜   ║      ↘\n";
            chart += $"反射光    ║{txtThickness.Text} nm ║     吸收\n";
            chart += "          ╚═══════╝\n";
            
            return chart;
        }

        private void ApplyCurrentViewMode()
        {
            if (rbSimpleView.IsChecked == true)
            {
                SetSimpleView();
            }
            else if (rbDetailedView.IsChecked == true)
            {
                SetDetailedView();
            }
            else if (rbFormulaView.IsChecked == true)
            {
                SetFormulaView();
            }
            else if (rbChartView.IsChecked == true)
            {
                SetChartView();
            }
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 初始化進度
                progressCalculation.Value = 0;
                lblCalculationStatus.Content = "正在解析參數...";
                
                // 參數解析
                var incidentIndex = new Complex(double.Parse(txtIncidentIndex.Text), 0);
                var filmIndex = new Complex(
                    double.Parse(txtFilmIndexReal.Text),
                    -double.Parse(txtFilmIndexImag.Text)  // 負號修正：N = nᵣ - iKᵣ
                );
                var substrateIndex = new Complex(double.Parse(txtSubstrateIndex.Text), 0);
                var thickness = double.Parse(txtThickness.Text);
                var wavelength = double.Parse(txtWavelength.Text);
                var incidentAngle = double.Parse(txtIncidentAngle.Text) * Math.PI / 180;
                var isSPolarization = rbSPolarization.IsChecked == true;

                progressCalculation.Value = 20;
                lblCalculationStatus.Content = "執行計算中...";
                
                // 執行計算
                currentLogger = new CalculationLogger();
                var calculator = new ThinFilmCalculator(currentLogger);
                var result = calculator.Calculate(incidentIndex, filmIndex, thickness, 
                    substrateIndex, wavelength, incidentAngle, isSPolarization);
                
                progressCalculation.Value = 80;

                // 綁定TreeView到計算步驟
                treeViewSteps.ItemsSource = currentLogger.Steps;
                
                // 顯示結果（保留備用）
                txtResults.Text = currentLogger.GetFullLog();
                txtResults.ScrollToEnd();
                
                // 更新摘要顯示
                lblReflectance.Content = $"{result.Reflectance * 100:F4}%";
                lblTransmittance.Content = $"{result.Transmittance * 100:F4}%";
                lblAbsorbance.Content = $"{result.Absorbance * 100:F4}%";
                lblEnergyConserved.Content = result.IsEnergyConserved ? "✓ 守恆" : "✗ 不守恆";
                lblEnergyConserved.Foreground = result.IsEnergyConserved ? 
                    System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
                
                progressCalculation.Value = 100;
                lblCalculationStatus.Content = "計算完成";
                
                // 根據當前選擇的模式顯示結果
                ApplyCurrentViewMode();
                
                // 更新狀態列
                txtStatus.Text = $"計算完成 - R: {result.Reflectance*100:F2}%, T: {result.Transmittance*100:F2}%, A: {result.Absorbance*100:F2}%";
                
                // 能量守恆檢查
                if (!result.IsEnergyConserved)
                {
                    MessageBox.Show("警告：能量不守恆，請檢查輸入參數！", "計算警告", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (FormatException)
            {
                progressCalculation.Value = 0;
                lblCalculationStatus.Content = "輸入錯誤";
                MessageBox.Show("輸入格式錯誤，請檢查所有數值輸入！", "輸入錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "輸入錯誤";
            }
            catch (Exception ex)
            {
                progressCalculation.Value = 0;
                lblCalculationStatus.Content = "計算失敗";
                MessageBox.Show($"計算錯誤: {ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "計算失敗";
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtResults.Clear();
            treeViewSteps.ItemsSource = null;
            currentLogger = null;
            lblReflectance.Content = "--";
            lblTransmittance.Content = "--";
            lblAbsorbance.Content = "--";
            lblEnergyConserved.Content = "--";
            lblEnergyConserved.Foreground = System.Windows.Media.Brushes.Black;
            progressCalculation.Value = 0;
            lblCalculationStatus.Content = "就緒";
            txtStatus.Text = "已清除結果";
        }

        private void btnCopyResults_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtResults.Text))
            {
                Clipboard.SetText(txtResults.Text);
                txtStatus.Text = "結果已複製到剪貼簿";
            }
            else
            {
                MessageBox.Show("沒有結果可複製！", "提示", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRunTests_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentLogger = new CalculationLogger();
                var calculator = new ThinFilmCalculator(currentLogger);
                
                currentLogger.LogHeader("自動驗證測試");
                
                // 測試案例1：預設參數
                var result1 = calculator.Calculate(
                    new Complex(1.0, 0),       // 空氣
                    new Complex(2.385, -0.1),  // 薄膜：N = nᵣ - iKᵣ
                    99.45,                     // 厚度
                    new Complex(1.52, 0),      // 基板
                    550,                       // 波長
                    0,                         // 垂直入射
                    true                       // S偏振
                );
                
                currentLogger.LogValidationResult("預設參數測試", 11.1475, 70.3446, 18.5079, 
                    result1.Reflectance, result1.Transmittance, result1.Absorbance);
                
                // 測試案例2：標準驗證案例
                var result2 = calculator.Calculate(
                    new Complex(1.0, 0),       // 空氣
                    new Complex(2.385, -0.1),  // 薄膜：N = nᵣ - iKᵣ
                    57.65,                     // 厚度
                    new Complex(1.52, 0),      // 基板
                    550,                       // 波長
                    0,                         // 垂直入射
                    true                       // S偏振
                );
                
                currentLogger.LogValidationResult("標準驗證案例", 31.4424, 59.5727, 8.9849, 
                    result2.Reflectance, result2.Transmittance, result2.Absorbance);
                
                // 測試案例3：複數數學函數測試
                currentLogger.LogStepHeader("複數數學函數測試");
                var testComplex = new Complex(1, 1);
                currentLogger.LogCalculation("測試複數", testComplex);
                currentLogger.LogCalculation("平方根", Complex.Sqrt(testComplex));
                currentLogger.LogCalculation("餘弦", Complex.Cos(testComplex));
                currentLogger.LogCalculation("正弦", Complex.Sin(testComplex));
                
                // 綁定TreeView到計算步驟
                treeViewSteps.ItemsSource = currentLogger.Steps;
                
                txtResults.Text = currentLogger.GetFullLog();
                txtResults.ScrollToEnd();
                
                // 應用當前視圖模式
                ApplyCurrentViewMode();
                
                txtStatus.Text = "驗證測試完成";
                
                MessageBox.Show("所有驗證測試已完成，請查看詳細結果。", "測試完成", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"測試過程發生錯誤: {ex.Message}", "測試錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "測試失敗";
            }
        }

        private void cbPresetSystems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPresetSystems.SelectedItem is ComboBoxItem selectedItem)
            {
                string selection = selectedItem.Content.ToString();
                
                switch (selection)
                {
                    case "預設測試案例":
                        SetParameters(1.0, 2.385, 0.1, 99.45, 1.52, 550, 0);
                        break;
                        
                    case "標準驗證案例":
                        SetParameters(1.0, 2.385, 0.1, 57.65, 1.52, 550, 0);
                        break;
                        
                    case "MgF2 抗反射鍍膜":
                        SetParameters(1.0, 1.22, 0, 113.0, 1.52, 550, 0);
                        break;
                        
                    case "TiO2 高折射率膜":
                        SetParameters(1.0, 2.4, 0, 229.2, 1.52, 550, 0);
                        break;
                        
                    case "SiO2 低折射率膜":
                        SetParameters(1.0, 1.46, 0, 188.4, 1.52, 550, 0);
                        break;
                }
                
                if (cbPresetSystems.SelectedIndex > 0)
                {
                    txtStatus.Text = $"已載入預設系統: {selection}";
                }
            }
        }

        private void SetParameters(double incidentN, double filmN, double filmK, 
            double thickness, double substrateN, double wavelength, double angle)
        {
            txtIncidentIndex.Text = incidentN.ToString();
            txtFilmIndexReal.Text = filmN.ToString();
            txtFilmIndexImag.Text = filmK.ToString();
            txtThickness.Text = thickness.ToString();
            txtSubstrateIndex.Text = substrateN.ToString();
            txtWavelength.Text = wavelength.ToString();
            txtIncidentAngle.Text = angle.ToString();
        }
    }
}