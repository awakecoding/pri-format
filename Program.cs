using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace MyApp
{
    internal class Program
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
        public struct MrmDataItem
        {
            public uint unknown1;
            public uint unknown2;
            public uint dataSize;
            public uint dataOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
        public struct MrmDecnInfo
        {
            public uint unknown1;
            public uint unknown2;
            public uint dataSize;
            public uint dataOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
        public struct MrmPriDescEx
        {
            public uint unknown1;
            public uint unknown2;
            public uint dataSize;
            public uint dataOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
        public struct MrmHSchemaEx
        {
            public uint unknown1;
            public uint unknown2;
            public uint dataSize;
            public uint dataOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
        public struct MrmResMap2
        {
            public uint unknown1;
            public uint unknown2;
            public uint dataSize;
            public uint dataOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
        public struct DefHNamesEx
        {
            public uint unknown1;
            public uint unknown2;
            public uint dataSize;
            public uint dataOffset;
        }

        static void LoadPriFile(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);

            const ulong mrm_pri2 = 0x326972705F6D726D;
            const ulong mrm_decn_info_lo = 0x6365645F6D726D5B;
            const ulong mrm_decn_info_hi = 0x005D6F666E695F6E;
            const ulong mrm_pridescex_lo = 0x6972705F6D726D5B;
            const ulong mrm_pridescex_hi = 0x005D786563736564;
            const ulong mrm_hschemaex_lo = 0x6373685F6D726D5B;
            const ulong mrm_hschemaex_hi = 0x205D7865616D6568;
            const ulong mrm_res_map2_lo = 0x7365725F6D726D5B;
            const ulong mrm_res_map2_hi = 0x005D5F3270616D5F;
            const ulong mrm_dataitem_lo = 0x7461645F6D726D5B;
            const ulong mrm_dataitem_hi = 0x00205D6D65746961;
            const ulong def_hnamesx_lo = 0x616E685F6665645B;
            const ulong def_hnamesx_hi = 0x0020205D7873656D;

            long offset = 0;

            unsafe
            {
                fixed (byte* ptr = data)
                {
                    if (*((ulong*) &ptr[offset]) == mrm_pri2) {
                        Console.WriteLine("it's a PRI file!");
                        offset = 32;
                    }

                    while (*((ulong*) &ptr[offset+8]) != mrm_pri2) {
                        //Console.WriteLine("current offset: {0}", offset);
                        if ((*((ulong*) &ptr[offset]) == mrm_dataitem_lo) && (*((ulong*) &ptr[offset+8]) == mrm_dataitem_hi)) {
                            MrmDataItem* rec = (MrmDataItem*) &ptr[offset+16];
                            Console.WriteLine("[mrm_dataitem]: size: {0}, offset: {1}", rec->dataSize, rec->dataOffset);

                            if (rec->dataSize > 0 && rec->dataOffset == 0) {
                                offset += rec->dataSize;
                            } else {
                                offset += 32;
                            }
                        } else if ((*((ulong*) &ptr[offset]) == mrm_decn_info_lo) && (*((ulong*) &ptr[offset+8]) == mrm_decn_info_hi)) {
                            MrmDecnInfo* rec = (MrmDecnInfo*) &ptr[offset+16];
                            Console.WriteLine("[mrm_decn_info]: size: {0}, offset: {1}", rec->dataSize, rec->dataOffset);

                            if (rec->dataSize > 0 && rec->dataOffset == 0) {
                                offset += rec->dataSize;
                            } else {
                                offset += 32;
                            }
                        } else if ((*((ulong*) &ptr[offset]) == mrm_pridescex_lo) && (*((ulong*) &ptr[offset+8]) == mrm_pridescex_hi)) {
                            MrmPriDescEx* rec = (MrmPriDescEx*) &ptr[offset+16];
                            Console.WriteLine("[mrm_pridescex]: size: {0}, offset: {1}", rec->dataSize, rec->dataOffset);

                            // [mrm_pridescex]: size: 4376, offset: 400
                            // [mrm_pridescex]: size: 400, offset: 0

                            if (rec->dataSize > 0 && rec->dataOffset == 0) {
                                offset += rec->dataSize;
                            } else {
                                offset += 32;
                            }
                        } else if ((*((ulong*) &ptr[offset]) == mrm_hschemaex_lo) && (*((ulong*) &ptr[offset+8]) == mrm_hschemaex_hi)) {
                            MrmHSchemaEx* rec = (MrmHSchemaEx*) &ptr[offset+16];
                            Console.WriteLine("[mrm_hschemaex]: size: {0}, offset: {1}", rec->dataSize, rec->dataOffset);

                            // [mrm_hschemaex]: size: 4776, offset: 54576
                            // [mrm_hschemaex]: size: 54576, offset: 0

                            offset += 32;

                            if ((offset % 16) != 0) {
                                offset += 16 - (offset % 16);
                            }
                        } else if ((*((ulong*) &ptr[offset]) == mrm_res_map2_lo) && (*((ulong*) &ptr[offset+8]) == mrm_res_map2_hi)) {
                            MrmResMap2* rec = (MrmResMap2*) &ptr[offset+16];
                            Console.WriteLine("[mrm_res_map2_]: size: {0}, offset: {1}", rec->dataSize, rec->dataOffset);

                            // [mrm_res_map2_]: size: 59352, offset: 93280
                            // [mrm_res_map2_]: size: 93280, offset: 0

                            if (rec->dataSize > 0 && rec->dataOffset == 0) {
                                offset += rec->dataSize;
                            } else {
                                offset += 32;
                            }

                            // 64760 -> 158040 (93280)
                        } else if ((*((ulong*) &ptr[offset]) == def_hnamesx_lo) && (*((ulong*) &ptr[offset+8]) == def_hnamesx_hi)) {
                            DefHNamesEx* rec = (DefHNamesEx*) &ptr[offset+16];
                            Console.WriteLine("[def_hnamesx]: size: {0}, offset: {1}", rec->dataSize, rec->dataOffset);

                            // [def_hnamesx]: size: 4089234841, offset: 448

                            uint size1 = *((uint*) &ptr[offset + 160 + 4]);
                            uint size2 = *((uint*) &ptr[offset + 160 + 24]);

                            Console.WriteLine("size1: {0}, size2: {1}", size1, size2);

                            // 54536 = 32 + 144 + 54360
                            offset += 32 + size1 + size2;
                        } else {
                            Console.WriteLine("unknown record at offset {0}", offset);
                            return;
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            LoadPriFile("resources.pri");
        }
    }
}
