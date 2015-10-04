using ColossalFramework.Plugins;
using ICities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using EightyOne.Areas;
using EightyOne.Attributes;
using EightyOne.ResourceManagers;
using EightyOne.Terrain;
using EightyOne.Zones;
using System.ComponentModel;

namespace EightyOne
{
    public class Mod : IUserMod
    {
        public string Description
        {
            get
            {
                return "Unlocks 81 tiles";
            }
        }

        public string Name
        {
            get
            {
                return "81 Tile Unlock - Trying to fix v1";
            }
        }
    }

    public class ModLoad : LoadingExtensionBase
    {
        public static Unlimiter unlimiter;
        public static GameObject gm;

        public override void OnLevelLoaded(LoadMode mode)
        {
            gm = new GameObject("unlimiter");
            unlimiter = gm.AddComponent<Unlimiter>();
            unlimiter.EnableHooks();
        }

        public override void OnLevelUnloading()
        {
            unlimiter.DisableHooks();
            GameObject.Destroy(gm);
        }

    }

    public class Unlimiter : MonoBehaviour
    {
        private static Dictionary<MethodInfo, RedirectCallsState> redirects;
        public static bool IsEnabled;
        private static BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        

        public void EnableHooks()
        {
            if (IsEnabled)
            {
                return;
            }
            IsEnabled = true;
            FakeWaterManager.Init();
            FakeDistrictManager.Init();
            FakeDistrictTool.Init();
            FakeImmaterialResourceManager.Init();
            FakeTerrainManager.Init();
            FakeZoneManager.Init();
            FakeNetManager.Init();
            FakeZoneTool.Init();
            FakeElectricityManager.Init();

            var toReplace = new Type[]
                {
                    typeof(GameAreaManager), typeof(FakeGameAreaManager),
                    typeof(GameAreaInfoPanel), typeof(FakeGameAreaInfoPanel),
                    //typeof(GameAreaTool), typeof(FakeGameAreaTool),

                    typeof(NetManager), typeof(FakeNetManager),
                    typeof(ZoneManager), typeof(FakeZoneManager),
                    typeof(BuildingTool ), typeof(FakeBuildingTool),
                    //typeof(Building ), typeof(FakeBuilding),

                    typeof(ZoneTool), typeof(FakeZoneTool),
                    //typeof(PrivateBuildingAI), typeof(FakePrivateBuildingAI),
                    typeof(TerrainManager), typeof(FakeTerrainManager),

                    typeof(ElectricityManager), typeof(FakeElectricityManager),
                    typeof(WaterManager), typeof(FakeWaterManager),
                    typeof(ImmaterialResourceManager), typeof(FakeImmaterialResourceManager),
                    typeof(DistrictManager), typeof(FakeDistrictManager),
                    typeof(DistrictTool), typeof(FakeDistrictTool),
                   typeof(NaturalResourceManager), typeof(FakeNatualResourceManager),
                };

            redirects = new Dictionary<MethodInfo, RedirectCallsState>();
            for (int i = 0; i < toReplace.Length; i += 2)
            {
                var from = toReplace[i];
                var to = toReplace[i + 1];

                foreach (var method in to.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (method.GetCustomAttributes(typeof(ReplaceMethodAttribute), false).Length == 1)
                    {
                        AddRedirect(from, method);
                    }
                }
            }
            FakeGameAreaManager.Init();
        }
        
        private void AddRedirect(Type type1, MethodInfo method)
        {
            var parameters = method.GetParameters();

            Type[] types;
            if (parameters.Length > 0 && parameters[0].ParameterType == type1)
                types = parameters.Skip(1).Select(p => p.ParameterType).ToArray();
            else
                types = parameters.Select(p => p.ParameterType).ToArray();

            var originalMethod = type1.GetMethod(method.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, types, null);
            if (originalMethod == null)
            {
                Debug.Log("Cannot find " + method.Name);
            }
            redirects.Add(originalMethod, RedirectionHelper.RedirectCalls(originalMethod, method));
        }

        public void DisableHooks()
        {
            if (!IsEnabled)
            {
                return;
            }
            IsEnabled = false;
            foreach (var kvp in redirects)
            {
                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
            }
            FakeImmaterialResourceManager.OnDestroy();
            FakeDistrictManager.OnDestroy();
            FakeWaterManager.OnDestroy();
            FakeElectricityManager.OnDestroy();
            FakeGameAreaManager.OnDestroy();
        }

        public void Update()
        {
            if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Input.GetKeyDown(KeyCode.U))
            {
                FakeGameAreaManager.UnlockAll();
            }
        }

        public static void CopyArray(IList newArray, object em, string propertyName)
        {
            var oldArray = (IList)em.GetType().GetField(propertyName, allFlags).GetValue(em);
            for (var i = 0; i < newArray.Count; i += 1)
            {
                newArray[i] = oldArray[i];
            }
        }

        public static void CopyArrayBack(IList newArray, object em, string propertyName)
        {
            var oldArray = (IList)em.GetType().GetField(propertyName, allFlags).GetValue(em);
            for (var i = 0; i < newArray.Count; i += 1)
            {
                 oldArray[i] = newArray[i];
            }
        }

        public static void CopyStructArray(IList newArray, object em, string propertyName)
        {
            var oldArray = (IList)em.GetType().GetField(propertyName, allFlags).GetValue(em);
            var fields = GetFieldsFromStruct(newArray[0], oldArray[0]);
            for (var i = 0; i < newArray.Count; i += 1)
            {
                newArray[i] = CopyStruct((object)newArray[0], oldArray[i], fields);     
            }
        }
        
        public static void CopyStructArrayBack(IList newArray, object em, string propertyName)
        {
            var oldArray = (IList)em.GetType().GetField(propertyName, allFlags).GetValue(em);
            var fields = GetFieldsFromStruct( oldArray[0],newArray[0]);
            for (var i = 0; i < newArray.Count; i += 1)
            {
                oldArray[i] = CopyStruct((object)oldArray[i], newArray[i], fields);
            }
        }


        public static Dictionary<FieldInfo, FieldInfo> GetFieldsFromStruct(object newArray, object oldArray)
        {
            var fields = new Dictionary<FieldInfo, FieldInfo>();
            foreach (var f in oldArray.GetType().GetFields(allFlags))
            {
                fields.Add(newArray.GetType().GetField(f.Name, allFlags), f);
            }
            return fields;
        }

        public static void SetPropertyValue<T>(ref T result, object obj, string propName)
        {
            result = (T)obj.GetType().GetField(propName, allFlags).GetValue(obj);
        }

        public static void SetPropertyValueBack(object result, object obj, string propName)
        {
            obj.GetType().GetField(propName, allFlags).SetValue(obj,result);
        }

        public static object CopyStruct(object newObj, object original, Dictionary<FieldInfo, FieldInfo> fields)
        {
            foreach (var field in fields)
            {
                if (field.Key.FieldType != field.Value.FieldType){
                    if (field.Key.FieldType == typeof(byte))
                    {
                        var oo = Mathf.Clamp((ushort)field.Value.GetValue(original),0,255);
                        field.Key.SetValue(newObj,(byte)oo);
                        continue;
                    }
                }
                field.Key.SetValue(newObj, field.Value.GetValue(original));                
            }
            return newObj;
        }
    }
}
