using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//надо добавить для работы класса
using System.Xml.Serialization;

namespace GraphMaster_X.Classes
{
    //Класс определяющий какие настройки есть в программе
    [Serializable]
    public class PropsFields
    {
        public string XMLFileName = Environment.CurrentDirectory + "\\settings.xml";
        public string BinFileName = Environment.CurrentDirectory + "\\settings.dat";

        //Унаследуй этот класс и объяви в нем нужные поля для сериализации.
        //Потом используй вместо T в Props
    }
    //Класс работы с настройками
    [Serializable]
    public class BinProps<T> where T : PropsFields, new()
    {
        public T Fields;
        public BinProps()
        {
            Fields = new T();
        }
        //Запись настроек в файл
        public void WriteBin()
        {
            BinaryFormatter ser = new BinaryFormatter();
            using (FileStream fs = new FileStream(Fields.BinFileName, FileMode.OpenOrCreate))
            {
                ser.Serialize(fs, Fields);
            }
        }
        //Чтение насроек из файла
        public void ReadBin()
        {
            if (File.Exists(Fields.BinFileName))
            {
                BinaryFormatter ser = new BinaryFormatter();
                using (FileStream fs = new FileStream(Fields.BinFileName, FileMode.OpenOrCreate))
                {
                    Fields = (T)ser.Deserialize(fs);

                }
            }
        }
    }
    [Serializable]
    public class XmlProps<T> where T : PropsFields, new()
    {
        public T Fields;

        public XmlProps()
        {
            Fields = new T();
        }
        //Запись настроек в файл
        public void WriteXml()
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            TextWriter writer = new StreamWriter(Fields.XMLFileName);
            ser.Serialize(writer, Fields);
            writer.Close();
        }
        //Чтение насроек из файла
        public void ReadXml()
        {
            if (File.Exists(Fields.XMLFileName))
            {
                XmlSerializer ser = new XmlSerializer(typeof(PropsFields));
                TextReader reader = new StreamReader(Fields.XMLFileName);
                Fields = ser.Deserialize(reader) as T;
                reader.Close();
            }
        }
    }
}
