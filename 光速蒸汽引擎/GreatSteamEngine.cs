using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace GreatSteamEngine
{
    public class Patches
    {
        // ================================【DLC的蒸汽引擎】====================================
        [HarmonyPatch(typeof(SteamEngineClusterConfig), "DoPostConfigureComplete")]
        public class Patches_b
        {
            public static void Postfix(GameObject go) // 参数要与定位的函数一致
            {
                RocketEngineCluster rocketEngineCluster = go.AddOrGet<RocketEngineCluster>();
                rocketEngineCluster.maxModules = 6;
                rocketEngineCluster.maxHeight = 35;
                rocketEngineCluster.fuelTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
                rocketEngineCluster.efficiency = 20000f;
                rocketEngineCluster.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
                rocketEngineCluster.requireOxidizer = false;
                rocketEngineCluster.exhaustElement = SimHashes.Oxygen;
                rocketEngineCluster.exhaustTemperature = 289.15f;
                go.AddOrGet<ModuleGenerator>();
                Storage storage = go.AddOrGet<Storage>();
                storage.capacityKg = 150f;
                storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
                {
                    Storage.StoredItemModifier.Hide,
                    Storage.StoredItemModifier.Seal,
                    Storage.StoredItemModifier.Insulate
                });
                FuelTank fuelTank = go.AddOrGet<FuelTank>();
                fuelTank.consumeFuelOnLand = false;
                fuelTank.storage = storage;
                fuelTank.FuelType = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
                fuelTank.targetFillMass = storage.capacityKg;
                fuelTank.physicalFuelCapacity = storage.capacityKg;
                go.AddOrGet<CopyBuildingSettings>();
                ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                conduitConsumer.conduitType = ConduitType.Gas;
                conduitConsumer.consumptionRate = 10f;
                conduitConsumer.capacityTag = fuelTank.FuelType;
                conduitConsumer.capacityKG = storage.capacityKg;
                conduitConsumer.forceAlwaysSatisfied = true;
                conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
                BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, 15, (float)270, 0.001f);
                go.GetComponent<KPrefabID>().prefabInitFn += delegate (GameObject inst)
                {
                };
            }
        }
    }
}
