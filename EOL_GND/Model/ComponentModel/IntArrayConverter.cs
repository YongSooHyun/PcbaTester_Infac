using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    public class IntArrayConverter : TypeConverter
    {
        public const string DashSign = "~";

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var intArray = value as int[];
                if (intArray == null)
                {
                    return null;
                }

                if (intArray.Length == 0)
                {
                    return "";
                }

                // 1-4, 6, 8, 9-12, 15 형식으로 변환.
                var textBuilder = new StringBuilder();
                textBuilder.Append(intArray[0]);
                int dashCount = 0;
                for (int i = 1; i < intArray.Length; i++)
                {
                    if (intArray[i] == intArray[i - 1] + 1)
                    {
                        // 마지막에 Dash를 붙인다.
                        dashCount++;
                    }
                    else
                    {
                        if (dashCount > 1)
                        {
                            textBuilder.Append(DashSign);
                            textBuilder.Append(intArray[i - 1]);
                        }
                        else if (dashCount == 1)
                        {
                            textBuilder.Append(", ");
                            textBuilder.Append(intArray[i - 1]);
                        }

                        textBuilder.Append(", ");
                        textBuilder.Append(intArray[i]);
                        dashCount = 0;
                    }
                }

                if (dashCount > 1)
                {
                    textBuilder.Append(DashSign);
                    textBuilder.Append(intArray[intArray.Length - 1]);
                }
                else if (dashCount == 1)
                {
                    textBuilder.Append(", ");
                    textBuilder.Append(intArray[intArray.Length - 1]);
                }

                return textBuilder.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (stringValue == null)
            {
                return null;
            }
            else if (stringValue.Trim().Length == 0)
            {
                return new int[0];
            }
            else
            {
                // 1-4, 6, 8, 9-12, 15 형식으로 된 문자열 파싱.
                var words = stringValue.Split(',');
                var intList = new List<int>();
                foreach (var word in words)
                {
                    if (string.IsNullOrWhiteSpace(word))
                    {
                        continue;
                    }

                    // x-y 형식인가 체크.
                    var dashedWords = word.Split(new string[] { DashSign }, StringSplitOptions.None);
                    if (dashedWords.Length == 2)
                    {
                        int startValue = int.Parse(dashedWords[0]);
                        int endValue = int.Parse(dashedWords[1]);
                        for (int i = startValue; i <= endValue; i++)
                        {
                            intList.Add(i);
                        }
                    }
                    else if (dashedWords.Length == 1)
                    {
                        int val = int.Parse(dashedWords[0]);
                        intList.Add(val);
                    }
                    else
                    {
                        throw new Exception($"Invalid text format. Allowed text format: Ex. 1{DashSign}3, 4, 9{DashSign}20");
                    }
                }

                return intList.ToArray();
            }
        }
    }
}
