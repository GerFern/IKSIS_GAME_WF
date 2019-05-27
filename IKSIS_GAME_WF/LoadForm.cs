using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKSIS_GAME_WF
{
    public partial class MsgForm : MyForm
    {
        //public enum LoadFormButtons
        //{
        //    None,
        //    Cont,
        //    Cancel
        //}

        public enum MsgFormButton
        {
            //None,
            Cont,
            Cancel,
        }

        Button bCont = new Button { Dock = DockStyle.Fill ,Text = "Продолжить", Tag = MsgFormButton.Cont };
        private Button bCancel = new Button { Dock = DockStyle.Fill, Text = "Отмена", Tag = MsgFormButton.Cancel };

        List<MsgFormButton> _buttons;

        public List<MsgFormButton> Buttons
        {
            get => _buttons;
            set
            {
                if(_buttons!=value)
                {
                    _buttons = value;
                    tableLayoutPanel2.Controls.Clear();
                    if (value == null || value.Count == 0) splitContainer1.Panel2Collapsed = true;
                    else
                    {
                        tableLayoutPanel2.ColumnCount = value.Count;
                        splitContainer1.Panel2Collapsed = false;
                        for (int i = 0; i < value.Count; i++)
                        {
                            switch (value[i])
                            {
                                case MsgFormButton.Cont:
                                    tableLayoutPanel2.Controls.Add(bCont, i, 0);
                                    break;
                                case MsgFormButton.Cancel:
                                    tableLayoutPanel2.Controls.Add(bCancel, i, 0);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    //else
                    //{
                    //    int i = 0;
                    //    if(value.)
                    //            tableLayoutPanel2.ColumnCount = 1;
                    //            tableLayoutPanel2.Controls.Add(bCont);
                    //}
                }
            }
        }

        public string Message
        {
            get => label1.Text;
            set => label1.Text = value;
        }
        public MsgForm()
        {
            InitializeComponent();
            bCont.Click += Btn_Click;
            bCancel.Click += Btn_Click;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if(((Control)sender).Tag is MsgFormButton btn)
            {
                ButtonClick?.Invoke(this, new EventArgsBtn(btn));
            }
        }

        public class EventArgsBtn : EventArgs
        {
            public MsgFormButton Button { get; }
            public EventArgsBtn(MsgFormButton button) => Button = button;
        }

        public event EventHandler<EventArgsBtn> ButtonClick;
    }
}
