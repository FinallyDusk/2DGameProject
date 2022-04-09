using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    using UnityGameFramework.Runtime;
	public partial class GameEntry
    {
        public static EventComponent Event { get; private set; }

        public static DataTableComponent DataTable { get; private set; }

        public static ProcedureComponent Procedure { get; private set; }

        public static EntityComponent Entity { get; private set; }

        public static UIComponent UI { get; private set; }

        public static DataNodeComponent DataNode { get; private set; }

        public static BaseComponent Base { get; private set; }

        public static ResourceComponent Resource { get; private set; }

        public static LocalizationComponent Localization { get; private set; }

        public static ConfigComponent Config { get; private set; }

        public static ObjectPoolComponent Pool { get; private set; }

        public static SoundComponent Sound { get; private set; }

        public static FsmComponent FSM { get; private set; }

        public static GameCheckpointSystem Checkpoint { get; private set; }

        public static UnitSystem Unit { get; private set; }

        public static UnitPropSystem UnitProp { get; private set; }

        public static PlayerInputSystem Input { get; private set; }

        public static SpriteSystem Sprite { get; private set; }

        public static BackpackSystem Backpack { get; private set; }

        public static SkillSystem UnitSkill { get; private set; }

        public static CombatSystem Combat { get; private set; }

        public static LuaComponent Lua { get; private set; }

        public static SpecialEffectSystem SpecialEffect { get; private set; }

        public static GameTimerSystem GameTime { get; private set; }

        /// <summary>
        /// 初始化组件
        /// </summary>
        public static void InitComponent()
        {
            Event = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            DataTable = UnityGameFramework.Runtime.GameEntry.GetComponent<DataTableComponent>();
            Procedure = UnityGameFramework.Runtime.GameEntry.GetComponent<ProcedureComponent>();
            Entity = UnityGameFramework.Runtime.GameEntry.GetComponent<EntityComponent>();
            UI = UnityGameFramework.Runtime.GameEntry.GetComponent<UIComponent>();
            DataNode = UnityGameFramework.Runtime.GameEntry.GetComponent<DataNodeComponent>();
            Base = UnityGameFramework.Runtime.GameEntry.GetComponent<BaseComponent>();
            Resource = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            Localization = UnityGameFramework.Runtime.GameEntry.GetComponent<LocalizationComponent>();
            Config = UnityGameFramework.Runtime.GameEntry.GetComponent<ConfigComponent>();
            Pool = UnityGameFramework.Runtime.GameEntry.GetComponent<ObjectPoolComponent>();
            Sound = UnityGameFramework.Runtime.GameEntry.GetComponent<SoundComponent>();
            FSM = UnityGameFramework.Runtime.GameEntry.GetComponent<FsmComponent>();
            Lua = UnityGameFramework.Runtime.GameEntry.GetComponent<LuaComponent>();

            GameTime = new GameTimerSystem();
            GameTime.InitSystem();
            UnityGameFramework.Runtime.GameEntry.GetComponent<GameTimerComponent>().SetGameTimerSystem(GameTime);

            Checkpoint = new GameCheckpointSystem();
            Unit = new UnitSystem();
            Input = new PlayerInputSystem();
            Sprite = new SpriteSystem();
            UnitProp = new UnitPropSystem();
            Backpack = new BackpackSystem();
            UnitSkill = new SkillSystem();
            Combat = new CombatSystem();
            SpecialEffect = new SpecialEffectSystem();

            InitCustomSystem();
            EnterCustomSystem();


            GameMain.GameStatus = GameStatus.Title;
        }

        /// <summary>
        /// 初始化自定义系统
        /// </summary>
        private static void InitCustomSystem()
        {
            Checkpoint.OnInit();
            Unit.OnInit();
            Input.OnInit();
            Sprite.OnInit();
            UnitProp.OnInit();
            Backpack.OnInit();
            UnitSkill.OnInit();
            Combat.OnInit();
            SpecialEffect.OnInit();
        }

        /// <summary>
        /// 进入自定义系统
        /// </summary>
        private static void EnterCustomSystem()
        {
            Checkpoint.OnEnter();
            Unit.OnEnter();
            Sprite.OnEnter();
            UnitProp.OnEnter();
            Backpack.OnEnter();
            UnitSkill.OnEnter();
            Combat.OnEnter();
            SpecialEffect.OnEnter();
        }

    }
}