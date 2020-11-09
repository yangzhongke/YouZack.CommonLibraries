using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;


namespace Infrastructures.EFCore
{
    //不要继承自非泛型的ValueConverter，用起来太复杂
    public class SplittedStringToStringArrayValueConverter : ValueConverter<string[], string>
    {
        //为什么ValueConverter的构造函数这么特别，那是因为即使不用创建子类也能用
        public SplittedStringToStringArrayValueConverter(string seperator, ConverterMappingHints mappingHints = null) :
            base(model => ToProvider(model, seperator), provider => ToModel(provider, seperator), mappingHints)
        {
        }

        private static string ToProvider(string[] model, string seperator)
        {
            return string.Join(seperator, model);
        }

        private static string[] ToModel(string provider, string seperator)
        {
            //actually,A null value will never be passed to a value converter. This makes the implementation of conversions easier and allows them to be shared amongst nullable and non-nullable properties.
            //不过习惯了，而且希望微软改掉这个“by design”
            if (provider == null)
            {
                return new string[0];
            }
            else
            {
                return provider.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
