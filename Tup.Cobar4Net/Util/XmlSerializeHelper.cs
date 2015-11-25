using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Tup.Cobar4Net
{
    /// <summary>
    /// XML 序列化 助手
    /// </summary>
    public static class XmlSerializeHelper
    {
        #region 序列化

        /// <summary>
        /// XML 序列化某一类型对象到指定的文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <exception cref="ArgumentNullException">filePath arg is null</exception>
        /// <exception cref="ArgumentNullException">obj arg is null</exception>
        public static void SerializeToXml<T>(string filePath, T obj)
            where T : class
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath", "filePath arg is null");
            if (obj == null)
                throw new ArgumentNullException("obj", "filePath arg is null");

            using (var writer = new StreamWriter(filePath))
            {
                var xs = new XmlSerializer(typeof(T));
                xs.Serialize(writer, obj);
            }
        }

        #endregion 序列化

        #region 反序列化

        /// <summary>
        /// 从某一 XML 文件反序列化到某一类型对象
        /// </summary>
        /// <param name="filePath">待反序列化的 XML 文件名称</param>
        /// <param name="type">反序列化出的</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">filePath arg is null</exception>
        /// <exception cref="FileNotFoundException">filePath File not found</exception>
        public static T DeserializeFromXml<T>(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath", "filePath arg is null");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("filePath File not found");

            using (var reader = new StreamReader(filePath))
            {
                return DeserializeFromXml<T>(reader);
            }
        }

        /// <summary>
        /// 从某一 XML 文件反序列化到某一类型对象
        /// </summary>
        /// <typeparam name="T">反序列化出的目标类型</typeparam>
        /// <param name="inStream">待反序列化的 XML 文件</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">filePath arg is null</exception>
        public static T DeserializeFromXml<T>(Stream inStream)
        {
            if (inStream == null)
                throw new ArgumentNullException("inStream", "inStream arg is null");

            using (var reader = new StreamReader(inStream))
            {
                return DeserializeFromXml<T>(reader);
            }
        }

        /// <summary>
        /// 从某一 XML 文件反序列化到某一类型对象
        /// </summary>
        /// <typeparam name="T">反序列化出的目标类型</typeparam>
        /// <param name="textReader">待反序列化的 XML 文件</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">filePath arg is null</exception>
        public static T DeserializeFromXml<T>(TextReader textReader)
        {
            if (textReader == null)
                throw new ArgumentNullException("textReader", "textReader arg is null");

            var xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(textReader);
        }

        /// <summary>
        /// 从某一 XML数据 文件反序列化到某一类型对象
        /// </summary>
        /// <typeparam name="T">反序列化出的目标类型</typeparam>
        /// <param name="xmlReader">待反序列化的 XML 数据</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">filePath arg is null</exception>
        public static T DeserializeFromXml<T>(XmlReader xmlReader)
        {
            if (xmlReader == null)
                throw new ArgumentNullException("xmlReader", "xmlReader arg is null");

            var xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(xmlReader);
        }

        #endregion 反序列化
    }
}
