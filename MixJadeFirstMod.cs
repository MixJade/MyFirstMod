﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using STRINGS;
using UnityEngine;
using static STRINGS.BUILDINGS.PREFABS;

namespace MixJadeFirstMod
{
    public class Patches // 固定代码
    {
        // ================================【人力发电机】====================================
        [HarmonyPatch(typeof(ManualGeneratorConfig), "CreateBuildingDef")] // 定位代码
        public class Patches_a  // 自定义名称
        {
            public static void Postfix(ref BuildingDef __result) // 函数名称固定，参数类型是定位函数的返回值
            {
                __result.GeneratorWattageRating = 40000f; // 发电功率
            }
        }
        // ================================【普通电池】====================================
        [HarmonyPatch(typeof(BatteryConfig), "DoPostConfigureComplete")]
        public class Patches_b
        {
            public static void Postfix(GameObject go) // 参数要与定位的函数一致
            {
                Battery battery = go.AddOrGet<Battery>();
                battery.capacity = 1000000f;
                battery.joulesLostPerSecond = 0f;
            }
        }
        // ================================【食物压制器】====================================
        [HarmonyPatch(typeof(MicrobeMusherConfig), "ConfigureRecipes")]
        public class Patches_c
        {
            public static void Postfix()
            {
                // 新配方：泥土和砂岩合成火椒面包
                ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement("Dirt".ToTag(), 0.1f),
                    new ComplexRecipe.RecipeElement("Sandstone".ToTag(), 0.1f)
                };
                ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement("SpiceBread".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
                };
                SpiceBreadConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MicrobeMusher", array, array2), array, array2)
                {
                    time = 8f,
                    description = ITEMS.FOOD.SPICEBREAD.RECIPEDESC,
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                    fabricators = new List<Tag>
                    {
                        "MicrobeMusher"
                    },
                    sortOrder = 600
                };
            }
        }
        // ================================【氧气扩散器】====================================
        [HarmonyPatch(typeof(MineralDeoxidizerConfig), "ConfigureBuildingTemplate")]
        public class Patches_d
        {
            public static void Postfix(GameObject go, Tag prefab_tag) // 参数要与定位的函数一致
            {
                ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
                elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
                {
            new ElementConverter.ConsumedElement(new Tag("Algae"), 0.01f, true)
                };
                elementConverter.outputElements = new ElementConverter.OutputElement[]
                {
            new ElementConverter.OutputElement(5f, SimHashes.Oxygen, 303.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0, true)
                };
            }
        }
    }
}
