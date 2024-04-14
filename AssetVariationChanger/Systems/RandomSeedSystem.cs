// <copyright file="TreeObjectDefinitionSystem.cs" company="Yenyangs Mods. MIT License">
// Copyright (c) Yenyangs Mods. MIT License. All rights reserved.
// </copyright>

using Colossal.Entities;
using Colossal.Logging;
using Game;
using Game.Common;
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
        private EntityQuery m_ObjectDefinitionQuery;
        private ILog m_Log;
        private Unity.Mathematics.Random m_Random;
        private int m_RandomSeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSeedSystem"/> class.
        /// </summary>
        public RandomSeedSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_Log = Mod.log;
            m_Log.Info($"[{nameof(RandomSeedSystem)}] {nameof(OnCreate)}");
            m_Random = new Unity.Mathematics.Random(1);
        }


        /// <inheritdoc/>
        protected override void OnUpdate()
        {
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

                if (Keyboard.current.insertKey.isPressed) 
                {
                    m_RandomSeed = m_Random.NextInt();
                    m_Log.Info($"InsertKeyPressed {m_RandomSeed}");
                }
                currentCreationDefinition.m_RandomSeed = m_RandomSeed;
                EntityManager.SetComponentData(entity, currentCreationDefinition);
            }

            entities.Dispose();
        }

    }
}
