﻿namespace FEZRepacker.XNB.Types
{
    class DictionaryContentType<K,V> : XNBContentType<Dictionary<K,V>> where K : notnull
    {
        private TypeAssemblyQualifier _name;

        public DictionaryContentType(XNBContentConverter converter) : base(converter) {
            // creating type assembly qualifier name, since we're using own types
            _name = BasicType.FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "DictionaryReader";
        }

        public override TypeAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            Dictionary<K, V> data = new Dictionary<K, V>();
            int dataCount = reader.ReadInt32();
            for(int i = 0; i < dataCount; i++)
            {
                K? key = _converter.ReadType<K>(reader);
                V? value = _converter.ReadType<V>(reader);
                if(key != null && value != null) data[key] = value;
            }
            return data;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Dictionary<K, V> dict = (Dictionary<K, V>)data;

            writer.Write(dict.Count);
            foreach((K k, V v) in dict)
            {
                _converter.WriteType<K>(k, writer);
                _converter.WriteType<V>(v, writer);
            }
        }
    }
}
