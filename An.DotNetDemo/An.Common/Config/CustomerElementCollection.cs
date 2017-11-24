using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace An.Common.Config
{
      public class CustomerElementCollection: ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomerElement)element).Key;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "Customer";
            }
        }
        public CustomerElement this[int index]
        {
            get
            {
                return (CustomerElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
        public CustomerElement this[string key]
        {
            get
            {
                return (CustomerElement)BaseGet(key);
            }
        }           
    }
}
