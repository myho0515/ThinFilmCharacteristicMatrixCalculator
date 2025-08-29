using System;
using System.Text;

namespace ThinFilmCharacteristicMatrixCalculator
{
    public class ComplexMatrix
    {
        public Complex[,] Matrix { get; }
        public int Rows => Matrix.GetLength(0);
        public int Columns => Matrix.GetLength(1);

        public ComplexMatrix(int rows, int columns)
        {
            Matrix = new Complex[rows, columns];
            // 初始化為零矩陣
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Matrix[i, j] = new Complex(0, 0);
                }
            }
        }

        public ComplexMatrix(Complex[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            Matrix = new Complex[rows, cols];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Matrix[i, j] = matrix[i, j];
                }
            }
        }

        // 運算符重載 - 矩陣乘法
        public static ComplexMatrix operator *(ComplexMatrix m1, ComplexMatrix m2)
        {
            if (m1.Columns != m2.Rows)
                throw new ArgumentException("Matrix dimensions do not allow multiplication");

            var result = new ComplexMatrix(m1.Rows, m2.Columns);
            
            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m2.Columns; j++)
                {
                    Complex sum = new Complex(0, 0);
                    for (int k = 0; k < m1.Columns; k++)
                    {
                        sum = sum + m1[i, k] * m2[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            
            return result;
        }

        // 運算符重載 - 矩陣加法
        public static ComplexMatrix operator +(ComplexMatrix m1, ComplexMatrix m2)
        {
            if (m1.Rows != m2.Rows || m1.Columns != m2.Columns)
                throw new ArgumentException("Matrix dimensions must match for addition");

            var result = new ComplexMatrix(m1.Rows, m1.Columns);
            
            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Columns; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }
            
            return result;
        }

        // 運算符重載 - 矩陣減法
        public static ComplexMatrix operator -(ComplexMatrix m1, ComplexMatrix m2)
        {
            if (m1.Rows != m2.Rows || m1.Columns != m2.Columns)
                throw new ArgumentException("Matrix dimensions must match for subtraction");

            var result = new ComplexMatrix(m1.Rows, m1.Columns);
            
            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Columns; j++)
                {
                    result[i, j] = m1[i, j] - m2[i, j];
                }
            }
            
            return result;
        }

        // 索引器
        public Complex this[int row, int col]
        {
            get => Matrix[row, col];
            set => Matrix[row, col] = value;
        }

        // 轉置矩陣
        public ComplexMatrix Transpose()
        {
            var result = new ComplexMatrix(Columns, Rows);
            
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result[j, i] = Matrix[i, j];
                }
            }
            
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    sb.Append(Matrix[i, j].ToString() + "\t");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        // 格式化輸出方法
        public string ToFormattedString(string title = "")
        {
            var sb = new StringBuilder();
            
            if (!string.IsNullOrEmpty(title))
            {
                sb.AppendLine(title);
            }
            
            // 計算每列的最大寬度
            int[] columnWidths = new int[Columns];
            for (int j = 0; j < Columns; j++)
            {
                columnWidths[j] = 0;
                for (int i = 0; i < Rows; i++)
                {
                    string formatted = FormatComplex(Matrix[i, j]);
                    columnWidths[j] = Math.Max(columnWidths[j], formatted.Length);
                }
                columnWidths[j] = Math.Max(columnWidths[j], 12); // 最小寬度
            }

            // 輸出矩陣
            for (int i = 0; i < Rows; i++)
            {
                sb.Append("[ ");
                for (int j = 0; j < Columns; j++)
                {
                    string formatted = FormatComplex(Matrix[i, j]);
                    sb.Append(formatted.PadLeft(columnWidths[j]));
                    if (j < Columns - 1)
                        sb.Append("  ");
                }
                sb.AppendLine(" ]");
            }
            
            return sb.ToString();
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

        // 單位矩陣
        public static ComplexMatrix Identity(int size)
        {
            var result = new ComplexMatrix(size, size);
            for (int i = 0; i < size; i++)
            {
                result[i, i] = new Complex(1, 0);
            }
            return result;
        }

        // 行列式（僅適用於2x2矩陣）
        public Complex Determinant()
        {
            if (Rows != 2 || Columns != 2)
                throw new InvalidOperationException("Determinant calculation is only implemented for 2x2 matrices");
            
            return Matrix[0, 0] * Matrix[1, 1] - Matrix[0, 1] * Matrix[1, 0];
        }

        // 逆矩陣（僅適用於2x2矩陣）
        public ComplexMatrix Inverse()
        {
            if (Rows != 2 || Columns != 2)
                throw new InvalidOperationException("Matrix inversion is only implemented for 2x2 matrices");
            
            Complex det = Determinant();
            if (det.AbsSquared() < 1e-15)
                throw new InvalidOperationException("Matrix is singular and cannot be inverted");
            
            var result = new ComplexMatrix(2, 2);
            result[0, 0] = Matrix[1, 1] / det;
            result[0, 1] = new Complex(0, 0) - Matrix[0, 1] / det;
            result[1, 0] = new Complex(0, 0) - Matrix[1, 0] / det;
            result[1, 1] = Matrix[0, 0] / det;
            
            return result;
        }
    }
}