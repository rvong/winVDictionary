using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace winVDictionary
{
    static class MyDir
    {

        public static List<string> find(string path, string patt)
        {
            WIN32_FIND_DATA findData;
            IntPtr findHandle = FindFirstFile(Path.Combine(path, patt), out findData);
            List<string> result = new List<string>();

            if (findHandle != INVALID_HANDLE_VALUE)
            {
                do
                {
                    string name = findData.cFileName;
                    if (name == "." || name == "..") continue;

                    bool isDir = (findData.dwFileAttributes & FileAttributes.Directory) != 0;
                    result.Add(Path.Combine(path, findData.cFileName) + " " + isDir);

                    if (isDir) result.AddRange(find(Path.Combine(path, name), "*"));
                } while (FindNextFile(findHandle, out findData));
            }
            else
            {
                Debug.WriteLine("[Invalid File Handle" + path + "]");
                
            }

            FindClose(findHandle);
            return result;
        }

    #region Import from kernel32

    private const int MAX_PATH = 260;

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    [BestFitMapping(false)]
    private struct WIN32_FIND_DATA
    {
        public FileAttributes dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternate;
    }

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FindClose(IntPtr hFindFile);

    private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    #endregion
    }
}
