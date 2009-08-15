using System;
using System.Windows.Forms;
using System.Text;

namespace MadScience.Wrappers
{
    public partial class RCOLHeaderEditor : Form
    {
        public MadScience.Wrappers.RcolHeader rcolHeader = new RcolHeader();

        public RCOLHeaderEditor()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            // Internal chunks
            this.rcolHeader.internalChunks.Clear();
            foreach (string line in txtInternalChunks.Lines)
            {
                if (!String.IsNullOrEmpty(line.Trim()))
                {
                    if (MadScienceSmall.Helpers.validateKey(line))
                    {
                        this.rcolHeader.internalChunks.Add(new MadScience.Wrappers.ResourceKey(line, (int)ResourceKeyOrder.ITG));
                    }
                }
            }

            // External chunks
            this.rcolHeader.externalChunks.Clear();
            foreach (string line in txtExternalChunks.Lines)
            {
                if (!String.IsNullOrEmpty(line.Trim()))
                {
                    if (MadScienceSmall.Helpers.validateKey(line))
                    {
                        this.rcolHeader.externalChunks.Add(new MadScience.Wrappers.ResourceKey(line, (int)ResourceKeyOrder.ITG));
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            //this.Hide();
        }

        private void RCOLHeaderEditor_Load(object sender, EventArgs e)
        {
            txtVersion.Text = this.rcolHeader.version.ToString();
            txtDatatype.Text = this.rcolHeader.datatype.ToString();

            StringBuilder sb = new StringBuilder();

            // Internal chunks
            for (int i = 0; i < this.rcolHeader.internalChunks.Count; i++)
            {
                sb.AppendLine(this.rcolHeader.internalChunks[i].ToString());
            }
            txtInternalChunks.Text = sb.ToString();

            sb = new StringBuilder();
            // External chunks
            for (int i = 0; i < this.rcolHeader.externalChunks.Count; i++)
            {
                sb.AppendLine(this.rcolHeader.externalChunks[i].ToString());
            }
            txtExternalChunks.Text = sb.ToString();
        }

    }
}
