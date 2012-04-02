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

        public static MatrixAnimation ParseRollPitchYaw(string fileName)
        {
            //Read the file, create a new Animation object, return it
            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();

            List<MatrixAnimationFrame> frames = new List<MatrixAnimationFrame>();

            while (line != null)
            {
                string[] splitString = line.Split(',');
                float roll = MathHelper.ToRadians(float.Parse(splitString[0]));
                float pitch = MathHelper.ToRadians(float.Parse(splitString[1]));
                float yaw = MathHelper.ToRadians(float.Parse(splitString[2]));
                frames.Add(new MatrixAnimationFrame(roll, pitch, yaw));
                line = sr.ReadLine();
            }

            sr.Close();

            return new MatrixAnimation(frames);
        }
    }
}
