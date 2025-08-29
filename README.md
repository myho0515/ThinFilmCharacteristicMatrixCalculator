# 光學薄膜特徵矩陣計算器

一個基於WPF的光學薄膜計算器，用於計算薄膜的反射率、穿透率和吸收率。採用特徵矩陣理論進行精確的光學計算。

## 🌟 主要功能

### 📊 光學計算
- **反射率 (R)** - 計算薄膜介面的反射特性
- **穿透率 (T)** - 分析光線通過薄膜的透射能力
- **吸收率 (A)** - 評估薄膜對光的吸收程度
- **能量守恆驗證** - 自動檢查 R + T + A = 1

### 🔬 計算方法
- **特徵矩陣法** - 基於經典光學理論的精確計算
- **導納方法驗證** - 雙重驗證確保計算準確性
- **複數折射率支援** - 正確處理吸收性材料 (N = nᵣ - iKᵣ)
- **斜入射計算** - 支援不同入射角度的光學計算

### 🎛️ 使用者介面
- **三欄式佈局** - 輸入參數 | 結果摘要 | 詳細過程
- **層次化步驟顯示** - TreeView展示計算流程
- **可調整面板** - GridSplitter支援自訂介面佈局
- **即時進度指示** - 視覺化計算進度和狀態
- **多種顯示模式** - 簡潔/詳細/公式/圖表模式

## 🖥️ 系統需求

- **作業系統**: Windows 10/11
- **.NET Framework**: .NET 6.0 或更高版本
- **開發環境**: Visual Studio 2022 (建議)

## 🚀 快速開始

### 安裝步驟

1. **克隆專案**
   ```bash
   git clone https://github.com/myho0515/ThinFilmCharacteristicMatrixCalculator.git
   cd ThinFilmCharacteristicMatrixCalculator
   ```

2. **開啟專案**
   - 使用 Visual Studio 開啟 `ThinFilmCharacteristicMatrixCalculator.sln`
   - 或使用 VS Code 開啟專案資料夾

3. **建置執行**
   ```bash
   dotnet build
   dotnet run --project ThinFilmCharacteristicMatrixCalculator
   ```

### 基本使用

1. **輸入參數**
   - 波長 (nm)
   - 薄膜厚度 (nm) 
   - 薄膜複數折射率 (nᵣ + iKᵣ)
   - 入射介質折射率
   - 基板折射率
   - 入射角度 (度)
   - 偏振類型 (S偏振/P偏振)

2. **選擇預設系統** (可選)
   - 預設測試案例
   - MgF₂ 抗反射鍍膜
   - TiO₂ 高折射率膜
   - SiO₂ 低折射率膜

3. **執行計算**
   - 點擊「計算」按鈕
   - 查看結果摘要和詳細過程
   - 驗證能量守恆

## 📐 理論基礎

### 特徵矩陣理論

對於單層薄膜，特徵矩陣定義為：

```
M = [cos(δ)      i·sin(δ)/η₁]
    [i·η₁·sin(δ)   cos(δ)  ]
```

其中：
- δ = 2πnd·cos(θ₁)/λ (相位厚度)
- η₁ = n₁·cos(θ₁) (S偏振) 或 n₁/cos(θ₁) (P偏振)

### 反射穿透計算

**反射率**: R = |r|² = |(η₀B - C)/(η₀B + C)|²

**穿透率**: T = 4η₀·Re(ηₛ)/|η₀B + C|²

**吸收率**: A = 1 - R - T

## 🏗️ 專案結構

```
ThinFilmCharacteristicMatrixCalculator/
├── ThinFilmCharacteristicMatrixCalculator/
│   ├── Models/                     # 資料模型
│   │   └── CalculationModels.cs    # 計算步驟和結果模型
│   ├── Converters/                 # 值轉換器
│   │   └── StatusConverters.cs     # 狀態到UI的轉換
│   ├── MainWindow.xaml             # 主要UI介面
│   ├── MainWindow.xaml.cs          # UI邏輯處理
│   ├── ThinFilmCalculator.cs       # 核心計算引擎
│   ├── CalculationLogger.cs        # 計算過程記錄
│   ├── Complex.cs                  # 複數數學運算
│   ├── ComplexMatrix.cs            # 複數矩陣運算
│   └── OpticalResult.cs            # 計算結果封裝
├── README.md                       # 專案說明
├── CLAUDE.md                       # 開發指導原則
└── .gitignore                      # Git忽略檔案
```

## 🎯 預設測試案例

### 案例 1: 預設參數
- 薄膜: n = 2.385 - 0.1i, t = 99.45 nm
- 基板: n = 1.52
- 波長: 550 nm
- **預期結果**: R ≈ 11.15%, T ≈ 70.34%, A ≈ 18.51%

### 案例 2: 標準驗證
- 薄膜: n = 2.385 - 0.1i, t = 57.65 nm  
- 基板: n = 1.52
- 波長: 550 nm
- **預期結果**: R ≈ 31.44%, T ≈ 59.57%, A ≈ 8.98%

## 🔧 開發指導

### 程式碼風格
- 使用繁體中文註解
- 遵循 SOLID 設計原則
- 四個空白縮排，每行不超過100字元
- 優先使用 LINQ 和異步/等待模式

### 新增功能
1. 在 `Models/` 中定義資料結構
2. 在 `ThinFilmCalculator.cs` 中實現計算邏輯
3. 在 `MainWindow.xaml` 中設計UI
4. 在 `MainWindow.xaml.cs` 中處理使用者互動

## 🐛 已知問題

- [ ] TreeView顯示需要修復XAML綁定問題
- [ ] 需要實現多層薄膜計算功能
- [ ] 圖表模式尚未完全實現

## 🗺️ 發展藍圖

### v1.1 (規劃中)
- [ ] 修復UI顯示問題
- [ ] 實現多種顯示模式功能
- [ ] 添加結果匯出功能

### v1.2 (未來)
- [ ] 多層薄膜支援
- [ ] 光譜掃描功能
- [ ] 三維視覺化

### v1.3 (長期)
- [ ] 薄膜設計優化器
- [ ] 材料資料庫整合
- [ ] 批次計算處理

## 📊 技術特點

- **MVVM架構** - 清晰的資料綁定和UI分離
- **值轉換器** - 豐富的狀態視覺化效果
- **TreeView階層** - 直觀的計算步驟展示
- **即時驗證** - 輸入參數和計算結果驗證
- **異步計算** - 不阻塞UI的計算體驗

## 🤝 貢獻指南

1. Fork 此專案
2. 創建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交變更 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

## 📄 授權

此專案採用 MIT 授權 - 查看 [LICENSE](LICENSE) 檔案以了解詳情。

## 👤 作者

**myho0515**
- GitHub: [@myho0515](https://github.com/myho0515)

## 🙏 致謝

- 基於經典光學薄膜理論
- 使用 WPF 框架進行UI開發
- 🤖 部分開發由 [Claude Code](https://claude.ai/code) 協助完成

---

⭐ 如果這個專案對你有幫助，請給一個星星！