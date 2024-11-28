// <copyright file="TreeObjectDefinitionSystem.cs" company="Yenyangs Mods. MIT License">
// Copyright (c) Yenyangs Mods. MIT License. All rights reserved.
// </copyright>

using System;
using System.Numerics;
using System.Reflection;
using Colossal.Entities;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game;
using Game.Common;
using Game.Input;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;

namespace AssetVariationChanger.Systems
{


    /// <summary>
    /// Overrides tree state on placement with object tool based on setting.
    /// </summary>
    public partial class VariationChangerSystem : GameSystemBase
    {
        public static VariationChangerSystem Instance;
        private EntityQuery m_ObjectDefinitionQuery;
        private ObjectToolSystem m_ObjectToolSystem;
        private ILog m_Log;
        private Unity.Mathematics.Random m_Random;
        private int m_RandomSeed;
        private ToolSystem m_ToolSystem;
        private bool m_FoundTreeController;
        private ComponentType m_VegetationComponentType;


        /// <summary>
        /// Initializes a new instance of the <see cref="VariationChangerSystem"/> class.
        /// </summary>
        public VariationChangerSystem()
        {
            Instance = this;
        }

        /// <inheritdoc/>
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Loop through all assemblies and look for type matching name space and type for Tree_Controller.Vegetation.
            // This type is added by Tree Controller to all prefabs entities in the Vegetation tab in the Landscaping UI menu.
            foreach (Assembly assembly in assemblies)
            {
                Type type = assembly.GetType("Tree_Controller.Vegetation");
                if (type != null)
                {
                    m_Log.Info($"Found {type.FullName} in {type.Assembly.FullName}. ");
                    m_VegetationComponentType = ComponentType.ReadOnly(type);
                    m_FoundTreeController = true;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_Log = Mod.log;
            //m_Log.Info($"[{nameof(RandomSeedSystem)}] {nameof(OnCreate)}");
            m_Random = new Unity.Mathematics.Random(1);
            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_ObjectToolSystem = World.GetOrCreateSystemManaged<ObjectToolSystem>();

            m_ObjectDefinitionQuery = SystemAPI.QueryBuilder()
                .WithAllRW<CreationDefinition>()
                .WithAll<Updated>()
                .WithNone<Deleted, Overridden>()
                .Build();

            //m_RandomSeed = m_Random.NextInt();
            m_RandomSeed = 0;
            RequireForUpdate(m_ObjectDefinitionQuery);
        }

        private void ForceUpdate()
        {
            var field = typeof(ObjectToolSystem).GetField("m_ForceUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(m_ObjectToolSystem, true);
            }
            else
            {
                m_Log.Error($"Field m_ForceUpdate not found in ObjectToolSystem.");
            }
        }


        public void OnPreviousAssetVariation(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Performed)
            {
                m_RandomSeed--;
                ForceUpdate();
                m_Log.Info($"Previous Variation: {m_RandomSeed}");
            }
        }

        public void OnNextAssetVariation(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Performed)
            {
                m_RandomSeed++;
                ForceUpdate();
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
            if (Mod.m_Setting.LineToolCompatibility && m_ToolSystem.activeTool.toolID == "Line Tool")
            {
                return;
            }

            NativeArray<Entity> entities = m_ObjectDefinitionQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                if (!EntityManager.TryGetComponent(entity, out CreationDefinition currentCreationDefinition))
                {
                    entities.Dispose();
                    return;
                }

                // If found tree controller type and the prefab entity has vegetation component then skip this entity.
                if (Mod.m_Setting.TreeControllerCompatibility && m_FoundTreeController && EntityManager.HasComponent(currentCreationDefinition.m_Prefab, m_VegetationComponentType))
                {
                    continue;
                }

                if (!EntityManager.TryGetComponent(entity, out ObjectDefinition currentObjectDefinition))
                {
                    entities.Dispose();
                    return;
                }

                currentCreationDefinition.m_RandomSeed = m_RandomSeed;
                EntityManager.SetComponentData(entity, currentCreationDefinition);

                /*currentObjectDefinition.m_Rotation = Quaternion.Euler(0, m_Random.NextFloat(0, 360), 0);
                EntityManager.SetComponentData(entity, currentObjectDefinition);*/
            }

            entities.Dispose();
        }
    }
}
