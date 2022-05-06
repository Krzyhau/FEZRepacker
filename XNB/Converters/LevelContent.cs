﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using FEZEngine;
using YamlDotNet.Core;

namespace FEZRepacker.XNB.Converters
{
    struct LevelData
    {
        public string Name;

        [YamlMember(SerializeAs = typeof(string))]
        public Vector3 Size;
        public TrileFace StartingFace;
        public string SequenceSamplesPath;
        public bool Flat;
        public bool SkipPostProcess;
        public float BaseDiffuse;
        public float BaseAmbient;
        public string GomezHaloName;
        public bool HaloFiltering;
        public bool BlinkingAlpha;
        public bool Loops;
        public int WaterType;
        public float WaterHeight;
        public string SkyName;
        public string TrileSetName;
        public Dictionary<int, Volume> Volumes;
        public Dictionary<int, Script> Scripts;
        public string SongName;
        public int FAPFadeOutStart;
        public int FAPFadeoutLength;
    }

    class LevelContent : XNBContent
    {
        public LevelData Data { get; set; }
        public override XNBContentConverter Converter => LevelContentConverter.Instance;
    }

    class LevelContentConverter : XNBContentConverter
    {
        public static readonly LevelContentConverter Instance = new LevelContentConverter();

        public override TypeAssemblyQualifier[] DataTypes => new TypeAssemblyQualifier[] {
            new("FezEngine.Readers.LevelReader"),
            new("Microsoft.Xna.Framework.Content.StringReader"),
            new("FezEngine.Readers.TrileFaceReader"),
            new("FezEngine.Readers.TrileEmplacementReader"),
            new("Microsoft.Xna.Framework.Content.EnumReader`1[[FezEngine.FaceOrientation]]"),
            new("Microsoft.Xna.Framework.Content.Int32Reader"),
            new("Microsoft.Xna.Framework.Content.EnumReader`1[[FezEngine.Structure.LiquidType]]"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.Int32],[FezEngine.Structure.Volume]]"),
            new("FezEngine.Readers.VolumeReader"),
            new("Microsoft.Xna.Framework.Content.ArrayReader`1[[FezEngine.FaceOrientation]]"),
            new("FezEngine.Readers.VolumeActorSettingsReader"),
            new("Microsoft.Xna.Framework.Content.ListReader`1[[FezEngine.Structure.DotDialogueLine]]"),
            new("FezEngine.Readers.DotDialogueLineReader"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.Int32],[FezEngine.Structure.Scripting.Script]]"),
            new("FezEngine.Readers.ScriptReader"),
            new("Microsoft.Xna.Framework.Content.ListReader`1[[FezEngine.Structure.Scripting.ScriptTrigger]]"),
            new("FezEngine.Readers.ScriptTriggerReader"),
            new("FezEngine.Readers.EntityReader"),
            new("Microsoft.Xna.Framework.Content.ListReader`1[[FezEngine.Structure.Scripting.ScriptAction]]"),
            new("FezEngine.Readers.ScriptActionReader"),
            new("Microsoft.Xna.Framework.Content.ArrayReader`1[[System.String]]"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[FezEngine.Structure.TrileEmplacement],[FezEngine.Structure.TrileInstance]]"),
            new("FezEngine.Readers.TrileInstanceReader"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.Int32],[FezEngine.Structure.ArtObjectInstance]]"),
            new("FezEngine.Readers.ArtObjectInstanceReader"),
            new("FezEngine.Readers.ArtObjectActorSettingsReader"),
            new("Microsoft.Xna.Framework.Content.EnumReader`1[[FezEngine.Structure.ActorType]]"),
            new("Microsoft.Xna.Framework.Content.EnumReader`1[[FezEngine.Viewpoint]]"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.Int32],[FezEngine.Structure.BackgroundPlane]]"),
            new("FezEngine.Readers.BackgroundPlaneReader"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.Int32],[FezEngine.Structure.TrileGroup]]"),
            new("FezEngine.Readers.TrileGroupReader"),
            new("Microsoft.Xna.Framework.Content.ListReader`1[[FezEngine.Structure.TrileInstance]]"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.Int32],[FezEngine.Structure.NpcInstance]]"),
            new("FezEngine.Readers.NpcInstanceReader"),
            new("Microsoft.Xna.Framework.Content.ListReader`1[[FezEngine.Structure.SpeechLine]]"),
            new("FezEngine.Readers.SpeechLineReader"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[FezEngine.Structure.NpcAction],[FezEngine.Structure.NpcActionContent]]"),
            new("Microsoft.Xna.Framework.Content.EnumReader`1[[FezEngine.Structure.NpcAction]]"),
            new("FezEngine.Readers.NpcActionContentReader"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.Int32],[FezEngine.Structure.MovementPath]]"),
            new("FezEngine.Readers.MovementPathReader"),
            new("Microsoft.Xna.Framework.Content.ListReader`1[[System.String]]"),
            new("Microsoft.Xna.Framework.Content.ListReader`1[[FezEngine.Structure.AmbienceTrack]]"),
            new("FezEngine.Readers.AmbienceTrackReader"),
            new("Microsoft.Xna.Framework.Content.EnumReader`1[[FezEngine.LevelNodeType]]")
        };

        public override string FileFormat => "fezlvl";

        public override XNBContent Read(BinaryReader reader)
        {
            LevelContent content = new LevelContent();

            LevelData lvlData = new LevelData();

            reader.ReadByte(); // 0x02
            lvlData.Name = reader.ReadString();

            lvlData.Size = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );

            reader.ReadByte(); // 0x03
            reader.ReadByte(); // 0x04
            lvlData.StartingFace.Position = new TrileEmplacement(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32()
            );
            reader.ReadByte(); // 0x05
            lvlData.StartingFace.Orientation = (FaceOrientation)reader.ReadInt32();

            if(reader.ReadByte() > 0) lvlData.SequenceSamplesPath = reader.ReadString();

            lvlData.Flat = reader.ReadBoolean();
            lvlData.SkipPostProcess = reader.ReadBoolean();

            lvlData.BaseDiffuse = reader.ReadSingle();
            lvlData.BaseAmbient = reader.ReadSingle();
            if (reader.ReadByte() > 0) lvlData.GomezHaloName = reader.ReadString();
            lvlData.HaloFiltering = reader.ReadBoolean();
            lvlData.BlinkingAlpha = reader.ReadBoolean();
            lvlData.Loops = reader.ReadBoolean();
            reader.ReadByte(); // 0x07
            lvlData.WaterType = reader.ReadInt32();
            lvlData.WaterHeight = reader.ReadSingle();
            lvlData.SkyName = reader.ReadString();
            if (reader.ReadByte() > 0) lvlData.TrileSetName = reader.ReadString();

            reader.ReadByte(); // 0x08
            lvlData.Volumes = new Dictionary<int, Volume>();
            int volumesCount = reader.ReadInt32();
            for (int i = 0; i < volumesCount; i++)
            {
                int volumeID = reader.ReadInt32();
                reader.ReadByte(); // 0x09
                Volume volume = new Volume();

                reader.ReadByte(); // 0x0A
                List<FaceOrientation> orientations = new();
                int orientationsCount = reader.ReadInt32();
                for(int j = 0; j < orientationsCount; j++)
                {
                    orientations.Add((FaceOrientation)reader.ReadInt32());
                }
                volume.Orientations = orientations.ToArray();

                volume.From = new Vector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );

                volume.To = new Vector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );

            }


            content.Data = lvlData;

            return content;
        }

        public override void Write(XNBContent data, BinaryWriter writer)
        {
            if (!(data is LevelContent)) throw new InvalidDataException();
            LevelContent lvlData = (LevelContent)data;

        }


        public override XNBContent ReadUnpacked(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteUnpacked(XNBContent data, BinaryWriter writer)
        {
            if (!(data is LevelContent)) throw new InvalidDataException();
            LevelContent lvlData = (LevelContent)data;

            var serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            var yaml = serializer.Serialize(lvlData.Data);

            writer.Write(Encoding.UTF8.GetBytes(yaml));
        }
    }
}
