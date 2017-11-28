#if IsWeb
using System;
using System.Collections.Generic;
using System.Web;
using FS.Infrastructure;
using FS.Utils.Common;

namespace FS.Extends
{
    public static partial class UtilsExtend
    {

//#if !IsMvc
//        /// <summary>
//        ///     通用的分页方法(多条件)
//        /// </summary>
//        /// <param name="set">ISet接口</param>
//        /// <param name="rpt">Repeater带分页控件</param>
//        /// <typeparam name="TEntity">实体类</typeparam>
//        public static List<TEntity> ToList<TEntity>(this IDbSet<TEntity> set, Repeater rpt) where TEntity : class, IEntity, new()
//        {
//            int recordCount;
//            var lst = set.ToList(rpt.PageSize, rpt.PageIndex, out recordCount);
//            rpt.PageCount = recordCount;

//            return lst;
//        }
//#endif
        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="set">ISet接口</param>
        /// <param name="tip">弹出框事务委托</param>
        public static TEntity Form<TEntity>(this IDbSet<TEntity> set, Action<Dictionary<string, List<string>>> tip, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.Form, tip, prefix);
        }

        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="set">ISet接口</param>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public static TEntity Form<TEntity>(this IDbSet set, out Dictionary<string, List<string>> dicError, string prefix = "hl") where TEntity : class, new()
        {
            return Req.Fill<TEntity>(HttpContext.Current.Request.Form, out dicError, prefix);
        }
    }
}
#endif