using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Bodies;
using GraphicsToolkit.Physics._2D.Geometry;

namespace GraphicsToolkit.Physics._2D.Partitions
{
    public class GridPartition2D : Partition2D
    {
        private Cell[] cells;
        private int rows, cols;
        private float invWidth, invHeight;
        private Vector2 min, max;

        public GridPartition2D(Vector2 min, Vector2 max, int rows, int cols)
        {
            cells = new Cell[rows * cols];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell();
            }

            this.rows = rows;
            this.cols = cols;

            UpdateMinMax(min, max);
        }

        public void UpdateMinMax(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;

            float width = (max.X - min.X);
            float height = (max.Y - min.Y);
            invWidth = 1f / width;
            invHeight = 1f / height;
        }

        public override void GenerateContacts(ref List<RigidBody2D> bodies, ref List<Contact2D> contacts, float dt)
        {
            //Clear all cells
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Clear();
            }

            //Insert bodies into all cells they overlap
            for (int i = 0; i < bodies.Count; i++)
            {
                AABB2D bounds = bodies[i].MotionBounds;
                Vector2 min = bounds.GetMin();
                Vector2 max = bounds.GetMax();

                int startRow = (int)(min.X*invWidth*rows);
                startRow = (int)Math.Max(startRow, 0);

                int startCol = (int)(min.Y * invHeight * cols);
                startCol = (int)Math.Max(startCol, 0);

                int endRow = (int)(max.X * invWidth * rows);
                endRow = (int)Math.Min(endRow, rows-1);

                int endCol = (int)(max.Y * invHeight * cols);
                endCol = (int)Math.Min(endCol, cols-1);

                for (int j = startRow; j <= endRow; j++)
                {
                    for (int k = startCol; k <= endCol; k++)
                    {
                        cells[(j * cols) + k].AddObject(i);
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

                Cell currentCell = cells[i];
                foreach (int index1 in currentCell.Indices)
                {
                    foreach (int index2 in currentCell.Indices)
                    {
                        if (index1 == index2)
                        {
                            continue;
                        }

                        RigidBody2D a = bodies[index1];
                        RigidBody2D b = bodies[index2];
                        if (a.InvMass != 0 || b.InvMass != 0)
                        {
                            //Add a speculative contact
                            contacts.Add(a.GenerateContact(b, dt));
                        }
                    }
                }
            }
        }

        internal class Cell
        {
            private LinkedList<int> indices;

            public Cell()
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
