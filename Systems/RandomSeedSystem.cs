// <copyright file="TreeObjectDefinitionSystem.cs" company="Yenyangs Mods. MIT License">
// Copyright (c) Yenyangs Mods. MIT License. All rights reserved.
// </copyright>

using Colossal.Entities;
using Colossal.Logging;
using Game;
using Game.Common;
using Game.Input;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.InputSystem;

namespace AssetVariationChanger.Systems
{


    /// <summary>
    /// Overrides tree state on placement with object tool based on setting.
    /// </summary>
    public partial class RandomSeedSystem : GameSystemBase
    {
        public static RandomSeedSystem Instance;
        private EntityQuery m_ObjectDefinitionQuery;
        private ILog m_Log;
        private Unity.Mathematics.Random m_Random;
        private int m_RandomSeed;
        private ToolSystem m_ToolSystem;


        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSeedSystem"/> class.
        /// </summary>
        public RandomSeedSystem()
        {
            Instance = this;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_Log = Mod.log;
            m_Log.Info($"[{nameof(RandomSeedSystem)}] {nameof(OnCreate)}");
            m_Random = new Unity.Mathematics.Random(1);
            m_ToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();

            //m_RandomSeed = m_Random.NextInt();
            m_RandomSeed = 0;
        }


        public void OnPreviousAssetVariation(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Performed)
            {
                m_RandomSeed--;
                m_Log.Info($"Previous Variation: {m_RandomSeed}");
            }
        }

        public void OnNextAssetVariation(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Performed)
            {
                m_RandomSeed++;
                m_Log.Info($"Next Variation: {m_RandomSeed}");
            }
        }


        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!Mod.m_Setting.EnableVariationChooser)
            {
                return;
            }
            if (m_ToolSystem.activeTool.toolID == "Line Tool")
            {
                return;
            }
            m_ObjectDefinitionQuery = SystemAPI.QueryBuilder()
                .WithAllRW<CreationDefinition>()
                .WithAll<Updated>()
                .WithNone<Deleted, Overridden>()
                .Build();

            RequireForUpdate(m_ObjectDefinitionQuery);
            NativeArray<Entity> entities = m_ObjectDefinitionQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                if (!EntityManager.TryGetComponent(entity, out CreationDefinition currentCreationDefinition))
                {
                    entities.Dispose();
                    return;
                }

                currentCreationDefinition.m_RandomSeed = m_RandomSeed;
                EntityManager.SetComponentData(entity, currentCreationDefinition);
            }

            entities.Dispose();
        }
    }
}
