﻿using System;
using System.Runtime.InteropServices;
using System.Text;


namespace TDengineDriver
{
    public struct UTF8PtrStruct
    {
        public IntPtr utf8Ptr { get; set; }
        public int utf8StrLength { get; set; }

        public UTF8PtrStruct(string str)
        {
#if NETSTANDARD2_1_OR_GREATER ||NET5_0_OR_GREATER||NETCOREAPP1_1_OR_GREATER
            utf8StrLength = Encoding.UTF8.GetByteCount(str);
            utf8Ptr = Marshal.StringToCoTaskMemUTF8(str);

#else

            var utf8Bytes = Encoding.UTF8.GetBytes(str);
            utf8StrLength = utf8Bytes.Length;
            byte[] targetUtf8Bytes = new byte[utf8StrLength + 1];
            utf8Bytes.CopyTo(targetUtf8Bytes, 0);

            utf8Ptr = Marshal.AllocHGlobal(utf8StrLength + 1);
            Marshal.Copy(targetUtf8Bytes, 0, utf8Ptr, utf8StrLength + 1);
#endif
        }
        public void UTF8FreePtr()
        {
#if NETSTANDARD2_1_OR_GREATER ||NET5_0_OR_GREATER||NETCOREAPP1_1_OR_GREATER
            Marshal.FreeCoTaskMem(utf8Ptr);
#else 
            Marshal.FreeHGlobal(utf8Ptr);
#endif
        }

    }
}
