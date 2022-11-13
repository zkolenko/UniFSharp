using System;
using System.Linq;

namespace UniFSharp
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class AliasNameAttribute : Attribute
    {
        string value;
        public AliasNameAttribute(string aliasName)
        {
            value = aliasName;
        }
        public string AliasName { get { return value; } }

        public static string ToAliasName<TEnum, U>(TEnum value) where TEnum : System.Enum
        {
            //var result = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(AliasNameAttribute), false).Cast<AliasNameAttribute>().Where(x => x != null).Single();
            var v1 = value.GetType();
            var v2 = v1.GetField(value.ToString());
            var v3 = v2.GetCustomAttributes(typeof(AliasNameAttribute), false);
            var v4 = v3.Cast<AliasNameAttribute>();
            var v5 = v4.Where(x => x != null);
            try
            {
                var v6 = v5.Single();
                return v6.AliasName;
            }
            catch (Exception)
            {
                throw new ArgumentException("AliasNameAttribute is not found.");
            }
        }
        public static string ToAliasName<TEnum, U>(U value) where TEnum : System.Enum
        {
            var res = (TEnum)Enum.ToObject(typeof(TEnum), value);
            return ToAliasName<TEnum, U>(res);
        }
    }
}
