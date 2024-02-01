using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public class TextProgressBar : ProgressBar
    {
        [Browsable(true)]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Invalidate();
            }
        }

        [Category("Appearance"), DefaultValue(true)]
        [Description("Indicates whether the displayed text is automatically generated from the value.")]
        public bool AutoText
        {
            get => _autoText;
            set
            {
                if (_autoText != value)
                {
                    _autoText = value;
                    Invalidate();
                }
            }
        }
        private bool _autoText = true;

        public TextProgressBar() : base()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var brush = new SolidBrush(ForeColor))
            {
                var rect = ClientRectangle;

                // Draw border.
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rect);

                // Draw progress.
                rect.Inflate(-1, -1);

                var valueWidth = Maximum - Minimum;
                float rate;
                if (valueWidth == 0)
                {
                    rate = 0;
                }
                else
                {
                    rate = (float)(Value - Minimum) / valueWidth;
                }

                if (rate > 0)
                {
                    var progressRect = new Rectangle(rect.X, rect.Y, (int)Math.Round(rate * rect.Width), rect.Height);
                    ProgressBarRenderer.DrawHorizontalChunks(e.Graphics, progressRect);
                }

                // Draw text.
                string textToDisplay;
                if (AutoText)
                {
                    if (valueWidth == 0)
                    {
                        textToDisplay = string.Empty;
                    }
                    else
                    {
                        var percent = (Value - Minimum) * 100 / valueWidth;
                        textToDisplay = $"{percent}%";
                    }
                }
                else
                {
                    textToDisplay = Text;
                }

                int centerX = rect.X + rect.Width / 2;
                int centerY = rect.Y + rect.Height / 2 + 1;
                var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(textToDisplay, Font, brush, centerX, centerY, textFormat);
            }
        }
    }
}
