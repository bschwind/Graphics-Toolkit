using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Animation
{
    public class Animation
    {
        private AnimationFrame[] frames;
        private int numMarkers;

        public Animation(AnimationFrame[] frames, int numMarkers)
        {
            this.frames = frames;
            this.numMarkers = numMarkers;
        }

        public AnimationFrame[] GetFrames()
        {
            return frames;
        }

        public int GetNumMarkers()
        {
            return numMarkers;
        }

        public static Animation Parse(string fileName)
        {
            //Read the file, create a new Animation object, return it
            StreamReader sr = new StreamReader(fileName);
            string numFramesString = sr.ReadLine();
            int numFrames = int.Parse(numFramesString.Split('\t')[1]);

            flushLines(sr, 1);

            string numMarkersString = sr.ReadLine();
            int numMarkers = int.Parse(numMarkersString.Split('\t')[1]);

            flushLines(sr, 8);

            AnimationFrame[] frames = new AnimationFrame[numFrames];

            for (int i = 0; i < numFrames; i++)
            {
                Vector3[] positions = new Vector3[numMarkers];

                string frameString = sr.ReadLine();
                string[] splitString = frameString.Split('\t');

                int frameNum = int.Parse(splitString[0]);
                float time = float.Parse(splitString[1].Split(' ')[0]);

                for (int j = 0; j < numMarkers; j++)
                {
                    Vector3 pos = new Vector3();
                    //Account for the different coordinate system...examine this later
                    pos.X = float.Parse(splitString[2 + (3 * j)]);
                    pos.Y = float.Parse(splitString[3 + (3 * j)]);
                    pos.Z = float.Parse(splitString[4 + (3 * j)]);

                    positions[j] = pos;
                }

                AnimationFrame frame = new AnimationFrame(frameNum, time, positions);
                frames[i] = frame;
            }

            return new Animation(frames, numMarkers);
        }

        private static void flushLines(StreamReader sr, int numLines)
        {
            for (int i = 0; i < numLines; i++)
            {
                sr.ReadLine();
            }
        }
    }
}
