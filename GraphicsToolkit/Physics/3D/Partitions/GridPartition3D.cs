using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Bodies;
using GraphicsToolkit.Physics._3D.Geometry;

namespace GraphicsToolkit.Physics._3D.Partitions
{
    public class GridPartition3D : Partition3D
    {
        private Cell3D[] cells;
        private int rows, cols, stacks;
        private float invWidth, invHeight, invDepth;
        private Vector3 min, max;

        public GridPartition3D(Vector3 min, Vector3 max, int rows, int cols, int stacks)
        {
            cells = new Cell3D[rows * cols * stacks];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell3D();
            }

            this.rows = rows;
            this.cols = cols;
            this.stacks = stacks;

            UpdateMinMax(min, max);
        }

        public void UpdateMinMax(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;

            float width = (max.X - min.X);
            float height = (max.Y - min.Y);
            float depth = (max.Z - min.Z);
            invWidth = 1f / width;
            invHeight = 1f / height;
            invDepth = 1f / depth;
        }

        public override void GenerateContacts(ref List<RigidBody3D> bodies, ref List<Contact3D> contacts, float dt)
        {
            //Clear all cells
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Clear();
            }

            //Insert bodies into all cells they overlap
            for (int i = 0; i < bodies.Count; i++)
            {
                AABB3D bounds = bodies[i].MotionBounds;
                Vector3 min = bounds.GetMin();
                Vector3 max = bounds.GetMax();

                int startRow = (int)(min.X*invWidth*rows);
                startRow = (int)Math.Max(startRow, 0);

                int startCol = (int)(min.Y * invHeight * cols);
                startCol = (int)Math.Max(startCol, 0);

                int startStack = (int)(min.Z * invDepth * stacks);
                startStack = (int)Math.Max(startStack, 0);

                int endRow = (int)(max.X * invWidth * rows);
                endRow = (int)Math.Min(endRow, rows-1);

                int endCol = (int)(max.Y * invHeight * cols);
                endCol = (int)Math.Min(endCol, cols-1);

                int endStack = (int)(max.Z * invDepth * stacks);
                endStack = (int)Math.Min(endStack, stacks - 1);

                for (int j = startRow; j <= endRow; j++)
                {
                    for (int k = startCol; k <= endCol; k++)
                    {
                        for (int l = startStack; l <= endStack; l++)
                        {
                            cells[(j * cols) + (k*rows) + l].AddObject(i);
                        }  
                    }
                }
            }

            //Loop through each cell, add speculative contacts to each object in the cell
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].Indices.Count <= 0)
                {
                    continue;
                }

                Cell3D currentCell = cells[i];
                foreach (int index1 in currentCell.Indices)
                {
                    foreach (int index2 in currentCell.Indices)
                    {
                        if (index1 == index2)
                        {
                            continue;
                        }

                        RigidBody3D a = bodies[index1];
                        RigidBody3D b = bodies[index2];
                        if (a.InvMass != 0 || b.InvMass != 0)
                        {
                            //Add a speculative contact
                            contacts.Add(a.GenerateContact(b, dt));
                        }
                    }
                }
            }
        }

        internal class Cell3D
        {
            private LinkedList<int> indices;

            public Cell3D()
            {
                indices = new LinkedList<int>();
            }

            public void AddObject(int i)
            {
                indices.AddLast(i);
            }

            public void Clear()
            {
                indices.Clear();
            }

            public LinkedList<int> Indices
            {
                get
                {
                    return indices;
                }
            }
        }
    }
}
