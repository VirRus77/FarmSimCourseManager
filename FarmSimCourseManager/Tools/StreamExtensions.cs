using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FarmSimCourseManager.Tools
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Создать копию входного потока в памяти.
        /// </summary>
        /// <param name="stream">входной поток</param>
        /// <param name="resetSourcePosition">флаг сброса позиции чтения во входном потоке</param>
        /// <returns>поток в памяти</returns>
        public static MemoryStream Duplicate(this Stream stream, bool resetSourcePosition)
        {
            var ms = new MemoryStream();
            ms.Write(stream);
            if (resetSourcePosition)
                stream.Seek(0, SeekOrigin.Begin);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// Прочитать все данные из входного потока в массив байтов.
        /// </summary>
        /// <param name="stream">входной поток</param>
        /// <returns>массив байтов данных из входного потока</returns>
        public static byte[] ReadAll(this Stream stream)
        {
            var buffers = new List<byte[]>();
            var totalSize = 0;

            const int readSize = 32 * 1024;
            var readBufferSize = stream.CanSeek ? Math.Min(readSize, stream.Length) : readSize;

            var readBuffer = new byte[readBufferSize];
            int bytesRead;
            while ((bytesRead = stream.Read(readBuffer, 0, readBuffer.Length)) != 0)
            {
                if (bytesRead == readBufferSize)
                {
                    // Поместить в список сам буфер, экономя на копировании.
                    buffers.Add(readBuffer);
                    // Выделить память под новый буфер.
                    readBuffer = new byte[readBufferSize];
                }
                else
                {
                    // Выделить память под буфер меньшего размера и перенести в него данные.
                    var readPart = new byte[bytesRead];
                    Array.Copy(readBuffer, 0, readPart, 0, bytesRead);
                    // Поместить в список блок данных меньшего размера.
                    buffers.Add(readPart);
                }
                totalSize += bytesRead;
            }

            // Собрать выходной буфер из списка буферов.
            var result = new byte[totalSize];
            var resultPos = 0;
            foreach (var rb in buffers)
            {
                Array.Copy(rb, 0, result, resultPos, rb.Length);
                resultPos += rb.Length;
            }
            Debug.Assert(resultPos == totalSize);

            return result;
        }

        /// <summary>
        /// Прочитать структуру из входного потока.
        /// </summary>
        /// <typeparam name="T">тип структуры</typeparam>
        /// <param name="stream">входной поток</param>
        /// <returns>прочитанный объект</returns>
        public static T ReadRaw<T>(this Stream stream)
            where T : struct
        {
            // Dispose у br не вызывать! Т.к. это закроет поток.
            var br = new BinaryReader(stream);
            var bytesToRead = Marshal.SizeOf(typeof(T));
            var raw = br.ReadBytes(bytesToRead);
            if (raw.Length != bytesToRead)
                throw new IOException("Can't read object");
            return DeserializeRawData<T>(raw);
        }

        /// <summary>
        /// Прочитать из потока 2-х байтовое целое без знака.
        /// </summary>
        /// <param name="stream">входной поток</param>
        /// <returns>2-х байтовое целое без знака</returns>
        [CLSCompliant(false)]
        public static UInt16 ReadUInt16(this Stream stream)
        {
            var b0 = stream.ReadByte();
            var b1 = stream.ReadByte();
            return (UInt16)(b0 + (b1 << 8));
        }

        /// <summary>
        /// Прочитать из потока массив байтов, завершаемый нулём.
        /// </summary>
        /// <param name="stream">входной поток</param>
        /// <returns>массив байтов, завершаемый нулём</returns>
        public static byte[] ReadAsciiZ(this Stream stream)
        {
            var bytes = new List<byte>();
            int b;
            while ((b = stream.ReadByte()) > 0)
                bytes.Add((byte)b);
            return bytes.ToArray();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Записать структуру в выходной поток.
        /// </summary>
        /// <typeparam name="T">тип структуры</typeparam>
        /// <param name="stream">выходной поток</param>
        /// <param name="anything">объект для записи</param>
        public static void WriteRaw<T>(this Stream stream, T anything)
            where T : struct
        {
            var raw = SerializeRawData(anything);
            stream.Write(raw, 0, raw.Length);
        }
#endif

        /// <summary>
        /// Записать содержимое одного потока в другой.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="src"></param>
        public static void Write(this Stream stream, Stream src)
        {
            const int readSize = 32 * 1024;
            var readBufferSize = src.CanSeek ? Math.Min(readSize, src.Length) : readSize;

            var buffer = new byte[readBufferSize];
            int bytesRead;
            while ((bytesRead = src.Read(buffer, 0, buffer.Length)) != 0)
                stream.Write(buffer, 0, bytesRead);
        }

        /// <summary>
        /// Преобразовать массив байтов в объект.
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="rawData">массив байтов</param>
        /// <returns>объект</returns>
        private static T DeserializeRawData<T>(byte[] rawData)
        {
            var handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Преобразовать объект в массив байтов.
        /// </summary>
        /// <param name="anything">объект</param>
        /// <returns>массив байтов</returns>
        private static byte[] SerializeRawData(object anything)
        {
            var rawsize = Marshal.SizeOf(anything);
            var rawData = new byte[rawsize];
            var buffer = Marshal.AllocHGlobal(rawsize);
            try
            {
                Marshal.StructureToPtr(anything, buffer, false);
                Marshal.Copy(buffer, rawData, 0, rawsize);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
            return rawData;
        }
#endif
    }
}
