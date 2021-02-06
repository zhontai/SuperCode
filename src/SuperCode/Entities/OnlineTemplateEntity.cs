using System;
using Element;
using FreeSql;
using FreeSql.DataAnnotations;


namespace SuperCode.Entities
{
    /// <summary>
    /// 在线模板工具管理
    /// </summary>
	[Table(Name = "code_online_template_tool")]
    [Index("idx_{tablename}_01", nameof(Name), true)]
    public class OnlineTemplateToolEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Column(Position = 1, IsIdentity = true,IsNullable = false)]
        [TableColumn(Text = "主键Id")]
        public long Id { get; set; }

        /// <summary>
        /// 模板工具名
        /// </summary>
        [Column(StringLength = 100, IsNullable = false)]
        [TableColumn(Text = "模板工具名")]
        public string Name { get; set; }

        /// <summary>
        /// 安装命令
        /// </summary>
        [Column(StringLength = 500, IsNullable = false)]
        [TableColumn(Text = "安装命令")]
        public string InstallCommand { get; set; }

        /// <summary>
        /// 卸载命令
        /// </summary>
        [Column(StringLength = 500, IsNullable = false)]
        [TableColumn(Text = "卸载命令")]
        public string UnInstallCommand { get; set; }

        /// <summary>
        /// 创建命令
        /// </summary>
        [Column(StringLength = -1, IsNullable = false)]
        [TableColumn(Text = "创建命令")]
        public string CreateCommand { get; set; }

        [TableColumn(Ignore = true)]
        public bool _InstallLoading = false;

        [TableColumn(Ignore = true)]
        public bool _UnInstallLoading = false;

        [TableColumn(Ignore = true)]
        public bool _CreateLoading = false;
    }

}
