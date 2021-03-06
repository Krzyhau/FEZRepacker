namespace FEZRepacker.XNB
{
    abstract class XNBContentConverter
    {
        public abstract XNBContentType[] Types { get; }
        public XNBContentType PrimaryType => Types[0];
        public abstract string FileFormat { get; }

        public abstract void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter);
        public abstract void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter);

        protected virtual void ValidateType()
        {
            if (PrimaryType == null)
            {
                throw new InvalidDataException($"{this.GetType().Name} doesn't have primary type defined.");
            }
        }

        public XNBContentConverter()
        {
            ValidateType();
        }

        public T? ReadType<T>(BinaryReader reader, bool skipIdentifier = false)
        {
            if (typeof(T).IsPrimitive) skipIdentifier = true;

            int type = skipIdentifier ? 0 : reader.Read7BitEncodedInt();
            // if (type > 0 && type <= Types.Length)
            // {
            //     var typeConverter = Types[type - 1];
            //     if (typeConverter.BasicType == typeof(T))
            //     {
            //         return (T)typeConverter.Read(reader);
            //     }
            //     else
            //     {
            //         throw new InvalidDataException($"Tried to read {typeof(T).Name}, found {typeConverter.BasicType} instead.");
            //     }
            // }

            // since types used in different files may vary, we're not using
            // the lookup table. instead, just find the reader we need.

            if (type > 0 || skipIdentifier)
            {
                XNBContentType? typeConverter = Types.ToList().Find(t => t.BasicType == typeof(T));
                if (typeConverter != null)
                {
                    return (T)typeConverter.Read(reader);
                }
                else
                {
                    throw new InvalidDataException($"Cannot convert value of type {typeof(T).Name}");
                }
            }

            // type is either null or invalid
            return default(T);
        }

        public void WriteType<T>(T data, BinaryWriter writer, bool skipIdentifier = false)
        {
            if (typeof(T).IsPrimitive) skipIdentifier = true;

            int typeID = Types.ToList().FindIndex(t => t.BasicType == typeof(T));
            if (typeID >= 0 && data != null)
            {
                if (!skipIdentifier)
                {
                    writer.Write7BitEncodedInt(typeID + 1);
                }
                Types[typeID].Write(data, writer);
            }
            else
            {
                if(!skipIdentifier)
                {
                    Console.WriteLine($"WARNING! Couldn't assign type for {data} in {this.GetType()}");
                    writer.Write((byte)0x00);
                }
            }
        }
    }
}
