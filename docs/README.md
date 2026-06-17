# Rola 项目文档

本目录是 Rola 项目（2D 横版动作 Roguelike）的文档入口。新成员或 Agent 接入时，建议按以下顺序阅读。

## 必读文档

1. **[项目规范 / 根目录 `AGENTS.md`](../AGENTS.md)**
   - 技术栈、项目结构、编码规范、Git 提交规范。

2. **[项目当前状态](current-state.md)**
   - 已完成系统清单，以及目前跑通到什么程度。

3. **[主路线图 / 根目录 `ROADMAP.md`](../ROADMAP.md)**
   - 后续开发阶段与当前优先级最高的任务。

## 待办与问题

- **[目前欠缺 / gaps](gaps.md)**：长期待解决问题、系统功能、美术/音频、测试与发布相关。
- **[当前计划](swordsman-class-plan.md)**：唯一 active 的实施计划（剑士职业落地）。

## 工具参考

- **[脚本速查 / 根目录 `SCRIPTS_REFERENCE.md`](../SCRIPTS_REFERENCE.md)**：49 个脚本的用途、关键字段、常见调整项。
- **[美术资源规范 / 根目录 `ASSETS_GUIDE.md`](../ASSETS_GUIDE.md)**：Sprite、Prefab、场景、动画导入规范。
- **[Agent Unity MCP 工作流 / 根目录 `CLAUDE.md`](../CLAUDE.md)**：Funplay Unity MCP 的使用方式与验证流程。

## 注意事项

- `docs/` 下只保留 **当前状态**、**长期 gaps**、**当前 active 计划** 三类文档。
- 已实施完成的历史设计计划不会留档在此，如需追溯请查看 Git 历史。
- 新增系统或重要修改后，请同步更新 `current-state.md` 和 `ROADMAP.md`。
