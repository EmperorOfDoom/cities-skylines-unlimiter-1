using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;
using EightyOne.Attributes;
using System;

namespace EightyOne.Zones
{
    /// <summary>
    /// None of these methods are exported, as ZoneBlock is a struct.
    /// </summary>
    internal class FakeZoneBlock
    {
        public static void CalculateBlock2(ref ZoneBlock z, ushort blockID)
        {
            if (((int)z.m_flags & 3) != 1)
                return;
            int rowCount = z.RowCount;
            Vector2 vector2_1 = new Vector2(Mathf.Cos(z.m_angle), Mathf.Sin(z.m_angle)) * 8f;
            Vector2 vector2_2 = new Vector2(vector2_1.y, -vector2_1.x);
            Vector2 vector2_3 = VectorUtils.XZ(z.m_position);
            Vector2 vector2_4 = vector2_3 - 4f * vector2_1 - 4f * vector2_2;
            Vector2 vector2_5 = vector2_3 + 0.0f * vector2_1 - 4f * vector2_2;
            Vector2 vector2_6 = vector2_3 + 0.0f * vector2_1 + (float)(rowCount - 4) * vector2_2;
            Vector2 vector2_7 = vector2_3 - 4f * vector2_1 + (float)(rowCount - 4) * vector2_2;
            float minX = Mathf.Min(Mathf.Min(vector2_4.x, vector2_5.x), Mathf.Min(vector2_6.x, vector2_7.x));
            float minZ = Mathf.Min(Mathf.Min(vector2_4.y, vector2_5.y), Mathf.Min(vector2_6.y, vector2_7.y));
            float maxX = Mathf.Max(Mathf.Max(vector2_4.x, vector2_5.x), Mathf.Max(vector2_6.x, vector2_7.x));
            float maxZ = Mathf.Max(Mathf.Max(vector2_4.y, vector2_5.y), Mathf.Max(vector2_6.y, vector2_7.y));
            ulong valid = z.m_valid;
            ulong shared = 0UL;
            ZoneManager instance = Singleton<ZoneManager>.instance;
            for (int index = 0; index < instance.m_cachedBlocks.m_size; ++index)
                CalculateImplementation2(ref z, blockID, ref instance.m_cachedBlocks.m_buffer[index], ref valid, ref shared, minX, minZ, maxX, maxZ);
            int num1 = Mathf.Max((int)(((double)minX - 46.0) / 64f + FakeZoneManager.HALFGRID), 0);
            int num2 = Mathf.Max((int)(((double)minZ - 46.0) / 64f + FakeZoneManager.HALFGRID), 0);
            int num3 = Mathf.Min((int)(((double)maxX + 46.0) / 64f + FakeZoneManager.HALFGRID), FakeZoneManager.GRIDSIZE - 1);
            int num4 = Mathf.Min((int)(((double)maxZ + 46.0) / 64f + FakeZoneManager.HALFGRID), FakeZoneManager.GRIDSIZE - 1);
            for (int index1 = num2; index1 <= num4; ++index1)
            {
                for (int index2 = num1; index2 <= num3; ++index2)
                {
                    ushort num5 = FakeZoneManager.zoneGrid[index1 * FakeZoneManager.GRIDSIZE + index2];
                    int num6 = 0;
                    while ((int)num5 != 0)
                    {
                        Vector3 vector3 = instance.m_blocks.m_buffer[(int)num5].m_position;
                        if ((double)Mathf.Max(Mathf.Max(minX - 46f - vector3.x, minZ - 46f - vector3.z), Mathf.Max((float)((double)vector3.x - (double)maxX - 46.0), (float)((double)vector3.z - (double)maxZ - 46.0))) < 0.0 && (int)num5 != (int)blockID)
                            CalculateImplementation2(ref z, blockID, ref instance.m_blocks.m_buffer[(int)num5], ref valid, ref shared, minX, minZ, maxX, maxZ);
                        num5 = instance.m_blocks.m_buffer[(int)num5].m_nextGridBlock;
                        if (++num6 >= 32768)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            ulong num7 = 144680345676153346UL;
            for (int index = 0; index < 7; ++index)
            {
                valid = (ulong)((long)valid & ~(long)num7 | (long)valid & (long)valid << 1 & (long)num7);
                num7 <<= 1;
            }
            z.m_valid = valid;
            z.m_shared = shared;
        }

        [Fixme("Make this more lightweight, ie. only a thin wrapper.")]
        private static void CalculateImplementation2(ref ZoneBlock z, ushort blockID, ref ZoneBlock other, ref ulong valid, ref ulong shared, float minX, float minZ, float maxX, float maxZ)
        {
            if (((int)other.m_flags & 1) == 0 || (double)Mathf.Abs(other.m_position.x - z.m_position.x) >= 92.0 || (double)Mathf.Abs(other.m_position.z - z.m_position.z) >= 92.0)
                return;
            bool flag1 = ((int)other.m_flags & 2) != 0;
            int rowCount1 = z.RowCount;
            int rowCount2 = other.RowCount;
            Vector2 vector2_1 = new Vector2(Mathf.Cos(z.m_angle), Mathf.Sin(z.m_angle)) * 8f;
            Vector2 vector2_2 = new Vector2(vector2_1.y, -vector2_1.x);
            Vector2 vector2_3 = new Vector2(Mathf.Cos(other.m_angle), Mathf.Sin(other.m_angle)) * 8f;
            Vector2 vector2_4 = new Vector2(vector2_3.y, -vector2_3.x);
            Vector2 vector2_5 = VectorUtils.XZ(other.m_position);
            Quad2 quad = new Quad2();
            quad.a = vector2_5 - 4f * vector2_3 - 4f * vector2_4;
            quad.b = vector2_5 + 0.0f * vector2_3 - 4f * vector2_4;
            quad.c = vector2_5 + 0.0f * vector2_3 + (float)(rowCount2 - 4) * vector2_4;
            quad.d = vector2_5 - 4f * vector2_3 + (float)(rowCount2 - 4) * vector2_4;
            Vector2 vector2_6 = quad.Min();
            Vector2 vector2_7 = quad.Max();
            if ((double)vector2_6.x > (double)maxX || (double)vector2_6.y > (double)maxZ || ((double)minX > (double)vector2_7.x || (double)minZ > (double)vector2_7.y))
                return;
            Vector2 vector2_8 = VectorUtils.XZ(z.m_position);
            Quad2 quad2_1 = new Quad2();
            quad2_1.a = vector2_8 - 4f * vector2_1 - 4f * vector2_2;
            quad2_1.b = vector2_8 + 0.0f * vector2_1 - 4f * vector2_2;
            quad2_1.c = vector2_8 + 0.0f * vector2_1 + (float)(rowCount1 - 4) * vector2_2;
            quad2_1.d = vector2_8 - 4f * vector2_1 + (float)(rowCount1 - 4) * vector2_2;
            if (!quad2_1.Intersect(quad))
                return;
            for (int z1 = 0; z1 < rowCount1; ++z1)
            {
                Vector2 vector2_9 = ((float)z1 - 3.99f) * vector2_2;
                Vector2 vector2_10 = ((float)z1 - 3.01f) * vector2_2;
                quad2_1.a = vector2_8 - 4f * vector2_1 + vector2_9;
                quad2_1.b = vector2_8 + 0.0f * vector2_1 + vector2_9;
                quad2_1.c = vector2_8 + 0.0f * vector2_1 + vector2_10;
                quad2_1.d = vector2_8 - 4f * vector2_1 + vector2_10;
                if (quad2_1.Intersect(quad))
                {
                    for (int x1 = 0; (long)x1 < 4L && ((long)valid & 1L << (z1 << 3 | x1)) != 0L; ++x1)
                    {
                        Vector2 vector2_11 = ((float)x1 - 3.99f) * vector2_1;
                        Vector2 vector2_12 = ((float)x1 - 3.01f) * vector2_1;
                        Vector2 p = vector2_8 + (vector2_12 + vector2_11 + vector2_10 + vector2_9) * 0.5f;
                        if (Quad2.Intersect(quad.a - vector2_3 - vector2_4, quad.b + vector2_3 - vector2_4, quad.c + vector2_3 + vector2_4, quad.d - vector2_3 + vector2_4, p))
                        {
                            Quad2 quad2_2 = new Quad2();
                            quad2_2.a = vector2_8 + vector2_11 + vector2_9;
                            quad2_2.b = vector2_8 + vector2_12 + vector2_9;
                            quad2_2.c = vector2_8 + vector2_12 + vector2_10;
                            quad2_2.d = vector2_8 + vector2_11 + vector2_10;
                            bool flag2 = true;
                            bool flag3 = false;
                            for (int z2 = 0; z2 < rowCount2 && flag2; ++z2)
                            {
                                Vector2 vector2_13 = ((float)z2 - 3.99f) * vector2_4;
                                Vector2 vector2_14 = ((float)z2 - 3.01f) * vector2_4;
                                for (int x2 = 0; (long)x2 < 4L && flag2; ++x2)
                                {
                                    if (((long)other.m_valid & ~(long)other.m_shared & 1L << (z2 << 3 | x2)) != 0L)
                                    {
                                        Vector2 vector2_15 = ((float)x2 - 3.99f) * vector2_3;
                                        Vector2 vector2_16 = ((float)x2 - 3.01f) * vector2_3;
                                        float num1 = Vector2.SqrMagnitude(vector2_5 + (vector2_16 + vector2_15 + vector2_14 + vector2_13) * 0.5f - p);
                                        if ((double)num1 < 144.0)
                                        {
                                            if (!flag1)
                                            {
                                                float f = Mathf.Abs(other.m_angle - z.m_angle) * 0.6366197f;
                                                float num2 = f - Mathf.Floor(f);
                                                if ((double)num1 < 0.00999999977648258 && ((double)num2 < 0.00999999977648258 || (double)num2 > 0.990000009536743))
                                                {
                                                    if (x1 < x2 || x1 == x2 && z.m_buildIndex < other.m_buildIndex)
                                                        other.m_shared |= (ulong)(1L << (z2 << 3 | x2));
                                                    else
                                                        flag3 = true;
                                                }
                                                else if (quad2_2.Intersect(new Quad2()
                                                {
                                                    a = vector2_5 + vector2_15 + vector2_13,
                                                    b = vector2_5 + vector2_16 + vector2_13,
                                                    c = vector2_5 + vector2_16 + vector2_14,
                                                    d = vector2_5 + vector2_15 + vector2_14
                                                }))
                                                {
                                                    if (x2 >= 4 && x1 >= 4 || x2 < 4 && x1 < 4)
                                                    {
                                                        if (x2 >= 2 && x1 >= 2 || x2 < 2 && x1 < 2)
                                                        {
                                                            if (z.m_buildIndex < other.m_buildIndex)
                                                                other.m_valid &= (ulong)~(1L << (z2 << 3 | x2));
                                                            else
                                                                flag2 = false;
                                                        }
                                                        else if (x2 < 2)
                                                            flag2 = false;
                                                        else
                                                            other.m_valid &= (ulong)~(1L << (z2 << 3 | x2));
                                                    }
                                                    else if (x2 < 4)
                                                        flag2 = false;
                                                    else
                                                        other.m_valid &= (ulong)~(1L << (z2 << 3 | x2));
                                                }
                                            }
                                            if ((double)num1 < 36.0 && x1 < 4 && x2 < 4)
                                            {
                                                ItemClass.Zone zone1 = z.GetZone(x1, z1);
                                                ItemClass.Zone zone2 = other.GetZone(x2, z2);
                                                if (zone1 == ItemClass.Zone.Unzoned)
                                                    z.SetZone(x1, z1, zone2);
                                                else if (zone2 == ItemClass.Zone.Unzoned && !flag1)
                                                    other.SetZone(x2, z2, zone1);
                                            }
                                        }
                                    }
                                }
                            }
                            if (!flag2)
                            {
                                valid = valid & (ulong)~(1L << (z1 << 3 | x1));
                                break;
                            }
                            if (flag3)
                                shared = shared | (ulong)(1L << (z1 << 3 | x1));
                        }
                    }
                }
            }
        }

        public static void SimulationStep(ref ZoneBlock b, ushort blockID)
        {
            ZoneManager instance = Singleton<ZoneManager>.instance;
            int rowCount = b.RowCount;
            Vector2 vector = new Vector2(Mathf.Cos(b.m_angle), Mathf.Sin(b.m_angle)) * 8f;
            Vector2 vector2 = new Vector2(vector.y, -vector.x);
            ulong num = b.m_valid & ~(b.m_occupied1 | b.m_occupied2);
            int num2 = 0;
            ItemClass.Zone zone = ItemClass.Zone.Unzoned;
            int num3 = 0;
            while (num3 < 4 && zone == ItemClass.Zone.Unzoned)
            {
                num2 = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)rowCount);
                if ((num & 1uL << (num2 << 3)) != 0uL)
                {
                    zone = b.GetZone(0, num2);
                }
                num3++;
            }
            DistrictManager instance2 = Singleton<DistrictManager>.instance;
            byte district = instance2.GetDistrict(b.m_position);
            int num4;
            switch (zone)
            {
                case ItemClass.Zone.ResidentialLow:
                    num4 = instance.m_actualResidentialDemand;
                    num4 += instance2.m_districts.m_buffer[(int)district].CalculateResidentialLowDemandOffset();
                    break;
                case ItemClass.Zone.ResidentialHigh:
                    num4 = instance.m_actualResidentialDemand;
                    num4 += instance2.m_districts.m_buffer[(int)district].CalculateResidentialHighDemandOffset();
                    break;
                case ItemClass.Zone.CommercialLow:
                    num4 = instance.m_actualCommercialDemand;
                    num4 += instance2.m_districts.m_buffer[(int)district].CalculateCommercialLowDemandOffset();
                    break;
                case ItemClass.Zone.CommercialHigh:
                    num4 = instance.m_actualCommercialDemand;
                    num4 += instance2.m_districts.m_buffer[(int)district].CalculateCommercialHighDemandOffset();
                    break;
                case ItemClass.Zone.Industrial:
                    num4 = instance.m_actualWorkplaceDemand;
                    num4 += instance2.m_districts.m_buffer[(int)district].CalculateIndustrialDemandOffset();
                    break;
                case ItemClass.Zone.Office:
                    num4 = instance.m_actualWorkplaceDemand;
                    num4 += instance2.m_districts.m_buffer[(int)district].CalculateOfficeDemandOffset();
                    break;
                default:
                    return;
            }
            Vector2 a = VectorUtils.XZ(b.m_position);
            Vector2 vector3 = a - 3.5f * vector + ((float)num2 - 3.5f) * vector2;
            int[] tmpXBuffer = instance.m_tmpXBuffer;
            for (int i = 0; i < 13; i++)
            {
                tmpXBuffer[i] = 0;
            }
            Quad2 quad = default(Quad2);
            quad.a = a - 4f * vector + ((float)num2 - 10f) * vector2;
            quad.b = a + 3f * vector + ((float)num2 - 10f) * vector2;
            quad.c = a + 3f * vector + ((float)num2 + 2f) * vector2;
            quad.d = a - 4f * vector + ((float)num2 + 2f) * vector2;
            Vector2 vector4 = quad.Min();
            Vector2 vector5 = quad.Max();
            int num5 = Mathf.Max((int)((vector4.x - 46f) / 64f + FakeZoneManager.HALFGRID), 0);
            int num6 = Mathf.Max((int)((vector4.y - 46f) / 64f + FakeZoneManager.HALFGRID), 0);
            int num7 = Mathf.Min((int)((vector5.x + 46f) / 64f + FakeZoneManager.HALFGRID), FakeZoneManager.GRIDSIZE - 1);
            int num8 = Mathf.Min((int)((vector5.y + 46f) / 64f + FakeZoneManager.HALFGRID), FakeZoneManager.GRIDSIZE - 1);
            for (int j = num6; j <= num8; j++)
            {
                for (int k = num5; k <= num7; k++)
                {
                    ushort num9 = FakeZoneManager.zoneGrid[j * FakeZoneManager.GRIDSIZE + k];
                    int num10 = 0;
                    while (num9 != 0)
                    {
                        Vector3 position = instance.m_blocks.m_buffer[(int)num9].m_position;
                        float num11 = Mathf.Max(Mathf.Max(vector4.x - 46f - position.x, vector4.y - 46f - position.z), Mathf.Max(position.x - vector5.x - 46f, position.z - vector5.y - 46f));
                        if (num11 < 0f)
                        {
                            CheckBlock(b,ref instance.m_blocks.m_buffer[(int)num9], tmpXBuffer, zone, vector3, vector, vector2, quad);
                        }
                        num9 = instance.m_blocks.m_buffer[(int)num9].m_nextGridBlock;
                        if (++num10 >= 32768)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            for (int l = 0; l < 13; l++)
            {
                uint num12 = (uint)tmpXBuffer[l];
                int num13 = 0;
                bool flag = (num12 & 196608u) == 196608u;
                bool flag2 = false;
                while ((num12 & 1u) != 0u)
                {
                    num13++;
                    flag2 = ((num12 & 65536u) != 0u);
                    num12 >>= 1;
                }
                if (num13 == 5 || num13 == 6)
                {
                    if (flag2)
                    {
                        num13 -= Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) + 2;
                    }
                    else
                    {
                        num13 = 4;
                    }
                    num13 |= 131072;
                }
                else if (num13 == 7)
                {
                    num13 = 4;
                    num13 |= 131072;
                }
                if (flag)
                {
                    num13 |= 65536;
                }
                tmpXBuffer[l] = num13;
            }
            int num14 = tmpXBuffer[6] & 65535;
            if (num14 == 0)
            {
                return;
            }
            bool flag3 = IsGoodPlace(b,vector3);
            if (Singleton<SimulationManager>.instance.m_randomizer.Int32(100u) >= num4)
            {
                if (flag3)
                {
                    instance.m_goodAreaFound[(int)zone] = 1024;
                }
                return;
            }
            if (!flag3 && instance.m_goodAreaFound[(int)zone] > -1024)
            {
                if (instance.m_goodAreaFound[(int)zone] == 0)
                {
                    instance.m_goodAreaFound[(int)zone] = -1;
                }
                return;
            }
            int num15 = 6;
            int num16 = 6;
            bool flag4 = true;
            while (true)
            {
                if (flag4)
                {
                    while (num15 != 0)
                    {
                        if ((tmpXBuffer[num15 - 1] & 65535) != num14)
                        {
                            break;
                        }
                        num15--;
                    }
                    while (num16 != 12)
                    {
                        if ((tmpXBuffer[num16 + 1] & 65535) != num14)
                        {
                            break;
                        }
                        num16++;
                    }
                }
                else
                {
                    while (num15 != 0)
                    {
                        if ((tmpXBuffer[num15 - 1] & 65535) < num14)
                        {
                            break;
                        }
                        num15--;
                    }
                    while (num16 != 12)
                    {
                        if ((tmpXBuffer[num16 + 1] & 65535) < num14)
                        {
                            break;
                        }
                        num16++;
                    }
                }
                int num17 = num15;
                int num18 = num16;
                while (num17 != 0)
                {
                    if ((tmpXBuffer[num17 - 1] & 65535) < 2)
                    {
                        break;
                    }
                    num17--;
                }
                while (num18 != 12)
                {
                    if ((tmpXBuffer[num18 + 1] & 65535) < 2)
                    {
                        break;
                    }
                    num18++;
                }
                bool flag5 = num17 != 0 && num17 == num15 - 1;
                bool flag6 = num18 != 12 && num18 == num16 + 1;
                if (flag5 && flag6)
                {
                    if (num16 - num15 > 2)
                    {
                        break;
                    }
                    if (num14 <= 2)
                    {
                        if (!flag4)
                        {
                            goto Block_34;
                        }
                    }
                    else
                    {
                        num14--;
                    }
                }
                else if (flag5)
                {
                    if (num16 - num15 > 1)
                    {
                        goto Block_36;
                    }
                    if (num14 <= 2)
                    {
                        if (!flag4)
                        {
                            goto Block_38;
                        }
                    }
                    else
                    {
                        num14--;
                    }
                }
                else if (flag6)
                {
                    if (num16 - num15 > 1)
                    {
                        goto Block_40;
                    }
                    if (num14 <= 2)
                    {
                        if (!flag4)
                        {
                            goto Block_42;
                        }
                    }
                    else
                    {
                        num14--;
                    }
                }
                else
                {
                    if (num15 != num16)
                    {
                        goto IL_884;
                    }
                    if (num14 <= 2)
                    {
                        if (!flag4)
                        {
                            goto Block_45;
                        }
                    }
                    else
                    {
                        num14--;
                    }
                }
                flag4 = false;
            }
            num15++;
            num16--;
        Block_34:
            goto IL_891;
        Block_36:
            num15++;
        Block_38:
            goto IL_891;
        Block_40:
            num16--;
        Block_42:
        Block_45:
        IL_884:
        IL_891:
            int num19;
            int num20;
            if (num14 == 1 && num16 - num15 >= 1)
            {
                num15 += Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)(num16 - num15));
                num16 = num15 + 1;
                num19 = num15 + Singleton<SimulationManager>.instance.m_randomizer.Int32(2u);
                num20 = num19;
            }
            else
            {
                do
                {
                    num19 = num15;
                    num20 = num16;
                    if (num16 - num15 == 2)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            num20--;
                        }
                        else
                        {
                            num19++;
                        }
                    }
                    else if (num16 - num15 == 3)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            num20 -= 2;
                        }
                        else
                        {
                            num19 += 2;
                        }
                    }
                    else if (num16 - num15 == 4)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            num16 -= 2;
                            num20 -= 3;
                        }
                        else
                        {
                            num15 += 2;
                            num19 += 3;
                        }
                    }
                    else if (num16 - num15 == 5)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            num16 -= 3;
                            num20 -= 2;
                        }
                        else
                        {
                            num15 += 3;
                            num19 += 2;
                        }
                    }
                    else if (num16 - num15 >= 6)
                    {
                        if (num15 == 0 || num16 == 12)
                        {
                            if (num15 == 0)
                            {
                                num15 = 3;
                                num19 = 2;
                            }
                            if (num16 == 12)
                            {
                                num16 = 9;
                                num20 = 10;
                            }
                        }
                        else if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            num16 = num15 + 3;
                            num20 = num19 + 2;
                        }
                        else
                        {
                            num15 = num16 - 3;
                            num19 = num20 - 2;
                        }
                    }
                }
                while (num16 - num15 > 3 || num20 - num19 > 3);
            }
            int num21 = 4;
            int num22 = num16 - num15 + 1;
            BuildingInfo.ZoningMode zoningMode = BuildingInfo.ZoningMode.Straight;
            bool flag7 = true;
            for (int m = num15; m <= num16; m++)
            {
                num21 = Mathf.Min(num21, tmpXBuffer[m] & 65535);
                if ((tmpXBuffer[m] & 131072) == 0)
                {
                    flag7 = false;
                }
            }
            if (num16 > num15)
            {
                if ((tmpXBuffer[num15] & 65536) != 0)
                {
                    zoningMode = BuildingInfo.ZoningMode.CornerLeft;
                    num20 = num15 + num20 - num19;
                    num19 = num15;
                }
                if ((tmpXBuffer[num16] & 65536) != 0 && (zoningMode != BuildingInfo.ZoningMode.CornerLeft || Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0))
                {
                    zoningMode = BuildingInfo.ZoningMode.CornerRight;
                    num19 = num16 + num19 - num20;
                    num20 = num16;
                }
            }
            int num23 = 4;
            int num24 = num20 - num19 + 1;
            BuildingInfo.ZoningMode zoningMode2 = BuildingInfo.ZoningMode.Straight;
            bool flag8 = true;
            for (int n = num19; n <= num20; n++)
            {
                num23 = Mathf.Min(num23, tmpXBuffer[n] & 65535);
                if ((tmpXBuffer[n] & 131072) == 0)
                {
                    flag8 = false;
                }
            }
            if (num20 > num19)
            {
                if ((tmpXBuffer[num19] & 65536) != 0)
                {
                    zoningMode2 = BuildingInfo.ZoningMode.CornerLeft;
                }
                if ((tmpXBuffer[num20] & 65536) != 0 && (zoningMode2 != BuildingInfo.ZoningMode.CornerLeft || Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0))
                {
                    zoningMode2 = BuildingInfo.ZoningMode.CornerRight;
                }
            }
            ItemClass.SubService subService = ItemClass.SubService.None;
            ItemClass.Level level = ItemClass.Level.Level1;
            ItemClass.Service service;
            switch (zone)
            {
                case ItemClass.Zone.ResidentialLow:
                    service = ItemClass.Service.Residential;
                    subService = ItemClass.SubService.ResidentialLow;
                    break;
                case ItemClass.Zone.ResidentialHigh:
                    service = ItemClass.Service.Residential;
                    subService = ItemClass.SubService.ResidentialHigh;
                    break;
                case ItemClass.Zone.CommercialLow:
                    service = ItemClass.Service.Commercial;
                    subService = ItemClass.SubService.CommercialLow;
                    break;
                case ItemClass.Zone.CommercialHigh:
                    service = ItemClass.Service.Commercial;
                    subService = ItemClass.SubService.CommercialHigh;
                    break;
                case ItemClass.Zone.Industrial:
                    service = ItemClass.Service.Industrial;
                    break;
                case ItemClass.Zone.Office:
                    service = ItemClass.Service.Office;
                    subService = ItemClass.SubService.None;
                    break;
                default:
                    return;
            }
            BuildingInfo buildingInfo = null;
            Vector3 vector6 = Vector3.zero;
            int num25 = 0;
            int num26 = 0;
            int num27 = 0;
            BuildingInfo.ZoningMode zoningMode3 = BuildingInfo.ZoningMode.Straight;
            int num28 = 0;
            while (num28 < 6)
            {
                switch (num28)
                {
                    case 0:
                        if (zoningMode != BuildingInfo.ZoningMode.Straight)
                        {
                            num25 = num15 + num16 + 1;
                            num26 = num21;
                            num27 = num22;
                            zoningMode3 = zoningMode;
                            goto IL_D6A;
                        }
                        break;
                    case 1:
                        if (zoningMode2 != BuildingInfo.ZoningMode.Straight)
                        {
                            num25 = num19 + num20 + 1;
                            num26 = num23;
                            num27 = num24;
                            zoningMode3 = zoningMode2;
                            goto IL_D6A;
                        }
                        break;
                    case 2:
                        if (zoningMode != BuildingInfo.ZoningMode.Straight)
                        {
                            if (num21 >= 4)
                            {
                                num25 = num15 + num16 + 1;
                                num26 = ((!flag7) ? 2 : 3);
                                num27 = num22;
                                zoningMode3 = zoningMode;
                                goto IL_D6A;
                            }
                        }
                        break;
                    case 3:
                        if (zoningMode2 != BuildingInfo.ZoningMode.Straight)
                        {
                            if (num23 >= 4)
                            {
                                num25 = num19 + num20 + 1;
                                num26 = ((!flag8) ? 2 : 3);
                                num27 = num24;
                                zoningMode3 = zoningMode2;
                                goto IL_D6A;
                            }
                        }
                        break;
                    case 4:
                        num25 = num15 + num16 + 1;
                        num26 = num21;
                        num27 = num22;
                        zoningMode3 = BuildingInfo.ZoningMode.Straight;
                        goto IL_D6A;
                    case 5:
                        num25 = num19 + num20 + 1;
                        num26 = num23;
                        num27 = num24;
                        zoningMode3 = BuildingInfo.ZoningMode.Straight;
                        goto IL_D6A;
                    default:
                        goto IL_D6A;
                }
            IL_DF0:
                num28++;
                continue;
            IL_D6A:
                vector6 = b.m_position + VectorUtils.X_Y(((float)num26 * 0.5f - 4f) * vector + ((float)num25 * 0.5f + (float)num2 - 10f) * vector2);
                if (zone == ItemClass.Zone.Industrial)
                {
                    ZoneBlock.GetIndustryType(vector6, out subService, out level);
                }
                buildingInfo = Singleton<BuildingManager>.instance.GetRandomBuildingInfo(ref Singleton<SimulationManager>.instance.m_randomizer, service, subService, level, num27, num26, zoningMode3, 0);
                if (buildingInfo != null)
                {
                    break;
                }
                goto IL_DF0;
            }
            if (buildingInfo == null)
            {
                return;
            }
            float num29 = Singleton<TerrainManager>.instance.WaterLevel(VectorUtils.XZ(vector6));
            if (num29 > vector6.y)
            {
                return;
            }
            float num30 = b.m_angle + 1.57079637f;
            if (zoningMode3 == BuildingInfo.ZoningMode.CornerLeft && buildingInfo.m_zoningMode == BuildingInfo.ZoningMode.CornerRight)
            {
                num30 -= 1.57079637f;
                num26 = num27;
            }
            else if (zoningMode3 == BuildingInfo.ZoningMode.CornerRight && buildingInfo.m_zoningMode == BuildingInfo.ZoningMode.CornerLeft)
            {
                num30 += 1.57079637f;
                num26 = num27;
            }
            ushort num31;
            if (Singleton<BuildingManager>.instance.CreateBuilding(out num31, ref Singleton<SimulationManager>.instance.m_randomizer, buildingInfo, vector6, num30, num26, Singleton<SimulationManager>.instance.m_currentBuildIndex))
            {
                Singleton<SimulationManager>.instance.m_currentBuildIndex += 1u;
                switch (service)
                {
                    case ItemClass.Service.Residential:
                        instance.m_actualResidentialDemand = Mathf.Max(0, instance.m_actualResidentialDemand - 5);
                        break;
                    case ItemClass.Service.Commercial:
                        instance.m_actualCommercialDemand = Mathf.Max(0, instance.m_actualCommercialDemand - 5);
                        break;
                    case ItemClass.Service.Industrial:
                        instance.m_actualWorkplaceDemand = Mathf.Max(0, instance.m_actualWorkplaceDemand - 5);
                        break;
                    case ItemClass.Service.Office:
                        instance.m_actualWorkplaceDemand = Mathf.Max(0, instance.m_actualWorkplaceDemand - 5);
                        break;
                }
            }
            instance.m_goodAreaFound[(int)zone] = 1024;
        }

        [Fixme("Make this a lightweight wrapper")]
        internal static bool IsGoodPlace(ZoneBlock x, Vector2 position)
        {
            int num1 = Mathf.Max((int)(((double)position.x - 104.0) / 64.0 + 135.0), 0);
            int num2 = Mathf.Max((int)(((double)position.y - 104.0) / 64.0 + 135.0), 0);
            int num3 = Mathf.Min((int)(((double)position.x + 104.0) / 64.0 + 135.0), 269);
            int num4 = Mathf.Min((int)(((double)position.y + 104.0) / 64.0 + 135.0), 269);
            Array16<Building> array16 = Singleton<BuildingManager>.instance.m_buildings;
            ushort[] numArray = Singleton<BuildingManager>.instance.m_buildingGrid;
            for (int index1 = num2; index1 <= num4; ++index1)
            {
                for (int index2 = num1; index2 <= num3; ++index2)
                {
                    ushort num5 = numArray[index1 * 270 + index2];
                    int num6 = 0;
                    while ((int)num5 != 0)
                    {
                        if ((array16.m_buffer[(int)num5].m_flags & (Building.Flags.Created | Building.Flags.Deleted)) == Building.Flags.Created)
                        {
                            BuildingInfo info;
                            int width;
                            int length;
                            array16.m_buffer[(int)num5].GetInfoWidthLength(out info, out width, out length);
                            if (info != null)
                            {
                                float b = info.m_buildingAI.ElectricityGridRadius();
                                if ((double)b > 0.100000001490116 || info.m_class.m_service == ItemClass.Service.Electricity)
                                {
                                    Vector2 vector2 = VectorUtils.XZ(array16.m_buffer[(int)num5].m_position);
                                    float num7 = Mathf.Max(8f, b) + 32f;
                                    if ((double)Vector2.SqrMagnitude(position - vector2) < (double)num7 * (double)num7)
                                        return true;
                                }
                            }
                        }
                        num5 = array16.m_buffer[(int)num5].m_nextGridBuilding;
                        if (++num6 >= 32768)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            return false;
        }

        [Fixme("Make this a lightweight wrapper")]
        internal static void CheckBlock(ZoneBlock b, ref ZoneBlock other, int[] xBuffer, ItemClass.Zone zone, Vector2 startPos, Vector2 xDir, Vector2 zDir, Quad2 quad)
        {
            float f1 = Mathf.Abs(other.m_angle - b.m_angle) * 0.6366197f;
            float num1 = f1 - Mathf.Floor(f1);
            if ((double)num1 >= 0.00999999977648258 && (double)num1 <= 0.990000009536743)
                return;
            int rowCount = other.RowCount;
            Vector2 vector2_1 = new Vector2(Mathf.Cos(other.m_angle), Mathf.Sin(other.m_angle)) * 8f;
            Vector2 vector2_2 = new Vector2(vector2_1.y, -vector2_1.x);
            ulong num2 = other.m_valid & (ulong)~((long)other.m_occupied1 | (long)other.m_occupied2);
            Vector2 vector2_3 = VectorUtils.XZ(other.m_position);
            if (!quad.Intersect(new Quad2()
            {
                a = vector2_3 - 4f * vector2_1 - 4f * vector2_2,
                b = vector2_3 - 4f * vector2_2,
                c = vector2_3 + (float)(rowCount - 4) * vector2_2,
                d = vector2_3 - 4f * vector2_1 + (float)(rowCount - 4) * vector2_2
            }))
                return;
            for (int z = 0; z < rowCount; ++z)
            {
                Vector2 vector2_4 = ((float)z - 3.5f) * vector2_2;
                for (int x = 0; x < 4; ++x)
                {
                    if (((long)num2 & 1L << (z << 3 | x)) != 0L && other.GetZone(x, z) == zone)
                    {
                        Vector2 vector2_5 = ((float)x - 3.5f) * vector2_1;
                        Vector2 vector2_6 = vector2_3 + vector2_5 + vector2_4 - startPos;
                        float f2 = (float)(((double)vector2_6.x * (double)xDir.x + (double)vector2_6.y * (double)xDir.y) * (1.0 / 64.0));
                        float f3 = (float)(((double)vector2_6.x * (double)zDir.x + (double)vector2_6.y * (double)zDir.y) * (1.0 / 64.0));
                        int num3 = Mathf.RoundToInt(f2);
                        int num4 = Mathf.RoundToInt(f3);
                        if (num3 >= 0 && num3 <= 6 && (num4 >= -6 && num4 <= 6) && ((double)Mathf.Abs(f2 - (float)num3) < 0.0125000001862645 && (double)Mathf.Abs(f3 - (float)num4) < 0.0125000001862645 && (x == 0 || num3 != 0)))
                        {
                            xBuffer[num4 + 6] |= 1 << num3;
                            if (x == 0)
                                xBuffer[num4 + 6] |= 1 << num3 + 16;
                        }
                    }
                }
            }
        }
    }
}
