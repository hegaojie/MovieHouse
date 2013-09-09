using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MovieHouse
{
    public class MovieConfig
    {
        private const string DEFAULT_MCONFIG_PATH = @"../../../mconfig.xml";

        private readonly string _configFile;

        public MovieConfig(string configFile)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                _configFile = DEFAULT_MCONFIG_PATH;
            }
            _configFile = configFile;
        }

        public object Load(Type type)
        {
            if (!File.Exists(_configFile))
            {
                throw new Exception("The configuration file doesn't exist.");
            }

            object result = null;
            var serializer = new XmlSerializer(type);
            var reader = new StreamReader(_configFile);

            try
            {
                result = serializer.Deserialize(reader.BaseStream);
            }
            catch (Exception e)
            {
                //throw new Exception("Configure file is empty or damaged.");
            }
            finally
            {
                reader.Close();
            }

            return result;
        }

        public void Save(ICollection<Movie> movies)
        {
            if (!File.Exists(_configFile))
            {
                File.Create(_configFile);
            }

            var serializer = new XmlSerializer(movies.GetType());
            var writer = new StreamWriter(_configFile);
            serializer.Serialize(writer.BaseStream, movies);
            writer.Close();
        }
    }
}