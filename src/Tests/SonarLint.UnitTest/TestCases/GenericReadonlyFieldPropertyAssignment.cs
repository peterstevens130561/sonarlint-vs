﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestCases
{
    interface IPoint
    {
        int X { get; set; }
        int Y { get; set; }
    }

    class PointManager<T> where T : IPoint
    {
        readonly T point;  // this could be a struct
        public PointManager(T point)
        {
            this.point = point;
            this.point.X = 1; // Compliant, we are in the constructor
        }

        public void MovePointVertically(int newX)
        {
            point.X = newX; //Noncompliant; if point is a struct, then nothing happened
            point.X++; //Noncompliant; if point is a struct, then nothing happened
            Console.WriteLine(point.X);
        }
    }

    class PointManager2<T> where T : class, IPoint
    {
        readonly T point;  // this can only be a class
        public PointManager2(T point)
        {
            this.point = point;
        }

        public void MovePointVertically(int newX)
        {
            point.X = newX;  // this assignment is guaranteed to work
            Console.WriteLine(point.X);
        }
    }

    class PointManager3<T> where T : struct, IPoint
    {
        readonly T point;  // this could be a struct
        public PointManager3(T point)
        {
            this.point = point;
            this.point.X = 1; // Compliant, we are in the constructor
        }

        public void MovePointVertically(int newX)
        {
            point.X = newX; // Compliant, compiler error CS1648
            point.X++;      // Compliant, compiler error CS1648
            Console.WriteLine(point.X);
        }
    }
}
