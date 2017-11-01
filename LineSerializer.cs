using SysDiag = System.Diagnostics;
using System.IO;
using UnityEngine;

namespace oi.plugin.linedrawing {

    public static class LinePointSerializer {

        public static byte[] Serialize(int lineID, int pointID, Vector3 pos) {
            byte[] data = null;

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {

                    writer.Write(3); // '3' announces line point packet
                    writer.Write(lineID);
                    writer.Write(pointID);
                    WritePosition(writer, pos);

                    stream.Position = 0;
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }
            }

            return data;
        }

        public static void Deserialize(byte[] data, out int lineID, out int pointID, out Vector3 pos) {
            pos = new Vector3();
            pointID = -1;
            lineID = -1;

            using (MemoryStream stream = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    int packetID = reader.ReadInt32();
                    if (packetID != 3) return;

                    lineID = reader.ReadInt32();
                    pointID = reader.ReadInt32();
                    pos = ReadPosition(reader);
                }
            }
        }

        private static void WritePosition(BinaryWriter writer, Vector3 location) {
            SysDiag.Debug.Assert(writer != null);

            writer.Write(location.x);
            writer.Write(location.y);
            writer.Write(location.z);
        }

        private static Vector3 ReadPosition(BinaryReader reader) {
            SysDiag.Debug.Assert(reader != null);

            Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return position;
        }

    }


    public static class LineSettingsSerializer {

        public static byte[] Serialize(int lineID, Color col, float width) {
            byte[] data = null;

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {

                    writer.Write(4); // '4' announces line settings packet
                    writer.Write(lineID);
                    WriteColor(writer, col);
                    writer.Write(width);

                    stream.Position = 0;
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }
            }

            return data;
        }

        public static void Deserialize(byte[] data, out int lineID, out Color col, out float width) {
            lineID = -1;
            col = new Color();
            width = 0.01f;

            using (MemoryStream stream = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    int packetID = reader.ReadInt32();
                    if (packetID != 4) return;

                    lineID = reader.ReadInt32();
                    col = ReadColor(reader);
                    width = reader.ReadSingle();
                }
            }
        }

        private static void WriteColor(BinaryWriter writer, Color col) {
            SysDiag.Debug.Assert(writer != null);

            writer.Write(col.r);
            writer.Write(col.g);
            writer.Write(col.b);
            writer.Write(col.a);
        }

        private static Color ReadColor(BinaryReader reader) {
            SysDiag.Debug.Assert(reader != null);

            Color col = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return col;
        }

    }


    public static class LineRemoveSerializer {

        public static byte[] Serialize(int lineID) {
            byte[] data = null;

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {

                    writer.Write(5); // '5' announces line remove packet
                    writer.Write(lineID);

                    stream.Position = 0;
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }
            }

            return data;
        }

        public static void Deserialize(byte[] data, out int lineID) {
            lineID = -1;

            using (MemoryStream stream = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    int packetID = reader.ReadInt32();
                    if (packetID != 5) return;

                    lineID = reader.ReadInt32();
                }
            }
        }

    }


    public static class LinesResetSerializer {

        public static byte[] Serialize() {
            byte[] data = null;

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {

                    writer.Write(6); // '6' announces lines reset packet

                    stream.Position = 0;
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }
            }

            return data;
        }

    }

}