using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationAndEncryption
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var seq = new List<int>();

      //var rand = new Random();
      //for(int i = 0; i < 32; ++i)
      //seq.Add(rand.Next(0, 100));

      #region PDFの配列を作る作業

      //確認のためpdfから配列コピペした
      int[] arr = {12, 4, 5, 3, 24, 0, 15, 29, 2, 27, 1, 21, 5, 4, 5, 20, 29, 24, 20,
                   19, 43, 21, 23, 39, 4, 34, 33, 5, 46, 25, 52, 42, 33, 5, 28, 43, 47, 26,
                   43, 59, 46, 69, 67, 46, 34, 61, 32, 80, 27, 62, 44, 51, 24, 34, 49, 49, 53,
                   52, 63, 73, 85, 84, 48, 75, 64, 51, 91, 47, 90, 73, 74, 32, 49, 59, 82, 86,
                   81, 72, 98, 91, 89, 59, 84, 97, 78, 99, 79, 96, 77, 86, 93, 67, 70, 91};

      //コピペしたまんまの順番ではないので、配列を転置
      int[] arr2 = new int[arr.Length];
      int index = 0;
      for(int j = 0; j < 19; ++j)
        for(int i = 0; i < 5; ++i)
        {
          if(j + i * 19 >= arr2.Length)
            break;
          arr2[index] = arr[j + i * 19];
          index++;
        }

      seq = arr2.ToList();
      #endregion PDFの配列を作る作業

      //中央値の表示
      Console.WriteLine("Pivot : " + Algorithm.PivotSelect(seq) + Environment.NewLine);

      //配列の表示
      Console.WriteLine("Data that is not sorted:");
      ShowSequence(arr);
      Console.WriteLine();

      //ソートした配列の表示
      Console.WriteLine("Data that is sorted:");
      seq.Sort();
      ShowSequence(seq);
    }

    private static void ShowSequence<T>(IEnumerable<T> seq)
    {
      foreach(var elm in seq.Select((v, i) => { return new { v, i }; }))
      {
        if(elm.i % 19 == 0)
          Console.WriteLine(Environment.NewLine);
        Console.Write(elm.v + "\t");
      }
      Console.WriteLine();
    }
  }
}
