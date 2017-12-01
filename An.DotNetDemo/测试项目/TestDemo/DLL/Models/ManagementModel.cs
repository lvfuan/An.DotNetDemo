using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    /// <summary>
    /// 管理员
    /// </summary>
     public class ManagementModel:BaseModel
    {
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string LoginId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string LoginPwd { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsState { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDelete { get; set; }
    }
}
