﻿using FS.Sql.Map.Attribute;

namespace FS.Sql.Tests.DB.Members
{
    public class InsertUserVO
    {
        /// <summary> 用户ID </summary>
        [Field(IsOutParam = true)]
        public int? ID { get; set; }

        /// <summary> 用户名 </summary>
        [Field(IsInParam = true)]
        public string UserName { get; set; }

        /// <summary> 密码 </summary>
        [Field(IsInParam = true)]
        public string PassWord { get; set; }
    }
}