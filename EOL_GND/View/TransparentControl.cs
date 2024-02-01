using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public class TransparentControl : Control
    {
        private bool drag = false;

        public TransparentControl()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            BackColor = Color.Transparent;
        }

        public int Opacity
        {
            get => _opacity;
            set
            {
                if (value > 100)
                {
                    value = 100;
                }
                else if (value < 1)
                {
                    value = 1;
                }

                if (_opacity != value)
                {
                    _opacity = value;
                    if (Parent != null)
                    {
                        Parent.Invalidate(Bounds, true);
                    }
                }
            }
        }
        private int _opacity = 100;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);

            Color frmColor = Parent.BackColor;
            Brush bckColor = default(Brush);

            int alpha = Opacity * 255 / 100;

            if (drag)
            {
                Color dragBckColor = default(Color);

                if (BackColor != Color.Transparent)
                {
                    int Rb = BackColor.R * alpha / 255 + frmColor.R * (255 - alpha) / 255;
                    int Gb = BackColor.G * alpha / 255 + frmColor.G * (255 - alpha) / 255;
                    int Bb = BackColor.B * alpha / 255 + frmColor.B * (255 - alpha) / 255;
                    dragBckColor = Color.FromArgb(Rb, Gb, Bb);
                }
                else
                {
                    dragBckColor = frmColor;
                }

                alpha = 255;
                bckColor = new SolidBrush(Color.FromArgb(alpha, dragBckColor));
            }
            else
            {
                bckColor = new SolidBrush(Color.FromArgb(alpha, BackColor));
            }

            if (BackColor != Color.Transparent | drag)
            {
                g.FillRectangle(bckColor, bounds);
            }

            bckColor.Dispose();
            base.OnPaint(e);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            if (Parent != null)
            {
                Parent.Invalidate(Bounds, true);
            }
            base.OnBackColorChanged(e);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            Invalidate();
            base.OnParentBackColorChanged(e);
        }
    }
}
