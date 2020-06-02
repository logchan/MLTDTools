using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenMLTD.DanceExport {
    public sealed class WindowConfig {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public FormWindowState State { get; set; } = FormWindowState.Normal;

        public void SetFrom(Form frm) {
            Top = frm.Top;
            Left = frm.Left;
            Width = frm.Width;
            Height = frm.Height;
            State = frm.WindowState;
        }

        public void RestoreForm(Form frm) {
            if (Width <= 0) {
                return;
            }

            frm.StartPosition = FormStartPosition.Manual;
            frm.WindowState = State;
            frm.Width = Width;
            frm.Height = Height;
            frm.Top = Top;
            frm.Left = Left;
        }
    }
}
