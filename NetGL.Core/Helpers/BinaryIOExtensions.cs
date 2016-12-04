using NetGL.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO {
    public static class BinaryIOExtensions {
        public static void Write(this BinaryWriter writer, Vector2 v) {
            Assert.NotNull(writer);

            writer.Write(v.X);
            writer.Write(v.Y);
        }
        public static void Write(this BinaryWriter writer, Vector3 v) {
            Assert.NotNull(writer);

            writer.Write(v.X);
            writer.Write(v.Y);
            writer.Write(v.Z);
        }
        public static void Write(this BinaryWriter writer, Vector4 v) {
            Assert.NotNull(writer);

            writer.Write(v.X);
            writer.Write(v.Y);
            writer.Write(v.Z);
            writer.Write(v.W);
        }

        public static Vector2 ReadVector2(this BinaryReader reader) {
            Assert.NotNull(reader);

            var result = new Vector2();
            result.X = reader.ReadSingle();
            result.Y = reader.ReadSingle();
            return result;
        }
        public static Vector3 ReadVector3(this BinaryReader reader) {
            Assert.NotNull(reader);

            var result = new Vector3();
            result.X = reader.ReadSingle();
            result.Y = reader.ReadSingle();
            result.Z = reader.ReadSingle();
            return result;
        }
        public static Vector4 ReadVector4(this BinaryReader reader) {
            Assert.NotNull(reader);

            var result = new Vector4();
            result.X = reader.ReadSingle();
            result.Y = reader.ReadSingle();
            result.Z = reader.ReadSingle();
            result.W = reader.ReadSingle();
            return result;
        }

        public static void WriteNullableString(this BinaryWriter writer, string text) {
            if (text == null) {
                writer.Write(false);
                return;
            }

            writer.Write(true);
            writer.Write(text);
        }
        public static string ReadNullbaleString(this BinaryReader reader) {
            var isNull = reader.ReadBoolean();
            if (isNull == false)
                return null;
            else
                return reader.ReadString();
        }
    }
}
