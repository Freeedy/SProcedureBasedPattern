using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcedureExecuter
{
  public  static class Helper
    {
     
      public static void AppendColorText(this RichTextBox box, string text, Color color)
      {
          box.SelectionStart = box.TextLength;
          box.SelectionLength = 0;
          Color b = box.ForeColor;
          box.SelectionColor = color;
          box.AppendText(text+"\n");
          box.SelectionColor = box.ForeColor;
      }
    }
}
