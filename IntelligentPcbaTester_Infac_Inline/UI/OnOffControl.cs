using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class OnOffControl : UserControl
    {
        private const string OnOffCategory = "ON/OFF State";

        [Category(OnOffCategory), DefaultValue(typeof(Color), "#FF32CD32"), 
            Description("Indicates a LED color of ON state.")]
        public Color OnColor
        {
            get => _onColor;
            set
            {
                if (_onColor != value)
                {
                    _onColor = value;
                    if (ON)
                    {
                        UpdateOnOffUI();
                    }
                }
            }
        }
        private Color _onColor = Color.LimeGreen;

        [Category(OnOffCategory), DefaultValue(typeof(Color), "#FFC0C0C0"), 
            Description("Indicates a LED color of OFF state.")]
        public Color OffColor
        {
            get => _offColor;
            set
            {
                if (_offColor != value)
                {
                    _offColor = value;
                    if (!ON)
                    {
                        UpdateOnOffUI();
                    }
                }
            }
        }
        private Color _offColor = Color.Silver;

        [Category(OnOffCategory), DefaultValue("ON"), 
            Description("Indicates a title of ON state.")]
        public string OnTitle
        {
            get => _onTitle;
            set
            {
                if (_onTitle != value)
                {
                    _onTitle = value;
                    if (ON)
                    {
                        UpdateOnOffUI();
                    }
                }
            }
        }
        private string _onTitle = "ON";

        [Category(OnOffCategory), DefaultValue("OFF"), 
            Description("Indicates a title of OFF state.")]
        public string OffTitle
        {
            get => _offTitle;
            set
            {
                if (_offTitle != value)
                {
                    _offTitle = value;
                    if (!ON)
                    {
                        UpdateOnOffUI();
                    }
                }
            }
        }
        private string _offTitle = "OFF";

        [Category(OnOffCategory), DefaultValue(false), 
            Description("Indicates a ON/OFF state.")]
        public bool ON
        {
            get => _on;
            set
            {
                if (_on != value)
                {
                    _on = value;
                    UpdateOnOffUI();
                }
            }
        }
        private bool _on = false;

        public OnOffControl()
        {
            InitializeComponent();
        }

        private void UpdateOnOffUI()
        {
            if (ON)
            {
                colorLabel.BackColor = OnColor;
                titleLabel.Text = OnTitle;
            }
            else
            {
                colorLabel.BackColor = OffColor;
                titleLabel.Text = OffTitle;
            }
        }
    }
}
