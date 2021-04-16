using Shamdev.ERP.DAL.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.ERP.DAL.Common.Utils
{
    public static partial class ExpressionUtils
    {
        public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, TypeFilterEnum comparison, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var body = MakeComparison(left, comparison, value);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression MakeComparison(Expression left, TypeFilterEnum comparison, string value)
        {
            switch (comparison)
            {
                case TypeFilterEnum.EQUAL:
                    return MakeBinary(ExpressionType.Equal, left, value);
                case TypeFilterEnum.NOT_EQUAL:
                    return MakeBinary(ExpressionType.NotEqual, left, value);
                case TypeFilterEnum.GREATER_THAN:
                    return MakeBinary(ExpressionType.GreaterThan, left, value);
                case TypeFilterEnum.GREATER_THAN_OR_EQUAL:
                    return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
                case TypeFilterEnum.LESS_THAN:
                    return MakeBinary(ExpressionType.LessThan, left, value);
                case TypeFilterEnum.LESS_THAN_OR_EQUAL:
                    return MakeBinary(ExpressionType.LessThanOrEqual, left, value);
                case TypeFilterEnum.CONTAINS:
                case TypeFilterEnum.STARTS_WITH:
                case TypeFilterEnum.ENDS_WITH:
                    return Expression.Call(MakeString(left), GetStringComparison(comparison), Type.EmptyTypes, Expression.Constant(value, typeof(string)));
                default:
                    throw new NotSupportedException($"Invalid comparison operator '{comparison}'.");
            }
        }
        private static string GetStringComparison(TypeFilterEnum typeFilterEnum) => (typeFilterEnum switch
        {
            TypeFilterEnum.CONTAINS => "Contains",
            TypeFilterEnum.STARTS_WITH => "StartsWith",
            TypeFilterEnum.ENDS_WITH => "EndsWith",
            _ => throw new NotSupportedException($"Invalid comparison operator '{typeFilterEnum}'.")
        });
        

      
        private static Expression MakeString(Expression source)
        {
            return source.Type == typeof(string) ? source : Expression.Call(source, "ToString", Type.EmptyTypes);
        }

        private static Expression MakeBinary(ExpressionType type, Expression left, string value)
        {
            object typedValue = value;
            if (left.Type != typeof(string))
            {
                if (string.IsNullOrEmpty(value))
                {
                    typedValue = null;
                    if (Nullable.GetUnderlyingType(left.Type) == null)
                        left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
                }
                else
                {
                    var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                    typedValue = valueType.IsEnum ? Enum.Parse(valueType, value) :
                        valueType == typeof(Guid) ? Guid.Parse(value) :
                        Convert.ChangeType(value, valueType);
                }
            }
            var right = Expression.Constant(typedValue, left.Type);
            return Expression.MakeBinary(type, left, right);
        }

      
    }
}
