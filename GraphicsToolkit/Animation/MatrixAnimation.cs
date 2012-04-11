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

        public MatrixAnimation(List<MatrixAnimationFrame> frames)
        {
            this.frames = frames;
        }

        public List<MatrixAnimationFrame> GetFrames()
        {
            return frames;
        }

        public static MatrixAnimation Parse(string fileName)
        {
            //Read the file, create a new Animation object, return it
            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();

            List<MatrixAnimationFrame> frames = new List<MatrixAnimationFrame>();

            while (line != null)
            {
                string[] splitString = line.Split('\t');
                int numMatrices = 2;
                Matrix[] matrices = new Matrix[numMatrices];
                for (int i = 0; i < numMatrices; i++)
                {
                    Matrix transform = Matrix.Identity;
                    int offset = i * 9;
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

            return new MatrixAnimation(frames);
        }
    }
}
