using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using FarmSimCourseManager.Contracts;
using FarmSimCourseManager.Tools;

namespace FarmSimCourseManager.Service
{
    [Serializable]
    [XmlRoot("Settings")]
    [XmlInclude(typeof(MapSettings))]
    public class Settings : ISettings
    {
        public Settings()
        {
            IsCreateBackup = true;
            CountBackups = 10;
            MapSettings = new MapSettings();
        }

        static XmlSerializer _serializer = new XmlSerializer(typeof(Settings), new[] { typeof(MapSettings) });

        [XmlAttribute]
        public bool IsCreateBackup { get; set; }

        [XmlAttribute]
        public int CountBackups { get; set; }

        [XmlIgnore]
        public IMapSettings MapSettings
        {
            get { return MapSettingsXml; }
            set { MapSettingsXml = (MapSettings) value; }
        }

        [XmlElement]
        public MapSettings MapSettingsXml { get; set; }

        public void Save(string fullFileName)
        {
            Save(fullFileName, this);
        }

        public static void Save(string fullFileName, Settings settings)
        {
            using (var streamMiddle = new FileStream(fullFileName, FileMode.Create))
            {
                var xmlSettings = new XmlWriterSettings
                {
                    Indent = true,
                };

                using (var xmlWriter = XmlWriter.Create(streamMiddle, xmlSettings))
                {
                    //Create our own namespaces for the output
                    var namespaces = new XmlSerializerNamespaces();
                    //Add an empty namespace and empty value
                    namespaces.Add("", "");



                    _serializer.Serialize(xmlWriter, settings, namespaces);
                    streamMiddle.Flush();
                    streamMiddle.Seek(0, SeekOrigin.Begin);
                }
            }
        }

        public static Settings Load(string fullFileName)
        {
            if (!File.Exists(fullFileName))
                return new Settings();
            using (var stream = new FileStream(fullFileName, FileMode.Open))
            {
                return (Settings)_serializer.Deserialize(stream);
            }
        }

        private void SetDefaultValue<T>(T valueObject)
        {
            var type = typeof(T);
            var properties = type.GetProperties().ToList();
            properties = properties
                .Where(propertyInfo => propertyInfo.CanWrite && propertyInfo.GetCustomAttributes(false).OfType<DefaultValueAttribute>().Any())
                .ToList();
            properties.ForEach(v => v.SetValue(valueObject, v.GetCustomAttributes(false).OfType<DefaultValueAttribute>().First().Value, new object[0]));
        }
    }

    [Export(typeof(ISettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsLoader : ISettings, IDisposable
    {
        private const string FileConfigName = "settings.xml";
        private readonly Settings _settings;

        ~SettingsLoader()
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine("SettingsLoader.~SettingsLoader()");
            _settings.Save(GetSettingsFullFileName());
        }

        public SettingsLoader()
        {
            var fullFileName = GetSettingsFullFileName();
            if (File.Exists(fullFileName))
                _settings = Settings.Load(fullFileName);
            else
            {
                _settings = new Settings();
                _settings.MapSettings.ImageStart = new ImageBinary
                {
                    ImageType = ImageType.Xaml,
                    Image = ReadResource("FarmSimCourseManager.Styles.Images.Xaml.StartImage.xaml"),
                    Width = 10,
                    Height = 10
                };
                _settings.MapSettings.ImageStop = new ImageBinary
                {
                    ImageType = ImageType.Xaml,
                    Image = ReadResource("FarmSimCourseManager.Styles.Images.Xaml.StopImage.xaml"),
                    Width = 10,
                    Height = 10
                };
            }
        }

        #region Implementation of ISettings

        public bool IsCreateBackup
        {
            get { return _settings.IsCreateBackup; }
            set { _settings.IsCreateBackup = value; }
        }

        public int CountBackups
        {
            get { return _settings.CountBackups; }
            set { _settings.CountBackups = value; }
        }

        public IMapSettings MapSettings
        {
            get { return _settings.MapSettings; }
            set { _settings.MapSettings = value; }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine("SettingsLoader.Dispose()");
            _settings.Save(GetSettingsFullFileName());
        }

        #endregion

        private static string GetSettingsFullFileName()
        {
            var workPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fullFileName = Path.Combine(workPath, FileConfigName);
            return fullFileName;
        }

        private static byte[] ReadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                return stream.ReadAll();
            }
        }
    }


}
