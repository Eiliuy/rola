# Rola

一款受《艾希》启发的 2D 横版动作游戏（ACT），融合快节奏近战连招、轻度 Roguelike Build 构筑与模块化角色外观。

## 目标平台

- PC（Windows / macOS / Linux）
- 移动端（iOS / Android）

## 技术栈

- 引擎：Unity 2022.3 LTS
- 语言：C#
- 物理：Rigidbody2D + Collider2D
- 动画：Unity Animator
- UI：Unity uGUI

## 当前阶段

核心战斗原型已验证完成，正在填充：

- 第一个可玩职业「剑士」的具体数值与技能
- 模块化角色外观系统（占位资源 + 换装验证）
- 美术资源、动画状态机与关卡内容

## 项目结构

```
Assets/
├── Scripts/          # C# 脚本（Core / Player / Enemy / UI / Effects）
├── Art/              # 美术资源（Sprites / Tilesets / Effects / UI）
├── Audio/            # 音效与音乐
├── Prefabs/          # 预制体
├── Scenes/           # 场景文件
├── Resources/        # 动态加载资源
├── ScriptableObjects/# 数据资产（职业 / 技能 / 装备 / 升级 / 外观）
└── Editor/           # 编辑器扩展与辅助工具
docs/                 # 项目文档（当前状态、路线图、缺口、设计计划）
```

## AI 辅助开发

项目计划通过 [Funplay Unity MCP](https://github.com/FunplayAI/funplay-unity-mcp) 将 OpenCode 与本地 Unity Editor 连接，实现：

- 读取场景层级与组件状态
- 创建 / 修改 ScriptableObject 资产
- 运行 Play Mode 测试并获取日志

> 目前 Unity Editor 与 MCP 的安装、激活配置计划在个人 PC（带 GPU/显示器）上完成，服务器端仅负责代码与文档维护。

## 文档

- [当前状态](docs/current-state.md)
- [后续迭代方向](docs/roadmap.md)
- [目前还欠缺什么](docs/gaps.md)
- [视觉系统旧逻辑清理计划](docs/visual-system-cleanup-plan.md)
- [剑士职业设计计划](docs/swordsman-class-plan.md)
