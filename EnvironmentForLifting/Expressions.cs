using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Expressions improved to work with certain types and operators
    /// Contains expressions to data conversions
    /// </summary>
    /// <typeparam name="T">Type of Data</typeparam>
    class ImprovedExpression<T> : Expression
    {
        private static List<Type> smallTypes = new List<Type> { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort) };
        private static List<Type> typesWithoutNegate = new List<Type>(smallTypes) { typeof(uint), typeof(ulong)};
        private static Expression ToByte(Expression expression)
        {
            return Convert(expression, typeof(byte));
        }
        private static Expression ToInt(Expression expression)
        {
            return Convert(expression, typeof(int));
        }
        private static Expression ToFloat(Expression expression)
        {
            return Convert(expression, typeof(float));
        }
        private static Expression ToDouble(Expression expression)
        {
            return Convert(expression, typeof(double));
        }
        private static Expression ToT(Expression expression)
        {
            return Convert(expression, typeof(T));
        }
        private static new Expression Negate(Expression expression)
        {
            if (typesWithoutNegate.Contains(expression.Type))
                return Convert(Expression.Negate(Convert(expression, typeof(int))), expression.Type);
            else
                return Expression.Negate(expression);
        }
        private static new Expression Equal(Expression expression1, Expression expression2)
        {
            if (expression1.Type != expression2.Type) throw new InvalidOperationException();
            return Expression.Equal(expression1, expression2);
        }
        private static new Expression Add(Expression expression1, Expression expression2)
        {
            if (expression1.Type != expression2.Type) throw new InvalidOperationException();
            if (smallTypes.Contains(expression1.Type))
                return Convert(Expression.Add(Convert(expression1, typeof(int)), Convert(expression2, typeof(int))), expression1.Type);
            else
                return Expression.Add(expression1, expression2);
        }
        private static new Expression Subtract(Expression expression1, Expression expression2)
        {
            if (expression1.Type != expression2.Type) throw new InvalidOperationException();
            if (smallTypes.Contains(expression1.Type))
                return Convert(Expression.Subtract(Convert(expression1, typeof(int)), Convert(expression2, typeof(int))), expression1.Type);
            else
                return Expression.Subtract(expression1, expression2);
        }
        private static new Expression Divide(Expression expression1, Expression expression2)
        {
            if (expression2.Type != typeof(int)) throw new InvalidOperationException();
            if (smallTypes.Contains(expression1.Type))
                return Convert(Expression.Divide(Convert(expression1, typeof(int)), expression2), expression1.Type);
            else
                return Expression.Divide(expression1, Convert(expression2, expression1.Type));
        }
        private static new Expression Multiply(Expression expression1, Expression expression2)
        {
            if (expression2.Type == typeof(double))
            {
                return Convert(Expression.Multiply(Convert(expression1, typeof(double)), expression2), expression1.Type);
            }
            else
            {
                if (smallTypes.Contains(expression1.Type))
                    return Convert(Expression.Multiply(Convert(expression1, typeof(int)), expression2), expression1.Type);
                else
                    return Expression.Multiply(expression1, Convert(expression2, expression1.Type));
            }
        }
        public static Func<T,T,bool> GetEqualFuncCompiled(ParameterExpression parameter1, ParameterExpression parameter2)
        {
            return Lambda<Func<T, T, bool>>(Equal(parameter1, parameter2), parameter1, parameter2).Compile();
        }
        public static Func<T,byte> GetToByteFuncCompiled(ParameterExpression parameter)
        {
            return Lambda<Func<T, byte>>(ToByte(parameter), parameter).Compile();
        }
        public static Func<T, int> GetToIntFuncCompiled(ParameterExpression parameter)
        {
            return Lambda<Func<T, int>>(ToInt(parameter), parameter).Compile();
        }
        public static Func<T, float> GetToFloatFuncCompiled(ParameterExpression parameter)
        {
            return Lambda<Func<T, float>>(ToFloat(parameter), parameter).Compile();
        }
        public static Func<T, double> GetToDoubleFuncCompiled(ParameterExpression parameter)
        {
            return Lambda<Func<T, double>>(ToDouble(parameter), parameter).Compile();
        }
        public static Func<byte,T> GetByteToTFuncCompiled(ParameterExpression parameter)
        {
            return Lambda<Func<byte, T>>(ToT(parameter), parameter).Compile();
        }
        public static Func<float, T> GetFloatToTFuncCompiled(ParameterExpression parameter)
        {
            return Lambda<Func<float, T>>(ToT(parameter), parameter).Compile();
        }
        public static Func<double, T> GetDoubleToTFuncCompiled(ParameterExpression parameter)
        {
            return Lambda<Func<double, T>>(ToT(parameter), parameter).Compile();
        }
        public static Func<T, T, T> GetAddFuncCompiled(ParameterExpression parameter1, ParameterExpression parameter2)
        {
            return Lambda<Func<T, T, T>>(Add(parameter1, parameter2), parameter1, parameter2).Compile();
        }
        public static Func<T, T, T> GetSubtractFuncCompiled(ParameterExpression parameter1, ParameterExpression parameter2)
        {
            return Lambda<Func<T, T, T>>(Subtract(parameter1, parameter2), parameter1, parameter2).Compile();
        }
        public static Func<T, int, T> GetDivideFuncCompiled(ParameterExpression parameter1, ParameterExpression parameter2)
        {
            return Lambda<Func<T, int, T>>(Divide(parameter1, parameter2), parameter1, parameter2).Compile();
        }
        public static Func<T, int, T> GetMultiplyIntFuncCompiled(ParameterExpression parameter1, ParameterExpression parameter2)
        {
            return Lambda<Func<T, int, T>>(Multiply(parameter1, parameter2), parameter1, parameter2).Compile();
        }
        public static Func<T, double, T> GetMultiplyDoubleFuncCompiled(ParameterExpression parameter1, ParameterExpression parameter2)
        {
            return Lambda<Func<T, double, T>>(Multiply(parameter1, parameter2), parameter1, parameter2).Compile();
        }
        public static Func<T,T> GetNegateFuncCompiled(ParameterExpression parameter)
        {
            return Lambda<Func<T, T>>(Negate(parameter), parameter).Compile();
        }
    }
}
