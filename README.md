# 介绍

SuperCode是支持跨平台的客户端，主要用来生成中台Admin前后端分离代码。使用到的技术包括Net5、Blazor Server、Electron、FreeSql

## 开发环境

> 开发工具
>
>  [Visual Studio 2019](https://visualstudio.microsoft.com/zh-hans/downloads/)
> 
> SDK
>
>  [.Net 5](https://dotnet.microsoft.com/download/dotnet-core)
>  
> Nuget包 
>
> [Electron.NET](https://github.com/ElectronNET/Electron.NET)、[FreeSql](https://github.com/2881099/FreeSql)、Blazor Server、Element
> 
> 内置轻量级
>
> Sqlite
>  

## 内置功能

* Blazor多界面标签路由功能，支持选项卡和导航菜单相互联动
* 数据库连接管理，支持数据库连接测试功能
* 在线模板工具管理，支持模板工具的安装、卸载、创建功能，支持创建后自动打开所在代码文件目录
* 系统设置支持开机自动启动，窗口关闭时最小化到托盘，自定义代码文件保存路径和打开目录功能

## 开源地址

* github： [https://github.com/zhontai/SuperCode](https://github.com/zhontai/SuperCode)
* Gitee： [https://gitee.com/zhontai/SuperCode](https://gitee.com/zhontai/SuperCode)

## 快速上手

*********************************************************
### 项目下载后，首先进入SuperCode目录下安装依赖包

```
npm install
或
npm install --registry=https://registry.npm.taobao.org
```

### 提示
>如果安装eletron在node install.js特别慢，打开命令台运行npm config edit，
>在.npmrc文件中增加ELECTRON_MIRROR="https://npm.taobao.org/mirrors/electron/"保存并退出

### 安装完成后，调试命令选择SuperCode Watch编译运行
调试命令说明
* SuperCode Watch 编译运行SuperCode客户端并监听文件修改自动编译，速度较快，测试客户端推荐。
* SuperCode Start 编译运行SuperCode客户端，速度较慢。
* SuperCode Build 打包生成安装软件和绿色版文件。
* SuperCode Web 编译运行成Web浏览模式，不能使用Electron功能，速度快，测试Web端推荐。


生成其它平台时，修改SuperCode Build命令：
```
electronize build /target win
electronize build /target osx
electronize build /target linux
```

### 提示
>修改样式less文件，需要打开任务运行资源管理器，选择任务/less右键运行生成css文件，再刷新Web项目
