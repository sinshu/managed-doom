using System;
using System.Runtime.CompilerServices;

namespace ManagedDoom
{
    public struct Fixed
    {
        public const int FracBits = 16;
        public const int FracUnit = 1 << FracBits;

        public static readonly Fixed Zero = new Fixed(0);
        public static readonly Fixed One = new Fixed(FracUnit);

        public static readonly Fixed MaxValue = new Fixed(int.MaxValue);
        public static readonly Fixed MinValue = new Fixed(int.MinValue);

        public static readonly Fixed Epsilon = new Fixed(1);
        public static readonly Fixed OneMinusEpsilon = new Fixed(FracUnit - 1);

        private int data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fixed(int data)
        {
            this.data = data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed FromInt(int value)
        {
            return new Fixed(value << FracBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed FromDouble(double value)
        {
            return new Fixed((int)(FracUnit * value));
        }

        public double ToDouble()
        {
            return (double)data / FracUnit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Abs(Fixed a)
        {
            if (a.data < 0)
            {
                return new Fixed(-a.data);
            }
            else
            {
                return a;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator +(Fixed a)
        {
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator -(Fixed a)
        {
            return new Fixed(-a.data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator +(Fixed a, Fixed b)
        {
            return new Fixed(a.data + b.data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator -(Fixed a, Fixed b)
        {
            return new Fixed(a.data - b.data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator *(Fixed a, Fixed b)
        {
            return new Fixed((int)(((long)a.data * (long)b.data) >> FracBits));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator *(int a, Fixed b)
        {
            return new Fixed(a * b.data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator *(Fixed a, int b)
        {
            return new Fixed(a.data * b);
        }

        public static Fixed operator /(Fixed a, Fixed b)
        {
            if ((Math.Abs(a.data) >> 14) >= Math.Abs(b.data))
            {
                return new Fixed((a.data ^ b.data) < 0 ? int.MinValue : int.MaxValue);
            }

            return FixedDiv2(a, b);
        }

        private static Fixed FixedDiv2(Fixed a, Fixed b)
        {
            var c = ((double)a.data) / ((double)b.data) * FracUnit;

            if (c >= 2147483648.0 || c < -2147483648.0)
            {
                throw new DivideByZeroException();
            }

            return new Fixed((int)c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator /(int a, Fixed b)
        {
            return Fixed.FromInt(a) / b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator /(Fixed a, int b)
        {
            return new Fixed(a.data / b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Fixed a, Fixed b)
        {
            return a.data == b.data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Fixed a, Fixed b)
        {
            return a.data != b.data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Fixed a, Fixed b)
        {
            return a.data < b.data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Fixed a, Fixed b)
        {
            return a.data > b.data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Fixed a, Fixed b)
        {
            return a.data <= b.data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Fixed a, Fixed b)
        {
            return a.data >= b.data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Min(Fixed a, Fixed b)
        {
            if (a < b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Max(Fixed a, Fixed b)
        {
            if (a < b)
            {
                return b;
            }
            else
            {
                return a;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIntFloor(Fixed a)
        {
            return a.data >> FracBits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIntCeiling(Fixed a)
        {
            return (a.data + FracUnit - 1) >> FracBits;
        }

        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override string ToString()
        {
            return ((double)data / FracUnit).ToString();
        }

        public int Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data;
        }
    }
}
