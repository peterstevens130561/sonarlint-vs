﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Tests.Diagnostics
{
    public class DisposableNotDisposed
    {
        public Stream field1 = new MemoryStream(), field5; //Compliant
        private static Stream field2; //Compliant

        private Stream field3; //Noncompliant
        private Stream field6 = new MemoryStream(); //Noncompliant
        private Stream field4; //Compliant, disposed
        private Stream field7; //Compliant, disposed

        public void Init()
        {
            field3 = new MemoryStream();
            field4 = new MemoryStream();
            field7 = new MemoryStream();
        }

        public void DoSomething()
        {
            field4.Dispose();
            field7?.Dispose();
        }
        public void WriteToFile(string path, string text)
        {
            var fs = new FileStream(path, FileMode.Open);  // Noncompliant
            var bytes = Encoding.UTF8.GetBytes(text);
            fs.Write(bytes, 0, bytes.Length);
        }

        public void WriteToFileOk(string path, string text)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        public void WriteToFileReturned(string path, string text)
        {
            var fs = new FileStream(path, FileMode.Open); // Compliant as it is returned
            var bytes = Encoding.UTF8.GetBytes(text);
            fs.Write(bytes, 0, bytes.Length);
            return fs;
        }

        public void WriteToFileEx2(Streams ss)
        {
            var fs = new BinaryReader(s.Stream); // Compliant as it is getting a non-local IDisposable as an argument
        }

        public void WriteToFileEx5(Streams ss)
        {
            var fs = new BinaryReader() // Compliant as it is getting a non-local IDisposable as an argument
            {
                BaseStream = ss.Stream
            };
        }

        public void WriteToFileEx3()
        {
            Stream sl = null;
            var fs = new BinaryReader(sl); // Noncompliant
        }

        public Stream WriteToFileEx4()
        {
            Stream sr = null;
            var fs = new BinaryReader(sr); // Noncompliant, this is a false positive
            return sr;
        }

        public Stream WriteToFileEx6()
        {
            Stream sr = null;
            var fs = new BinaryReader() // Noncompliant, this is a false positive
            {
                BaseStream = sr
            };
            return sr;
        }

        public void WriteElvis()
        {
            var fs = new BinaryReader();
            fs?.Dispose();
        }
    }

    public interface IContainer : IDisposable
    { }
    public class Container : IContainer
    {
        public void Dispose()
        { }
    }

    class Form1
    {
        private IContainer components = null; // Compliant
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        #endregion
    }
}
