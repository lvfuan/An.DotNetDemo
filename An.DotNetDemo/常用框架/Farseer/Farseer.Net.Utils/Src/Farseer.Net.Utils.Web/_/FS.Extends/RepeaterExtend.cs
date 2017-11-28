#if IsWeb
using System.Collections;
using FS.Utils.UI;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     IEnumerable绑定到Repeater
        /// </summary>
        /// <param name="rpt">QynRepeater</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="lst">IEnumerable</param>
        public static void Bind(this Repeater rpt, IEnumerable lst, int recordCount = -1)
        {
            rpt.DataSource = lst;
            rpt.DataBind();

            if (recordCount > -1)
            {
                rpt.PageCount = recordCount;
            }
        }
    }
}
#endif