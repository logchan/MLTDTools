using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMLTD.DanceExport {
    public interface ILogger {
        void Info(string msg);
        void Warning(string msg);
        void Error(string msg);
    }
}
