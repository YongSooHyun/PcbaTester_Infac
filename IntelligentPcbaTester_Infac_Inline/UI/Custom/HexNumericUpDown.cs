using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public class HexNumericUpDown : NumericUpDown
    {
        public HexNumericUpDown()
        {
            Hexadecimal = true;
        }

        private long ParseHexString(string hexText)
        {
            if (long.TryParse(hexText, NumberStyles.HexNumber, null, out long parsed))
            {
                return parsed;
            }
            else
            {
                return (long)Minimum;
            }
        }

        protected override void UpdateEditText()
        {
            Text = $"{(long)Value:X}";
            //base.UpdateEditText();
        }

        protected override void ValidateEditText()
        {
            var parsedValue = ParseHexString(Text);
            if (parsedValue > Maximum)
            {
                Value = Maximum;
            }
            else if (parsedValue < Minimum)
            {
                Value = Minimum;
            }
            else
            {
                Value = parsedValue;
            }

            UserEdit = false;
            UpdateEditText();
            //base.ValidateEditText();
        }
    }
}
