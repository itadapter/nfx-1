﻿/*<FILE_LICENSE>
* NFX (.NET Framework Extension) Unistack Library
* Copyright 2003-2018 Agnicore Inc. portions ITAdapter Corp. Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
</FILE_LICENSE>*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

using NFX.Scripting;

using NFX;
using NFX.Collections;
using NFX.Serialization.Slim;
using NFX.Serialization.JSON;

namespace NFX.UTest.Serialization
{
  [Runnable(TRUN.BASE)]
  public class Slim5
  {

    [Run]
    public void Circular1()
    {
      using (var ms = new MemoryStream())
      {
        var s = new SlimSerializer();

        var dIn = new cA
        {
          A = 2190,
          B = 23232
        };

        dIn.Child = new cA { A = -100, B = -900, Child = dIn };//circular reference!!!


        s.Serialize(ms, dIn);
        ms.Seek(0, SeekOrigin.Begin);

        var dOut = (cA)s.Deserialize(ms);

        Aver.AreEqual(2190, dOut.A);
        Aver.AreEqual(23232, dOut.B);
        Aver.IsNotNull(dOut.Child);
        Aver.AreEqual(-100, dOut.Child.A);
        Aver.AreEqual(-900, dOut.Child.B);
        Aver.AreSameRef(dOut, dOut.Child.Child);
      }
    }


    [Run]
    public void Circular2()
    {
      using (var ms = new MemoryStream())
      {
        var s = new SlimSerializer();

        var dIn = new[]{
         new cA {  A = 1, B = 100},
         new cB {  A = 2, B = 200},
         new cB {  A = 3, B = 300},
        };

        dIn[0].Child = dIn[2];
        dIn[1].Child = dIn[0];
        dIn[2].Child = dIn[1]; //circular reference


        s.Serialize(ms, dIn);
        ms.Seek(0, SeekOrigin.Begin);

        var dOut = (cA[])s.Deserialize(ms);

        Aver.AreEqual( 3, dOut.Length );
        Aver.IsTrue( dOut[0] is cA );
        Aver.IsTrue( dOut[1] is cB );
        Aver.IsTrue( dOut[2] is cB );

        Aver.AreEqual(1, dOut[0].A);
        Aver.AreEqual(100, dOut[0].B);

        Aver.AreEqual(2, dOut[1].A);
        Aver.AreEqual(200, dOut[1].B);

        Aver.AreEqual(3, dOut[2].A);
        Aver.AreEqual(300, dOut[2].B);

      }
    }



    private class cA
    {

      protected int m_A;
      protected int m_B;
      protected cA m_Child;

      public int A{ get{ return m_A;} set{m_A = value;}}
      public int B{ get{ return m_B;} set{m_B = value;}}
      public cA Child{ get{ return m_Child;} set{m_Child = value;}}
    }

    private class cB : cA
    {
    }


  }
}
