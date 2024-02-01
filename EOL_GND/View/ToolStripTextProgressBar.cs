using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace EOL_GND.View
{
    /// <summary>
    /// 컨트롤의 가운데 텍스트를 보여주는 Progress Bar.
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
    public class ToolStripTextProgressBar : ToolStripControlHost
    {
        public ToolStripTextProgressBar() : base(new TextProgressBar())
        {
        }

        [Category("Behavior")]
        [Description("The speed of the marquee animation in milliseconds.")]
        [DefaultValue(100)]
        public int MarqueeAnimationSpeed
        {
            get => ((TextProgressBar)Control).MarqueeAnimationSpeed;
            set => ((TextProgressBar)Control).MarqueeAnimationSpeed = value;
        }

        [Category("Behavior")]
        [Description("The upper bound of the range this ProgressBar is working with.")]
        [DefaultValue(100)]
        public int Maximum
        {
            get => ((TextProgressBar)Control).Maximum;
            set => ((TextProgressBar)Control).Maximum = value;
        }

        [Category("Behavior")]
        [Description("The lower bound of the range this ProgressBar is working with.")]
        [DefaultValue(0)]
        public int Minimum
        {
            get => ((TextProgressBar)Control).Minimum;
            set => ((TextProgressBar)Control).Minimum = value;
        }

        [Category("Behavior")]
        [Description("The current value for the ProgressBar, in the range specified by the minimum and maximum properties.")]
        [DefaultValue(0)]
        public int Value
        {
            get => ((TextProgressBar)Control).Value;
            set => ((TextProgressBar)Control).Value = value;
        }

        [Category("Behavior")]
        [Description("The amount to increment the current value of the control by when the PerformStep() method is called.")]
        [DefaultValue(10)]
        public int Step
        {
            get => ((TextProgressBar)Control).Step;
            set => ((TextProgressBar)Control).Step = value;
        }

        [Category("Behavior")]
        [Description("This property allows the user to set the style of the ProgressBar.")]
        [DefaultValue(ProgressBarStyle.Blocks)]
        public ProgressBarStyle Style
        {
            get => ((TextProgressBar)Control).Style;
            set => ((TextProgressBar)Control).Style = value;
        }

        [Browsable(true)]
        public override string Text
        { 
            get => base.Text; 
            set
            {
                base.Text = value;
                ((TextProgressBar)Control).Text = value;
                Invalidate();
            }
        }

        [Category("Appearance"), DefaultValue(true)]
        [Description("Indicates whether the displayed text is automatically generated from the value.")]
        public bool AutoText
        {
            get => ((TextProgressBar)Control).AutoText;
            set => ((TextProgressBar)Control).AutoText = value;
        }
    }
}
