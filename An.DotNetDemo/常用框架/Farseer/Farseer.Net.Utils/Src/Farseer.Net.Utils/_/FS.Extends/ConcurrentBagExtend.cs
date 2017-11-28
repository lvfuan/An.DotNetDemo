using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class ConcurrentBagExtend
    {
        /// <summary>
        ///     清除对象的数据
        /// </summary>
        public static void Clear<TEntity>(this ConcurrentBag<TEntity> lst)
        {
            TEntity entity;
            while (lst.TryTake(out entity)) { }
        }
    }
}