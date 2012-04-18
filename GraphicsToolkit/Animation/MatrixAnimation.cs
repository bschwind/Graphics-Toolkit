using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Animation
{
    public class MatrixAnimation
    {
        private List<MatrixAnimationFrame> frames;
        private int numBodies;

        public MatrixAnimation(List<MatrixAnimationFrame> frames, int numBodies)
        {
            this.numBodies = numBodies;
            this.frames = frames;
        }

        public List<MatrixAnimationFrame> GetFrames()
        {
            return frames;
        }

        public int NumBodies()
        {
            return numBodies;
        }

        public static MatrixAnimation Parse(string fileName)
        {
            //Read the file, create a new Animation object, return it
            StreamReader sr = new StreamReader(fileName);

            List<MatrixAnimationFrame> frames = new List<MatrixAnimationFrame>();

            string numFramesString = sr.ReadLine();
            int numFrames = int.Parse(numFramesString.Split('\t')[1]);

            flushLines(sr, 1);

            string numMatricesString = sr.ReadLine();
            int numMatrices = int.Parse(numMatricesString.Split('\t')[1]);

            flushLines(sr, 8);

            string line = sr.ReadLine();
            while (line != null)
            {
                string[] splitString = line.Split('\t');
                Matrix[] matrices = new Matrix[numMatrices];
                for (int i = 0; i < numMatrices; i++)
                {
                    Matrix transform = Matrix.Identity;
                    int offset = i * 17 + 9;
                    transform.M11 = float.Parse(splitString[offset + 0]);
                    transform.M12 = float.Parse(splitString[offset + 1]);
                    transform.M13 = float.Parse(splitString[offset + 2]);
                    transform.M21 = float.Parse(splitString[offset + 3]);
                    transform.M22 = float.Parse(splitString[offset + 4]);
                    transform.M23 = float.Parse(splitString[offset + 5]);
                    transform.M31 = float.Parse(splitString[offset + 6]);
                    transform.M32 = float.Parse(splitString[offset + 7]);
                    transform.M33 = float.Parse(splitString[offset + 8]);
                    matrices[i] = transform;
                }

                frames.Add(new MatrixAnimationFrame(matrices));
                
                line = sr.ReadLine();
            }

            sr.Close();

            return new MatrixAnimation(frames, numMatrices);
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
