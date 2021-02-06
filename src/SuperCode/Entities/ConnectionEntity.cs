using System;
using Element;
using FreeSql;
using FreeSql.DataAnnotations;


namespace SuperCode.Entities
{
    /// <summary>
    /// 连接管理
    /// </summary>
	[Table(Name = "code_connection")]
    [Index("idx_{tablename}_01", nameof(ConnectionName), true)]
    public class ConnectionEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Column(Position = 1, IsIdentity = true,IsNullable = false)]
        [TableColumn(Text = "主键Id")]
        public long Id { get; set; }

        /// <summary>
        /// 连接名
        /// </summary>
        [Column(StringLength = 100, IsNullable = false)]
        [TableColumn(Text = "连接名")]
        public string ConnectionName { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        [Column(MapType = typeof(int), IsNullable = false)]
        [TableColumn(Text = "数据库类型", Ignore = false)]
        public DataType DbType { get; set; }

        //[Column(IsIgnore = true)]
        //[TableColumn(Text = "数据库类型")]
        //public string DbTypeName { get { return DbType.ToDescriptionOrString(); } }

        /// <summary>
        /// 连接字符串
        /// </summary>
        [Column(StringLength = -1, IsNullable = false)]
        [TableColumn(Text = "连接字符串")]
        public string ConnectionString { get; set; }


        [TableColumn(Ignore = true)]
        public bool _TestConnectioinLoading = false;
    }

}
