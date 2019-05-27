using System;
using System.Windows.Forms;

namespace IKSIS_GAME_WF
{
    public static class Extensions
    {
        public static void InvokeFix(this Control control, Action action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else action.Method.Invoke(action.Target, null);
        }
    }


}
