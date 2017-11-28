using System;

namespace FS.Utils.Component
{
    /// <summary>
    ///     申请代码审查
    /// </summary>
    public class ApplyCheck : Attribute
    {
        /// <summary>
        ///     申请代码审查
        /// </summary>
        /// <param name="isApplyCheck">是否申请代码审查</param>
        /// <param name="applyVer">申请版本</param>
        /// <param name="checkVer">检查版本</param>
        public ApplyCheck(bool isApplyCheck, int applyVer, int checkVer)
        {
            ApplyVer = applyVer;
            CheckVer = checkVer;
            IsApplyCheck = isApplyCheck;
        }

        /// <summary>
        ///     申请版本
        /// </summary>
        public int ApplyVer { get; set; }

        /// <summary>
        ///     检查版本
        /// </summary>
        public int CheckVer { get; set; }

        /// <summary>
        ///     是否申请代码审查
        /// </summary>
        public bool IsApplyCheck { get; set; }
    }
}