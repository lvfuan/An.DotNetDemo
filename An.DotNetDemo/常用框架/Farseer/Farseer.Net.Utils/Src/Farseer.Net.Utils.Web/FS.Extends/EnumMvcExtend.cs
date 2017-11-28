using System;
using System.Collections.Generic;
using FS.Cache;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static class EnumMvcExtend
    {

    /// <summary>
    ///     枚举转ListItem
    /// </summary>
        public static List<System.Web.Mvc.SelectListItem> ToSelectListItem(this Type enumType)
        {
            var lst = new List<System.Web.Mvc.SelectListItem>();
            foreach (int value in Enum.GetValues(enumType))
            {
                lst.Add(new System.Web.Mvc.SelectListItem
                {
                    Value = value.ToString(),
                    Text = ((Enum)Enum.ToObject(enumType, value)).GetName()
                });
            }
            return lst;
        }

    }
}