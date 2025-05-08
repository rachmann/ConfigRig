using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigRig.Lib.Models
{
    public class ConfigSection
    {
        public Dictionary<string, object> Values { get; set; } = new();
        public Dictionary<string, ConfigSection> Subsections { get; set; } = new();
    }

}
