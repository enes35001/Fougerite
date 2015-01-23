﻿namespace Fougerite
{
    using Facepunch;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Timers;
    using uLink;
    using UnityEngine;

    public class World
    {
        private static World world;
        public Dictionary<string, Zone3D> zones;

        public World()
        {
            this.zones = new Dictionary<string, Zone3D>();
        }

        public static World GetWorld()
        {
            if (world == null)
            {
                world = new World();
            }
            return world;
        }

        public void Airdrop()
        {
            this.Airdrop(1);
        }

        public void Airdrop(int rep)
        {
            System.Random rand = new System.Random();
            Vector3 rpog;
            for (int i = 0; i < rep; i++)
            {
                RandomPointOnGround(ref rand, out rpog);
                SupplyDropZone.CallAirDropAt(rpog);
            }
        }

        private static void RandomPointOnGround(ref System.Random rand, out Vector3 onground)
        {
            float z = (float)rand.Next(-6100, -1000);
            float x = (float)3600;
            if (z < -4900 && z >= -6100)
            {
                x = (float)rand.Next(3600, 6100);
            }
            if (z < 2400 && z >= -4900)
            {
                x = (float)rand.Next(3600, 7300);
            }
            if (z <= -1000 && z >= -2400)
            {
                x = (float)rand.Next(3600, 6700);
            }
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 500, z));
            onground = new Vector3(x, y, z);
        }

        public void AirdropAt(float x, float y, float z)
        {
            this.AirdropAt(x, y, z, 1);
        }

        public void AirdropAt(float x, float y, float z, int rep)
        {
            Vector3 target = new Vector3(x, y, z);
            this.AirdropAt(target, rep);
        }

        public void AirdropAtPlayer(Fougerite.Player p)
        {
            this.AirdropAt(p.X, p.Y, p.Z, 1);
        }

        public void AirdropAtPlayer(Fougerite.Player p, int rep)
        {
            this.AirdropAt(p.X, p.Y, p.Z, rep);
        }

        public void AirdropAt(Vector3 target, int rep)
        {
            Vector3 original = target;
            System.Random rand = new System.Random();
            int r, reset;
            r = reset = 20;
            for (int i = 0; i < rep; i++)
            {
                r--;
                if (r == 0)
                {
                    r = reset;
                    target = original;
                }
                target.y = original.y + rand.Next(-5, 20) * 20;
                SupplyDropZone.CallAirDropAt(target);
                Jitter(ref target);
            }
        }

        private static void Jitter(ref Vector3 target)
        {
            Vector2 jitter = UnityEngine.Random.insideUnitCircle;
            target.x += jitter.x * 100;
            target.z += jitter.y * 100;
        }

        public void Blocks()
        {
            foreach (ItemDataBlock block in DatablockDictionary.All)
            {
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Name: " + block.name + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "ID: " + block.uniqueID + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Flags: " + block._itemFlags.ToString() + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Max Condition: " + block._maxCondition + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Loose Condition: " + block.doesLoseCondition + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Max Uses: " + block._maxUses + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Mins Uses (Display): " + block._minUsesForDisplay + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Spawn Uses Max: " + block._spawnUsesMax + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Spawn Uses Min: " + block._spawnUsesMin + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Splittable: " + block._splittable + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Category: " + block.category.ToString() + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Combinations:\n");
                foreach (ItemDataBlock.CombineRecipe recipe in block.Combinations)
                {
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "\t" + recipe.ToString() + "\n");
                }
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Icon: " + block.icon + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "IsRecycleable: " + block.isRecycleable + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "IsRepairable: " + block.isRepairable + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "IsResearchable: " + block.isResearchable + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Description: " + block.itemDescriptionOverride + "\n");
                if (block is BulletWeaponDataBlock)
                {
                    BulletWeaponDataBlock block2 = (BulletWeaponDataBlock)block;
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Min Damage: " + block2.damageMin + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Max Damage: " + block2.damageMax + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Ammo: " + block2.ammoType.ToString() + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Recoil Duration: " + block2.recoilDuration + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "RecoilPitch Min: " + block2.recoilPitchMin + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "RecoilPitch Max: " + block2.recoilPitchMax + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "RecoilYawn Min: " + block2.recoilYawMin + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "RecoilYawn Max: " + block2.recoilYawMax + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Bullet Range: " + block2.bulletRange + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Sway: " + block2.aimSway + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "SwaySpeed: " + block2.aimSwaySpeed + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Aim Sensitivity: " + block2.aimSensitivtyPercent + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "FireRate: " + block2.fireRate + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "FireRate Secondary: " + block2.fireRateSecondary + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Max Clip Ammo: " + block2.maxClipAmmo + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Reload Duration: " + block2.reloadDuration + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "Attachment Point: " + block2.attachmentPoint + "\n");
                }
                File.AppendAllText(Util.GetAbsoluteFilePath("BlocksData.txt"), "------------------------------------------------------------\n\n");
            }
        }

        public StructureMaster CreateSM(Fougerite.Player p)
        {
            return this.CreateSM(p, p.X, p.Y, p.Z, p.PlayerClient.transform.rotation);
        }

        public StructureMaster CreateSM(Fougerite.Player p, float x, float y, float z)
        {
            return this.CreateSM(p, x, y, z, Quaternion.identity);
        }

        public StructureMaster CreateSM(Fougerite.Player p, float x, float y, float z, Quaternion rot)
        {
            StructureMaster master = NetCull.InstantiateClassic<StructureMaster>(Bundling.Load<StructureMaster>("content/structures/StructureMasterPrefab"), new Vector3(x, y, z), rot, 0);
            master.SetupCreator(p.PlayerClient.controllable);
            return master;
        }

        public Zone3D CreateZone(string name)
        {
            return new Zone3D(name);
        }

        public float GetGround(float x, float z)
        {
            Vector3 above = new Vector3(x, 2000f, z);
            return (float)((RaycastHit)Physics.RaycastAll(above, Vector3.down, 2000f)[0]).point.y;
        }

        public float GetGround(Vector3 target)
        {
            Vector3 above = new Vector3(target.x, 2000f, target.z);
            return (float)((RaycastHit)Physics.RaycastAll(above, Vector3.down, 2000f)[0]).point.y;
        }

        public float GetTerrainHeight(Vector3 target)
        {
            return Terrain.activeTerrain.SampleHeight(target);
        }

        public float GetTerrainHeight(float x, float y, float z)
        {
            return GetTerrainHeight(new Vector3(x, y, z));
        }

        public float GetTerrainSteepness(Vector3 target)
        {
            return Terrain.activeTerrain.terrainData.GetSteepness(target.x, target.z);
        }

        public float GetTerrainSteepness(float x, float z)
        {
            return Terrain.activeTerrain.terrainData.GetSteepness(x, z);
        }

        public float GetGroundDist(float x, float y, float z)
        {
            float ground = GetGround(x, z);
            return y - ground;
        }

        public float GetGroundDist(Vector3 target)
        {
            float ground = GetGround(target);
            return target.y - ground;
        }

        public void Lists()
        {
            foreach (LootSpawnList list in DatablockDictionary._lootSpawnLists.Values)
            {
                File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "Name: " + list.name + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "Min Spawn: " + list.minPackagesToSpawn + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "Max Spawn: " + list.maxPackagesToSpawn + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "NoDuplicate: " + list.noDuplicates + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "OneOfEach: " + list.spawnOneOfEach + "\n");
                File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "Entries:\n");
                foreach (LootSpawnList.LootWeightedEntry entry in list.LootPackages)
                {
                    File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "Amount Min: " + entry.amountMin + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "Amount Max: " + entry.amountMax + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "Weight: " + entry.weight + "\n");
                    File.AppendAllText(Util.GetAbsoluteFilePath("Lists.txt"), "Object: " + entry.obj.ToString() + "\n\n");
                }
            }
        }

        public void Prefabs()
        {
            foreach (ItemDataBlock block in DatablockDictionary.All)
            {
                if (block is DeployableItemDataBlock)
                {
                    DeployableItemDataBlock block2 = block as DeployableItemDataBlock;
                    File.AppendAllText(Util.GetAbsoluteFilePath("Prefabs.txt"), "[\"" + block2.ObjectToPlace.name + "\", \"" + block2.DeployableObjectPrefabName + "\"],\n");
                } else if (block is StructureComponentDataBlock)
                {
                    StructureComponentDataBlock block3 = block as StructureComponentDataBlock;
                    File.AppendAllText(Util.GetAbsoluteFilePath("Prefabs.txt"), "[\"" + block3.structureToPlacePrefab.name + "\", \"" + block3.structureToPlaceName + "\"],\n");
                }
            }
        }


        public void DataBlocks()
        {
            foreach (ItemDataBlock block in DatablockDictionary.All)
            {
                File.AppendAllText(Util.GetAbsoluteFilePath("DataBlocks.txt"), string.Format("name={0} uniqueID={1}\n", block.name, block.uniqueID));
            }
        }

        public object Spawn(string prefab, Vector3 location)
        {
            return this.Spawn(prefab, location, 1);
        }

        public object Spawn(string prefab, Vector3 location, int rep)
        {
            return this.Spawn(prefab, location, Quaternion.identity, rep);
        }

        public object Spawn(string prefab, float x, float y, float z)
        {
            return this.Spawn(prefab, x, y, z, 1);
        }

        private object Spawn(string prefab, Vector3 location, Quaternion rotation, int rep)
        {
            object obj2 = null;
            for (int i = 0; i < rep; i++)
            {
                if (prefab == ":player_soldier")
                {
                    obj2 = NetCull.InstantiateDynamic(uLink.NetworkPlayer.server, prefab, location, rotation);
                } else if (prefab.Contains("C130"))
                {
                    obj2 = NetCull.InstantiateClassic(prefab, location, rotation, 0);
                } else
                {
                    GameObject obj3 = NetCull.InstantiateStatic(prefab, location, rotation);
                    obj2 = obj3;
                    StructureComponent component = obj3.GetComponent<StructureComponent>();
                    if (component != null)
                    {
                        obj2 = new Entity(component);
                    } else
                    {
                        DeployableObject obj4 = obj3.GetComponent<DeployableObject>();
                        if (obj4 != null)
                        {
                            obj4.ownerID = 0L;
                            obj4.creatorID = 0L;
                            obj4.CacheCreator();
                            obj4.CreatorSet();
                            obj2 = new Entity(obj4);
                        }
                    }
                }
            }
            return obj2;
        }

        public object Spawn(string prefab, float x, float y, float z, int rep)
        {
            return this.Spawn(prefab, new Vector3(x, y, z), Quaternion.identity, rep);
        }

        public object Spawn(string prefab, float x, float y, float z, Quaternion rot)
        {
            return this.Spawn(prefab, x, y, z, rot, 1);
        }

        public object Spawn(string prefab, float x, float y, float z, Quaternion rot, int rep)
        {
            return this.Spawn(prefab, new Vector3(x, y, z), rot, rep);
        }

        public object SpawnAtPlayer(string prefab, Fougerite.Player p)
        {
            return this.Spawn(prefab, p.Location, p.PlayerClient.transform.rotation, 1);
        }

        public object SpawnAtPlayer(string prefab, Fougerite.Player p, int rep)
        {
            return this.Spawn(prefab, p.Location, p.PlayerClient.transform.rotation, rep);
        }

        public float DayLength
        {
            get { return env.daylength; }
            set { env.daylength = value; }
        }

        public List<Entity> AllStructures
        {
            get
            {
                IEnumerable<Entity> structures = from s in StructureMaster.AllStructures
                                                             select new Entity(s);
                return structures.ToList<Entity>();
            }
        }

        public List<Entity> Entities
        {
            get
            {
                IEnumerable<Entity> component = from c in (UnityEngine.Object.FindObjectsOfType<StructureComponent>() as StructureComponent[])
                                                            select new Entity(c);
                IEnumerable<Entity> deployable = from d in (UnityEngine.Object.FindObjectsOfType<DeployableObject>() as DeployableObject[])
                                                            select new Entity(d);
                // this is much faster than Concat
                List<Entity> entities = new List<Entity>(component.Count() + deployable.Count());
                entities.AddRange(component);
                entities.AddRange(deployable);
                return entities;
            }
        }

        public float NightLength
        {
            get { return env.nightlength; }
            set { env.nightlength = value; }
        }

        public float Time
        {
            get
            {
                try
                {
                    float hour = EnvironmentControlCenter.Singleton.GetTime();
                    return hour;
                } catch (NullReferenceException)
                {
                    return 12f;
                }
            }
            set
            {
                float hour = value;
                if (hour < 0f || hour > 24f)
                    hour = 12f;

                try
                {
                    EnvironmentControlCenter.Singleton.SetTime(hour);
                } catch (Exception)
                {
                }
            }
        }

        public static bool IsBP(string search, out string match)
        {
            string BP = "BP";
            string BLUEPRINT = "BLUEPRINT";
            bool flag = false;          
            string[] terms = search.Split(new char[] { ' ' });
            foreach (string term in terms)
            {
                match = term;
                flag = BP.Distance(term) == 0 ? true : false;
                if (flag)
                    return flag;

                int distance = Math.Abs(term.Length - BLUEPRINT.Length) + 1;
                flag = BLUEPRINT.Distance(term) <= distance ? true : false;
                if (flag)
                    return flag;
            }
            match = string.Empty;
            return flag;
        }

        public string MatchItemName(string search)
        {
            IEnumerable<string> query = from term in itemNames
                                                 group term by search.Distance(term) into match
                                                 orderby match.Key ascending
                                                 select match.FirstOrDefault();
            if (query.Count() == 1)
                return query.FirstOrDefault();

            Logger.LogDebug("[MatchItemName] found more than one match, returning first.");
            Logger.LogDebug(string.Format("[MatchItemName] search={0} matches={1}", search, string.Join(", ", query.ToArray())));
            return query.FirstOrDefault();
        }

        public readonly string[] itemNames = { "556 Ammo Blueprint", "556 Ammo", "556 Casing Blueprint", "9mm Ammo Blueprint", "9mm Ammo", "9mm Casing Blueprint", "9mm Pistol Blueprint", "9mm Pistol", 
            "Animal Fat", "Anti-Radiation Pills", "Armor Part 1 BP", "Armor Part 1", "Armor Part 2 BP", "Armor Part 2", "Armor Part 3 BP", "Armor Part 3", "Armor Part 4 BP", 
            "Armor Part 4", "Armor Part 5 BP", "Armor Part 5", "Armor Part 6 BP", "Armor Part 6", "Armor Part 7 BP", "Armor Part 7", "Arrow Blueprint", "Arrow", "Bandage Blueprint", 
            "Bandage", "Bed Blueprint", "Bed", "Blood Draw Kit Blueprint", "Blood Draw Kit", "Blood", "Bolt Action Rifle Blueprint", "Bolt Action Rifle", "Camp Fire Blueprint", "Camp Fire", "Can of Beans", 
            "Can of Tuna", "Charcoal", "Chocolate Bar", "Cloth Boots BP", "Cloth Boots", "Cloth Helmet BP", "Cloth Helmet", "Cloth Pants BP", "Cloth Pants", "Cloth Vest BP", "Cloth Vest", 
            "Cloth", "Cooked Chicken Breast", "Empty 556 Casing", "Empty 9mm Casing", "Empty Shotgun Shell Blueprint", "Empty Shotgun Shell", "Explosive Charge Blueprint", "Explosive Charge", 
            "Explosives Blueprint", "Explosives", "F1 Grenade Blueprint", "F1 Grenade", "Flare Blueprint", "Flare", "Flashlight Mod BP", "Flashlight Mod", "Furnace Blueprint", "Furnace", 
            "Granola Bar", "Gunpowder Blueprint", "Gunpowder", "HandCannon Blueprint", "HandCannon", "Handmade Lockpick Blueprint", "Handmade Lockpick", "Handmade Shell Blueprint", "Handmade Shell", 
            "Hatchet Blueprint", "Hatchet", "Holo sight BP", "Holo sight", "Hunting Bow Blueprint", "Hunting Bow", "Invisible Boots", "Invisible Helmet", "Invisible Pants", "Invisible Vest", 
            "Kevlar Boots BP", "Kevlar Boots", "Kevlar Helmet BP", "Kevlar Helmet", "Kevlar Pants BP", "Kevlar Pants", "Kevlar Vest BP", "Kevlar Vest", "Large Medkit Blueprint", "Large Medkit", 
            "Large Spike Wall Blueprint", "Large Spike Wall", "Large Wood Storage Blueprint", "Large Wood Storage", "Laser Sight BP", "Laser Sight", "Leather Boots BP", "Leather Boots", "Leather Helmet BP", 
            "Leather Helmet", "Leather Pants BP", "Leather Pants", "Leather Vest BP", "Leather Vest", "Leather", "Low Grade Fuel Blueprint", "Low Grade Fuel", "Low Quality Metal Blueprint", "Low Quality Metal", 
            "M4 Blueprint", "M4", "MP5A4 Blueprint", "MP5A4", "Metal Ceiling BP", "Metal Ceiling", "Metal Door Blueprint", "Metal Door", "Metal Doorway BP", "Metal Doorway", "Metal Foundation BP",
            "Metal Foundation", "Metal Fragments", "Metal Ore", "Metal Pillar BP", "Metal Pillar", "Metal Ramp BP", "Metal Ramp", "Metal Stairs BP", "Metal Stairs", "Metal Wall BP", "Metal Wall", "Metal Window BP", 
            "Metal Window Bars Blueprint", "Metal Window Bars", "Metal Window", "P250 Blueprint", "P250", "Paper Blueprint", "Paper", "Pick Axe Blueprint", "Pick Axe", "Pipe Shotgun Blueprint", "Pipe Shotgun", 
            "Primed 556 Casing Blueprint", "Primed 556 Casing", "Primed 9mm Casing Blueprint", "Primed 9mm Casing", "Primed Shotgun Shell Blueprint", "Primed Shotgun Shell", "Rad Suit Boots BP", "Rad Suit Boots", 
            "Rad Suit Helmet BP", "Rad Suit Helmet", "Rad Suit Pants BP", "Rad Suit Pants", "Rad Suit Vest BP", "Rad Suit Vest", "Raw Chicken Breast", "Recycle Kit 1", "Repair Bench Blueprint", "Repair Bench", 
            "Research Kit 1", "Research Kit Blueprint", "Revolver Blueprint", "Revolver", "Rock", "Shotgun Blueprint", "Shotgun Shells Blueprint", "Shotgun Shells", "Shotgun", "Silencer BP", "Silencer", 
            "Sleeping Bag Blueprint", "Sleeping Bag", "Small Medkit Blueprint", "Small Medkit", "Small Rations", "Small Stash Blueprint", "Small Stash", "Small Water Bottle", "Spike Wall Blueprint", "Spike Wall", 
            "Stone Hatchet Blueprint", "Stone Hatchet", "Stones", "Sulfur Ore", "Sulfur", "Supply Signal", "Torch Blueprint", "Torch", "Uber Hatchet", "Uber Hunting Bow", "Weapon Part 1 BP", "Weapon Part 1", 
            "Weapon Part 2 BP", "Weapon Part 2", "Weapon Part 3 BP", "Weapon Part 3", "Weapon Part 4 BP", "Weapon Part 4", "Weapon Part 5 BP", "Weapon Part 5", "Weapon Part 6 BP", "Weapon Part 6", "Weapon Part 7 BP", 
            "Weapon Part 7", "Wood Barricade Blueprint", "Wood Barricade", "Wood Ceiling BP", "Wood Ceiling", "Wood Doorway BP", "Wood Doorway", "Wood Foundation BP", "Wood Foundation", "Wood Gate Blueprint", 
            "Wood Gate", "Wood Gateway Blueprint", "Wood Gateway", "Wood Pillar BP", "Wood Pillar", "Wood Planks Blueprint", "Wood Planks", "Wood Ramp BP", "Wood Ramp", "Wood Shelter Blueprint", "Wood Shelter", 
            "Wood Stairs BP", "Wood Stairs", "Wood Storage Box Blueprint", "Wood Storage Box", "Wood Wall BP", "Wood Wall", "Wood Window BP", "Wood Window", "Wood", "Wooden Door Blueprint", "Wooden Door", 
            "Workbench Blueprint", "Workbench"
        };
    }

}