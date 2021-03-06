﻿using System;
using System.Collections.Generic;

namespace Tests.Diagnostics
{
    public abstract partial class A
    {
        public abstract void X();
    }
    public abstract partial class A
    {
        public void Y();
    }

    public abstract class Empty //Noncompliant
    {

    }

    public abstract class Animal //Noncompliant
    {
        abstract void move();
        abstract void feed();

    }

    public abstract class Animal2 : SomeBaseClass //Compliant
    {
        abstract void move();
        abstract void feed();

    }

    public abstract class Color //Noncompliant
    {
        private int red = 0;
        private int green = 0;
        private int blue = 0;

        public int getRed()
        {
            return red;
        }
    }

    public interface AnimalCompliant
    {

        void move();
        void feed();

    }

    public class ColorCompliant
    {
        private int red = 0;
        private int green = 0;
        private int blue = 0;

        private ColorCompliant()
        { }

        public int getRed()
        {
            return red;
        }
    }

    public abstract class LampCompliant
    {

        private bool switchLamp = false;

        public abstract void glow();

        public void flipSwitch()
        {
            switchLamp = !switchLamp;
            if (switchLamp)
            {
                glow();
            }
        }
    }

    public abstract class View //Noncompliant, should be an interface
    {
        public abstract string Content { get; }
    }

    public abstract class View2 //Compliant, has abstract and non abstract members
    {
        public abstract string Content { get; }
        public string Content2 { get; }
    }
}
