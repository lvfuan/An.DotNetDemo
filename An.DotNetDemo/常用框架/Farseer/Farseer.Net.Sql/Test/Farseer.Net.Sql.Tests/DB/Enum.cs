﻿using System.ComponentModel.DataAnnotations;

namespace FS.Sql.Tests.DB
{
    /// <summary>
    ///     用户组
    /// </summary>
    public enum eumGenderType : byte
    {
        /// <summary>
        ///     男士
        /// </summary>
        [Display(Name = "男士")] Man = 0,

        /// <summary>
        ///     女士
        /// </summary>
        [Display(Name = "女士")] Woman
    }
}