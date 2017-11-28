using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Redis.Infrastructure
{
    public enum eumSetType
    {
        /// <summary>
        /// 不限制
        /// </summary>
        None,
        /// <summary>
        /// 只在键不存在时，才对键进行设置操作。
        /// </summary>
        NX,
        /// <summary>
        /// 只在键已经存在时，才对键进行设置操作。
        /// </summary>
        XX
    }
}
