using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMLTD.DanceExport {
    public sealed class Config {
        public string CacheFolder { get; set; }
        public WindowConfig LastWindow { get; set; } = new WindowConfig();
    }
}
