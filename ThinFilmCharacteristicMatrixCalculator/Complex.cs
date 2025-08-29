using System;

namespace ThinFilmCharacteristicMatrixCalculator
{
	public class Complex
	{
		public double Real { get; set; }
		public double Imaginary { get; set; }

		public Complex(double real, double imaginary = 0)
		{
			Real = real;
			Imaginary = imaginary;
		}

		// 基本運算符重載
		public static Complex operator +(Complex c1, Complex c2)
		{
			return new Complex(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
		}

		public static Complex operator -(Complex c1, Complex c2)
		{
			return new Complex(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
		}

		public static Complex operator *(Complex c1, Complex c2)
		{
			return new Complex(
				c1.Real * c2.Real - c1.Imaginary * c2.Imaginary,
				c1.Real * c2.Imaginary + c1.Imaginary * c2.Real
			);
		}

		public static Complex operator /(Complex c1, Complex c2)
		{
			double denominator = c2.Real * c2.Real + c2.Imaginary * c2.Imaginary;
			if (Math.Abs(denominator) < 1e-15)
				throw new DivideByZeroException("Division by zero complex number");

			return new Complex(
				(c1.Real * c2.Real + c1.Imaginary * c2.Imaginary) / denominator,
				(c1.Imaginary * c2.Real - c1.Real * c2.Imaginary) / denominator
			);
		}

		// 與實數運算
		public static Complex operator *(Complex c, double d)
		{
			return new Complex(c.Real * d, c.Imaginary * d);
		}

		public static Complex operator *(double d, Complex c)
		{
			return new Complex(c.Real * d, c.Imaginary * d);
		}

		public static Complex operator /(Complex c, double d)
		{
			if (Math.Abs(d) < 1e-15)
				throw new DivideByZeroException("Division by zero");
			return new Complex(c.Real / d, c.Imaginary / d);
		}

		// 關鍵數學函數
		public static Complex Cos(Complex c)
		{
			return new Complex(
				Math.Cos(c.Real) * Math.Cosh(c.Imaginary),
				-Math.Sin(c.Real) * Math.Sinh(c.Imaginary)
			);
		}

		public static Complex Sin(Complex c)
		{
			return new Complex(
				Math.Sin(c.Real) * Math.Cosh(c.Imaginary),
				Math.Cos(c.Real) * Math.Sinh(c.Imaginary)
			);
		}

		public static Complex Exp(Complex c)
		{
			double expReal = Math.Exp(c.Real);
			return new Complex(
				expReal * Math.Cos(c.Imaginary),
				expReal * Math.Sin(c.Imaginary)
			);
		}

		public static Complex Sqrt(Complex c)
		{
			double magnitude = Math.Sqrt(c.Real * c.Real + c.Imaginary * c.Imaginary);
			double phase = Math.Atan2(c.Imaginary, c.Real) / 2.0;
			double sqrtMagnitude = Math.Sqrt(magnitude);
			return new Complex(
				sqrtMagnitude * Math.Cos(phase),
				sqrtMagnitude * Math.Sin(phase)
			);
		}

		// 實例方法
		public double AbsSquared()
		{
			return Real * Real + Imaginary * Imaginary;
		}

		public double Abs()
		{
			return Math.Sqrt(AbsSquared());
		}

		public Complex Conjugate()
		{
			return new Complex(Real, -Imaginary);
		}

		public double Arg()
		{
			return Math.Atan2(Imaginary, Real);
		}

		public Complex Reciprocal()
		{
			double denominator = AbsSquared();
			if (Math.Abs(denominator) < 1e-15)
				throw new DivideByZeroException("Cannot compute reciprocal of zero complex number");

			return new Complex(Real / denominator, -Imaginary / denominator);
		}

		// 字串表示
		public override string ToString()
		{
			if (Math.Abs(Imaginary) < 1e-10)
				return $"{Real:F6}";
			else if (Imaginary > 0)
				return $"{Real:F6} + {Imaginary:F6}i";
			else
				return $"{Real:F6} - {Math.Abs(Imaginary):F6}i";
		}

		public string ToString(string format)
		{
			if (Math.Abs(Imaginary) < 1e-10)
				return Real.ToString(format);
			else if (Imaginary > 0)
				return $"{Real.ToString(format)} + {Imaginary.ToString(format)}i";
			else
				return $"{Real.ToString(format)} - {Math.Abs(Imaginary).ToString(format)}i";

		}
	}
}