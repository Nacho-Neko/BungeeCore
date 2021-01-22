using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BungeeCore.Common.Helper
{
    public class EntityMapper
    {
        private delegate object mapEntity(Block block);
        private static Dictionary<Type, Delegate> cachedMappers = new Dictionary<Type, Delegate>();
        public static object MapToEntities(Type type, Block block)
        {
            if (!cachedMappers.ContainsKey(type))
            {
                Type[] methodArgs = { typeof(Block) };
                DynamicMethod dm = new DynamicMethod("MapDR", returnType: type, parameterTypes: methodArgs, Assembly.GetExecutingAssembly().GetType().Module);
                ILGenerator il = dm.GetILGenerator();
                il.DeclareLocal(type);
                il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldarg_0);
                    switch (propertyInfo.PropertyType.Name)
                    {
                        case "VarInt":
                            // obj = block.readVarInt();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readVarInt"));
                            break;
                        case "readVarLong":
                            // obj = block.readVarLong();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readVarLong"));
                            break;
                        case "String":
                            // obj = block.readString();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readString", new Type[] { }));
                            break;
                        case "Boolean":
                            // block.readBoolean();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readBoolean"));
                            break;
                        case "SByte":
                            // block.readSByte();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readSByte"));
                            break;
                        case "Byte":
                            // block.readByte();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readByte"));
                            break;
                        case "Int16":
                            // block.readShort();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readShort"));
                            break;
                        case "UInt16":
                            // block.readUnsignedShort();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readUnsignedShort"));
                            break;
                        case "Int32":
                            // block.readInt();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readInt"));
                            break;
                        case "Int64":
                            // block.readLong();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readLong"));
                            break;
                        case "Byte[]":
                            // block.readBytes();
                            il.Emit(OpCodes.Callvirt, typeof(Block).GetMethod("readBytes"));
                            break;
                        default:
                            il.Emit(OpCodes.Pop);
                            il.Emit(OpCodes.Ret);
                            break;
                    }
                    il.Emit(OpCodes.Callvirt, type.GetMethod("set_" + propertyInfo.Name, new Type[] { propertyInfo.PropertyType }));
                }
                il.Emit(OpCodes.Ret);
                cachedMappers.Add(type, dm.CreateDelegate(typeof(mapEntity)));
            }
            mapEntity invokeMapEntity = (mapEntity)cachedMappers[type];
            // For each row, map the row to an instance of T and yield return it
            return invokeMapEntity(block);
        }
    }
}
