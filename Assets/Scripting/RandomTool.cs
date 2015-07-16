using System;
using UnityEngine;
using Random = System.Random;

namespace MilitaryDemo
{
    public static class RandomTool
    {
        public static Random random = new Random((int)DateTime.Now.Ticks);

        public static int NextInt(int min, int max)
        {
            return random.Next(min, max);
        }

        public static int NextInt(int max)
        {
            if (max >= 0)
                return random.Next(max);
            return -12;//quite random, I think
        }

        public static int NextInt()
        {
            return random.Next();
        }

        public static char NextChoice(params char[] objects)
        {
            return objects[NextInt(objects.Length)];
        }

        public static string NextChoice(params string[] objects)
        {
            return objects[NextInt(objects.Length)];
        }

        public static int NextChoice(params int[] objects)
        {
            return objects[NextInt(objects.Length)];
        }

        public static byte NextByte()
        {
            return (byte)random.Next();
        }
        public static byte NextByte(byte max)
        {
            return (byte)random.Next(max);
        }
        public static byte NextByte(byte min, byte max)
        {
            return (byte)random.Next(min, max);
        }

        public static double NextDouble()
        {
            return random.NextDouble();
        }

        public static float NextSingle()
        {
            return (float)random.NextDouble();
        }

        public static float NextSingle(float min, float max)
        {
            return (max - min) * NextSingle() + min;
        }

        public static bool NextBool(float ratio)
        {
            return random.NextDouble() <= ratio;
        }

        public static bool NextBool(double ratio)
        {
            return random.NextDouble() <= ratio;
        }

        public static bool NextBool()
        {
            return random.NextDouble() <= 0.5;
        }

        public static sbyte NextSign()
        {
            return random.NextDouble() <= 0.5 ? (sbyte)1 : (sbyte)-1;
        }
        static public Color NextColor()
        {
            return new Color(NextByte() / 255f, NextByte() / 255f, NextByte() / 255f, 1f);
        }
        static public Vector2 NextUnitVector2()
        {
            float radians = RandomTool.NextSingle(-Mathf.PI, Mathf.PI);
            return new Vector2((float)Math.Cos(radians),(float)Math.Sin(radians));
        }
        static public Vector3 NextUnitVector3()
        {
            Single radians = RandomTool.NextSingle(-Mathf.PI, Mathf.PI);

            Single z = RandomTool.NextSingle(-1f, 1f);

            Single t = (float)Math.Sqrt(1f - (z * z));

            Vector2 planar = new Vector2
            {
                x = (float)Math.Cos(radians) * t,
                y = (float)Math.Sin(radians) * t
            };

            return new Vector3
            {
                x = planar.x,
                y = planar.y,
                z = z
            };
        }
    }
}