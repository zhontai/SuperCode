using ElectronNET.API;
using ElectronNET.API.Entities;

namespace SuperCode.Configs
{
    /// <summary>
    /// 设置
    /// </summary>
    public class CodeSettings
    {
        /// <summary>
        /// 窗口关闭时最小化到托盘
        /// </summary>
		public bool CloseToTray { get; set; } = true;

        /// <summary>
        /// 开机自动启动
        /// </summary>
		public bool OpenAtLogin { get; set; } = true;

        /// <summary>
        /// 保存路径
        /// </summary>
		public string SavePath { get; set; }

        /// <summary>
        /// 初始化Db
        /// </summary>
		public bool InitDb { get; set; } = true;
    }
}
