using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PeterHan.PLib.Options;
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
                // 新配方：沙子合成冰霜汉堡(烤炉制作)
                ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement("Sand".ToTag(), 0.01f)
                };
                ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement("Burger".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
                };
                SpiceBreadConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array3, array4), array3, array4)
                {
                    time = 8f,
                    description = ITEMS.FOOD.SPICEBREAD.RECIPEDESC,
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                    fabricators = new List<Tag>
                    {
                        "CookingStation"
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
        // ================================【存储箱】====================================
        [HarmonyPatch(typeof(StorageLockerConfig),"DoPostConfigureComplete")]
        public class Patches_e
        {
            // 容量从两万到十万，这个修改原理还没搞懂，因为capacityKg是类中的常量
            public static void Postfix(ref GameObject go)
            {
                go.AddOrGet<Storage>().capacityKg = 100000f;
            }
        }
        // ================================【电线的负载功率】====================================
        [HarmonyPatch(typeof(Wire), "GetMaxWattageAsFloat")]
        public class Patches_f
        {
            // 将所有的电线负载改成5万
            public static bool Prefix(ref float __result, Wire.WattageRating rating)
            {
                __result = 50000f;
                return false;
            }
        }
        // ================================【储液库的容量100吨】====================================
        [HarmonyPatch(typeof(LiquidReservoirConfig),"ConfigureBuildingTemplate")]
        public class Patches_g
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
            {
                List<CodeInstruction> list = codeInstructions.ToList<CodeInstruction>();
                list[17].operand = 100000f;
                return list.AsEnumerable<CodeInstruction>();
            }
        }
        // ================================【储气库的容量100吨】====================================
        [HarmonyPatch(typeof(GasReservoirConfig),"ConfigureBuildingTemplate")]
        public class Patches_h
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
            {
                List<CodeInstruction> list = codeInstructions.ToList<CodeInstruction>();
                list[14].operand = 100000f;
                return list.AsEnumerable<CodeInstruction>();
            }
        }
        // ================================【修改碎石机配方】====================================
        [HarmonyPatch(typeof(RockCrusherConfig), "ConfigureBuildingTemplate")]
        public class Patches_i
        {
            // 配方：沙子生成金子和钢、塑料
            public static void Postfix()
            {
                Element element = ElementLoader.FindElementByHash(SimHashes.Gold);
                ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Sand).tag, 1f)
                };
                ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Steel).tag, 600f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Gold).tag, 600f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag, 600f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                };
                ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array, array2), array, array2);
                complexRecipe.time = 8f;
                complexRecipe.description = string.Format(BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Gold.CreateTag().ProperName(), ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME);
                complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
                complexRecipe.fabricators = new List<Tag>
                {
                    TagManager.Create("RockCrusher")
                };
            }
        }
    }
}
