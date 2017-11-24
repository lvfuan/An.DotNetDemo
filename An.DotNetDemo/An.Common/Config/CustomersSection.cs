using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace An.Common.Config
{
    public class CustomersSection : ConfigurationSection
    {
        [ConfigurationProperty("Customers", IsRequired = true)]
        public string Category
        {
            get
            {
                return (string)base["Category"];
            }
            set
            {
                base["Category"] = value;
            }
        }
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public CustomerElementCollection Customers
        {
            get
            {
                return (CustomerElementCollection)base[""];
            }
        }
    }
}
