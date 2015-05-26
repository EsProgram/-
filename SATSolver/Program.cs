using Microsoft.SolverFoundation.Solvers;
using Poorsat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Program
{
  private static void Main(string[] args)
  {
    List<List<string>> p = new List<List<string>>(){
         new List<string>(){" "," ","1"," "},
         new List<string>(){" ","3"," ","4"},
         new List<string>(){"3"," ","4"," "},
         new List<string>(){" ","2"," "," "},

         //new List<string>(){" ","3"," "," "},
         //new List<string>(){" ","4"," ","2"},
         //new List<string>(){"3","2"," ","4"},
         //new List<string>(){"4"," ","2"," "},

         //new List<string>(){" ","4"," ","2"},
         //new List<string>(){"1","2"," "," "},
         //new List<string>(){" "," ","4","3"},
         //new List<string>(){"4"," "," ","1"},

         //new List<string>(){"3","7"," ","6"," ","9"," ","2","1"},
         //new List<string>(){"4"," "," "," ","8"," "," "," ","3"},
         //new List<string>(){" "," ","6","3"," ","5","7"," "," "},
         //new List<string>(){"6"," ","4"," "," "," ","3"," ","9"},
         //new List<string>(){" ","9"," "," ","3"," "," ","8"," "},
         //new List<string>(){"2"," ","7"," "," "," ","1"," ","4"},
         //new List<string>(){" "," ","1","9"," ","8","4"," "," "},
         //new List<string>(){" "," "," "," ","6"," "," "," "," "},
         //new List<string>(){"8","2"," ","1"," ","4"," ","9","5"},
       };

    Suudoku suudoku = new Suudoku(p.Count);

    var r = suudoku.Solve(p);

    //結果の表示
    for(int j = 0; j < p.Count; ++j)
    {
      for(int i = 0; i < p.Count; ++i)
        Console.Write(r[j, i] + " ");
      Console.WriteLine();
    }
  }
}
