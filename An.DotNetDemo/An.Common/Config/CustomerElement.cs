using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace An.Common.Config
{
     public class CustomerElement: ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get
            {
                return (string)base["key"];
            }

            set
            {
                base["key"] = value;
            }
        }
        [ConfigurationProperty("value", IsRequired = true)]

        public double Value
        {
            get
            {
                return (double)base["value"];
            }
            set
            {
                base["value"] = value;
            }
        }

    }
}
