using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerMap
{
    public static class ExtensionMethods
    {
        public static List<TEnum> GetEnumList<TEnum>() where TEnum : Enum 
            => ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();
        
        public static T GetAttributeOfType<T>(this Enum enumVal) where T:System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }
}