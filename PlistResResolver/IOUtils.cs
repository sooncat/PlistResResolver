using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PlistResResolver
{
    class IOUtils
    {
        public static string Byte2String(byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data);
        }

        public static byte[] ReadFileStream(string path)
        {
            byte[] b;
            using (Stream file = File.OpenRead(path))
            {
                b = new byte[(int)file.Length];
                file.Read(b, 0, b.Length);
                file.Close();
                file.Dispose();
            }
            return b;
        }

        /// <summary>
        /// 需要全路径和扩展名，覆盖模式
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        public static void SaveFile(string content, string path)
        {
            string folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);

            StreamWriter write = new StreamWriter(path, false, utf8WithoutBom); // Unity's TextAsset.text borks when encoding used is UTF8 :(
            write.Write(content);
            write.Flush();
            write.Close();
            write.Dispose();

            Console.Out.WriteLine("SaveFile = " + path);
        }

        /// <summary>
        /// 只能删除persistentPath目录下的文件，需要全路径和扩展名
        /// </summary>
        /// <param name="path"></param>
        public static void DelFile(string path)
        {
            File.Delete(path);
        }
    }
}
