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