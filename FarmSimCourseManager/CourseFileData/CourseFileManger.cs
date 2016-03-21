using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.CourseFileData
{
    /// <summary>
    /// Раблота с файлом курса
    /// </summary>
    public static class CourseFileManger
    {
        private const string FromCourse = "FarmSimCourseManager.CourseFileData.FromCourse.xslt";
        private const string ToCourse = "FarmSimCourseManager.CourseFileData.ToCourse.xslt";

        private const string BackupExtension = ".backup";
        private const string BackupDateTimeFormat = "yyyyMMdd_HHmmss_FFFFFFF";

        /// <summary>
        /// Сериализатор файла маршрута
        /// </summary>
        private static readonly XmlSerializer SerializerCourseFile;
        /// <summary>
        /// Трансформация в удобоваримый сериализатором формат файла курса
        /// </summary>
        private static readonly XslCompiledTransform TransformFromCourse;
        /// <summary>
        /// Трансформация в формат файла курса после сериализатора
        /// </summary>
        private static readonly XslCompiledTransform TransformToCourse;

        static CourseFileManger()
        {
            SerializerCourseFile = new XmlSerializer(typeof(CourseFile));

            TransformFromCourse = new XslCompiledTransform();
            TransformFromCourse.Load(ReadResourceXml(FromCourse));

            TransformToCourse = new XslCompiledTransform();
            TransformToCourse.Load(ReadResourceXml(ToCourse));
        }

        public static CourseFile OpenFile(string filePath)
        {
            var xml = new XmlDocument();
            xml.Load(filePath);

            using (var stream = TransformFromCourse.TransformToXmlStream(xml))
            {
                var courseFile = (CourseFile)SerializerCourseFile.Deserialize(stream);
                return courseFile;
            }
        }

        public static void SaveFile(string filePath, CourseFile courseFile, bool isBackup = true, int coutBackups = 10)
        {
            if (isBackup && File.Exists(filePath))
            {
                var fileName = Path.GetFileName(filePath);
                var fileDir = Path.GetDirectoryName(filePath);
                RemoveOldBackups(fileDir, coutBackups);
                var fileBackName = string.Format("{0}.{1}{2}",
                    fileName,
                    string.Format("{0:" + BackupDateTimeFormat + "}", DateTime.Now),
                    BackupExtension);
                var fileBackPath = Path.Combine(fileDir, fileBackName);
                File.Copy(filePath, fileBackPath);
            }

            SaveAsFile(filePath, courseFile);
        }

        public static void SaveAsFile(string filePath, CourseFile courseFile)
        {
            using (var streamMiddle = new MemoryStream())
            using (var xmlWriterMiddle = XmlWriter.Create(streamMiddle))
            {
                //Create our own namespaces for the output
                var namespaces = new XmlSerializerNamespaces();
                //Add an empty namespace and empty value
                namespaces.Add("", "");

                SerializerCourseFile.Serialize(xmlWriterMiddle, courseFile, namespaces);
                streamMiddle.Flush();
                streamMiddle.Seek(0, SeekOrigin.Begin);

                using (var xmlReader = XmlReader.Create(streamMiddle))
                {
                    using (var streamOut = new FileStream(filePath, FileMode.Create))
                    {
                        TransformToCourse.TransformToXmlStream(xmlReader, streamOut);
                    }
                }
            }
        }

        private static void RemoveOldBackups(string fileDir, int countBackups)
        {
            var backupFiles = new List<string>(Directory.GetFiles(fileDir));

            // Выделяем файлы бекапа
            backupFiles = backupFiles
                .Where(v => v.EndsWith(BackupExtension, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var cultureInfo = CultureInfo.CurrentCulture;
            var backupExtensionlength = BackupExtension.Length;

            if (backupFiles.Count < countBackups)
                return;

            // Выделяем свои бекапы если попались не свои файлы
            var backupFilesDic = backupFiles
                .ToDictionary(
                    k => k,
                    v =>
                    {
                        DateTime dateTime;
                        var tryParseExact = DateTime.TryParseExact(
                            v.Substring(0, v.Length - backupExtensionlength)
                                .Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                .Last(),
                            BackupDateTimeFormat,
                            cultureInfo,
                            DateTimeStyles.None,
                            out dateTime);

                        return tryParseExact ? (DateTime?)dateTime : default(DateTime?);
                    })
                .Where(kv => kv.Value.HasValue)
                .ToDictionary(kv => kv.Key, kv => kv.Value.Value);

            var countFiles = backupFilesDic.Count;
            if (countFiles < countBackups)
                return;

            var countDeleteFiles = countFiles - (countBackups - 1);
            var deleteList = backupFilesDic
                .OrderBy(v => v.Value)
                .TakeWhile((kv, index) => index < countDeleteFiles)
                .Select(v => v.Key)
                .ToList();
            foreach (var fileName in deleteList)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private static XmlDocument ReadResourceXml(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                var xml = new XmlDocument();
                xml.Load(stream);
                return xml;
            }
        }
    }
}
