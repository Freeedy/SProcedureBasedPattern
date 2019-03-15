using SPBP;
using SPBP.Connector;
using SPBP.Connector.Class;
using SPBP.Handling;
using SPBP.Modules.SQl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcedureExecuter
{
    public partial class Form1 : Form
    {
        private DbAgent _currentAgent;
        private ProcedureFactory _currentFactory;
        private DataSItem _selectedProcedure;
        private BindingList<DataSItem> _procedures = new BindingList<DataSItem>();

        private bool _isloaded = false;

        public Form1()
        {
            InitializeComponent();
            BtnStates(false);
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                _currentAgent = new DbAgent(txtName.Text, txtConstr.Text, chkState.Checked);
                btnLoad.Enabled = false;
               await  LoadProcedures(_currentAgent);
                _procedures=new BindingList<DataSItem>( _currentFactory.Procedures.Values.ToArray());
                lstProcs.DataSource = _procedures;
                
               
                btnLoad.Enabled = true;
                rTxtresult.AppendColorText(string.Format(" {0} - procedures are  loaded  .  ", _currentFactory.Procedures.Count.ToString()), Color.GreenYellow);
                _isloaded = true;
                BtnStates(true);
            }
            catch (Exception ex )
            {
                rTxtresult.AppendColorText(ex .ToString(),Color.Red);
                BtnStates(false );
            }
           
        }


        #region HelperMethods


        private async Task LoadProcedures( DbAgent agent )
        {

            Stopwatch sw = new Stopwatch();
            sw .Start();
                    _currentFactory = await  SqlManager.GetProceduresFactoryAsync(agent);

            sw.Stop();
            Console.WriteLine("GetProceduresFactoryAsync - Time : "+ sw.ElapsedMilliseconds.ToString());
        }
      
        #endregion 

        private void lstProcs_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedProcedure = (lstProcs.SelectedItem as DataSItem);

            if (lstProcs.Items.Count < 1)
            {
                BtnStates(false );
            }
        }

      

        private async void btnAddreturnparam_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatues.Text = "adding ....";
                rTxtresult.AppendColorText("Adding  return parameter to procedures ",Color.Green);
                btnAddreturnparam.Enabled = false;
                await _currentFactory.AddReturnValueToEachProcedureAsync();
                btnAddreturnparam.Enabled = true;
                lblStatues.Text = "Done.";
                rTxtresult.AppendColorText("All procedures have return parameters", Color.Green);
                btnAddreturnparam.Enabled = false;
            }
            catch (Exception exc )
            {
                btnAddreturnparam.Enabled = true;
                rTxtresult.AppendColorText(exc .ToString() , Color.Red );
            }
           
        }

        private async void btnExecuteDs_Click(object sender, EventArgs e)
        {
            try
            {

                if (lstProcs.SelectedItems.Count > 0)
                {
                    _selectedProcedure = (lstProcs.SelectedItem as DataSItem);
                }

                DataSItem temp = _selectedProcedure; 
                btnExecuteDs.Enabled = false;
               
              
                await ExecuteDs(temp );

                btnExecuteDs.Enabled = true;
               
            }
            catch (Exception exc )
            {
                rTxtresult.AppendColorText(exc.ToString() ,Color.Red);
                btnExecuteDs.Enabled = true;
            }
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rTxtresult.Clear();
            dataGridSet.DataBindings.Clear();
            dataGridSet.DataSource = null;
        }

        private void rTxtresult_TextChanged(object sender, EventArgs e)
        {
            rTxtresult.ScrollToCaret();
        }


        private void BtnStates(bool state )
        {
            btnAddreturnparam.Enabled = btnExecuteDs.Enabled = btnExecuteNonQ.Enabled = state;
        }

        #region  HelperMethods 

       

        private async Task ExecuteDs( DataSItem temp )
        {
            using (frmparamLoad load = new frmparamLoad(temp))
            {
                if (load.ShowDialog() == DialogResult.OK)
                {
                    if (load.Result != null)
                    {
                       
                                _selectedProcedure = load.Result;

                                  lblStatues.Text = "Loading dataSetExecution ...";
                                ExecAsyncResult result = await _selectedProcedure.ExecDataSetAsync(_currentAgent);
                                lblStatues.Text = "Done.";
                                rTxtresult.AppendColorText(result.ToString(), Color.Blue);

                                DataSet rsSet = result.Object as DataSet;

                                if (rsSet.Tables.Count > 0)
                                {
                                    dataGridSet.DataSource = rsSet.Tables[0];
                                }
                                if (_selectedProcedure.HasOutputParam)
                                {
                                    rTxtresult.AppendColorText("Output Params", Color.GreenYellow);
                                    foreach (DataParam dataParam in _selectedProcedure.OutputParams.Values)
                                    {
                                        rTxtresult.AppendColorText(dataParam.Name + " = " + dataParam.Value, Color.Green);
                                    }
                                    rTxtresult.AppendColorText(new string('-', 30), Color.GreenYellow);
                                }

                              
                           
                       


                    }
                }
            }
        }

        private async Task ExecuteNonQuery(DataSItem temp)
        {
            using (frmparamLoad load = new frmparamLoad(_selectedProcedure))
            {
                if (load.ShowDialog() == DialogResult.OK)
                {
                    if (load.Result != null)
                    {

                        _selectedProcedure = load.Result;

                        lblStatues.Text = "Loading NonQuery procedure ";
                        ExecAsyncResult result = await _selectedProcedure.ExecuteNonQueryAsync(_currentAgent);


                        lblStatues.Text = "Done.";
                        rTxtresult.AppendColorText(result.ToString(), Color.Blue);


                        if (_selectedProcedure.HasOutputParam)
                        {
                            rTxtresult.AppendColorText("Output Params", Color.GreenYellow);
                            foreach (DataParam dataParam in _selectedProcedure.OutputParams.Values)
                            {
                                rTxtresult.AppendColorText(dataParam.Name + " = " + dataParam.Value, Color.Green);
                            }
                            rTxtresult.AppendColorText(new string('-', 30), Color.GreenYellow);
                        }






                    }
                }
            }

        }

       

        #endregion

        private async  void btnExecuteNonQ_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstProcs.SelectedItems.Count>0)
                {
                    _selectedProcedure = (lstProcs.SelectedItem as DataSItem);
                }

                DataSItem temp = _selectedProcedure; 
                btnExecuteNonQ.Enabled = false;

                await ExecuteNonQuery(temp);



                btnExecuteNonQ.Enabled = true;

            }
            catch (Exception exc)
            {
                rTxtresult.AppendColorText(exc.ToString(), Color.Red);
                btnExecuteNonQ.Enabled = true;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmAbout about =new frmAbout())
            {
                about.ShowDialog();
            }
        }

     
    }

    
   
}

