using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcedureExecuter
{
    public partial class Contrl : UserControl
    {
        public string Description { get { return lblDescription.Text; } set { lblDescription.Text = value; } }

        public string Body { get { return txtBody.Text; } set { txtBody.Text = value; } }

        public Contrl(string desc, string body)
        {
            InitializeComponent();
            Description = desc;
            Body = body;
        }

        public Contrl() 
        {
            InitializeComponent();
        }
    }
}
