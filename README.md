## 前言

这个2D回合制游戏项目算是自己的练手之作，使用[GameFramework](https://gameframework.cn/)制作。目前已弃坑良久，而且我的技术力也不高，但终究是我花费了很多心力制作的项目。不想就这样白费所以便将源码分享出来，希望可以帮到一些朋友。项目中使用的收费插件是我自己购买的，仅供学习使用，如有纠纷，概不负责。[项目地址：GitHub - FinallyDusk/2DGameProject](https://github.com/FinallyDusk/2DGameProject)

## 开始

#### 游戏游玩说明

找到`Asset/Game/Scene/GameTitle`,打开这个场景就可以直接运行了。

目前只有**开始游戏**按钮有作用，其他的都没有做。

进入游戏可以ESC呼出菜单。目前玩家有两个单位，可以在`PreLoadProcedure.LoadPlayerUnit`中找到，由于还没写存档机制，所以先使用固定写法。

在菜单-背包选项中，按下q键可以加载两件装备（需要切换选项卡触发页面刷新）。装备是固定+随机属性的形式。

最后主界面战斗，移动到别的单位上去，即进入战斗。

战斗采用速度回合制，目前只制作了攻击、移动和技能。每个操作会有不同的速度值损耗，可以从`Config/GameMain`中进行修改。

#### 代码说明

可以通过查看Procedure中的入口流程`GameTitleProcedure`进行查看，主要就是读表然后初始化各个系统。其中战斗的技能是通过xLua去写的，个人觉得lua不需要那么多定义类型，方便一点。

关于数据表的编写，是自己写的一个简单的jsonhelper，也内置了一个数据表编辑器（`Tools\Custom\Data Window`）,Data Window本身是一个数据表总览窗口，可以添加数据类型（实现了UnityGameFramewor.Runtime.DataRowBase的类）和数据文件的映射关系，然后通过点击文件夹图标可以查看具体属性，此编辑器使用Odin完成。

## 一些问题

刚开始做的时候，以为需要自己新建一个单位（Unit）对象，到现在才发现GF的Entity本身就可以当做一个单位对象，并不是只是显示用的，数据可以存储在Entity，亏我还多此一举的新建了一个UnitSystem。


