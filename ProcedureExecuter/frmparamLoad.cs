using SPBP.Handling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcedureExecuter
{
    public partial class frmparamLoad : Form
    {
        private DataSItem _result;

        private DataSItem _selectedProcedure; 

        public DataSItem Result { get { return _result; } }

      

        public frmparamLoad(DataSItem procedure )
        {
            InitializeComponent();

            _selectedProcedure = procedure;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Contrl contrl in flowLayoutPanel1.Controls)
                {
                    _selectedProcedure.Params[contrl.Description].Value = contrl.Body;
                }

                _result = _selectedProcedure;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception exc )
            {
                MessageBox.Show(exc.ToString());
            }
            
        }

        private async   void frmparamLoad_Load(object sender, EventArgs e)
        {
            try
            {
               
                    await LoadParamView();
              
               
            }
            catch (Exception exc )
            {

                MessageBox.Show(exc.ToString());
            }
             
        }


        #region HelperMethods

        private Task  LoadParamView( )
        {
           
                return Task.Factory.StartNew(InvokedParam);
            
        
           
        }

        private void InvokedParam()
        {
            Action act = new Action(LoadParams);

            if (InvokeRequired)
            {
                Invoke(act);
            }
        }
        
        private void LoadParams()
        {
           
               
                if (_selectedProcedure != null)
                {


                    flowLayoutPanel1.Controls.Clear();
                    foreach (DataParam param in _selectedProcedure.Params.Values)
                    {
                        if (param.Direction == ParamDirection.Input)
                        {
                            Contrl cnt = new Contrl(param.Name, param.Value);
                            flowLayoutPanel1.Controls.Add(cnt);
                        }
                       
                    }



                }
            
           
        }

        #endregion

        private void btnCansel_Click(object sender, EventArgs e)
        {
            this.DialogResult=DialogResult.Cancel;
        }
    }
}
