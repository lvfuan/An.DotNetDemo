using System.ComponentModel.DataAnnotations;

namespace FS.Sql.Data
{
    /// <summary> ���ݿ����� </summary>
    public enum eumSqlDbType
    {
        /// <summary> SqlServer���ݿ� </summary>
        [Display(Name = "System.Data.SqlClient")] SqlServer,

        /// <summary> Access���ݿ� </summary>
        [Display(Name = "System.Data.OleDb")] OleDb,

        /// <summary> MySql���ݿ� </summary>
        [Display(Name = "MySql.Data.MySqlClient")] MySql,

        /// <summary> SQLite </summary>
        [Display(Name = "System.Data.SQLite")] SQLite,

        /// <summary> Oracle </summary>
        [Display(Name = "System.Data.OracleClient")] Oracle,
    }
}