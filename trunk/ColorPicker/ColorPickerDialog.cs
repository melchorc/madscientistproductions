using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ColorPicker
{
	public partial class ColorPickerDialog : Form
	{
        Color _color = new Color();
        public Color Color
        {
            get { return this._color; }
            set { 
                this._color = value;
                m_colorPicker.SelectedColor = value;
            }
        }

        /*
        int _Alpha = 0;
        public int Alpha
        {
            get { return this._Alpha; }
            set
            {
                this._Alpha = value;
                m_colorPicker.SelectedAlpha = value;
            }
        }
        */
		public ColorPickerDialog()
		{
			InitializeComponent();
		}

		private void OnSelected(object sender, TabControlEventArgs e)
		{
			if (e.TabPage == m_knownColorsTabPage)
				m_colorList.SelectColor(m_colorPicker.SelectedColor);
			if (e.TabPage == m_colorTabPage)
				m_colorPicker.SelectedColor = (Color)m_colorList.SelectedItem;
		}

        private void m_ok_Click(object sender, EventArgs e)
        {
            
            if (m_tabControl.SelectedTab == m_colorTabPage)
            {
                this._color = m_colorPicker.SelectedColor;
            }
            else
            {
                this._color = (Color)m_colorList.SelectedItem;
            }
                //this._Alpha = m_colorPicker.SelectedAlpha;
        }

	}
}