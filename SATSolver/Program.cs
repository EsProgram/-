using Microsoft.SolverFoundation.Solvers;
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
    List<List<string>> puzzle = new List<List<string>>();
    int[,] result;

    if(args.Length > 0)
    {
      var path = ArgumentPath(args);
      if(string.IsNullOrEmpty(path))
      {
        Console.WriteLine("コマンドライン引数が正しくありません");
        return;
      }
      puzzle = CreatePuzzle(path);
    }
    else
      puzzle = CreatePuzzle(Directory.GetCurrentDirectory() + "/puzzle.txt");
    Sudoku sudoku = new Sudoku();

    if(sudoku.Solve(puzzle, out result))
    {
      Console.WriteLine("解がありました");

      //結果の表示
      for(int j = 0; j < puzzle.Count; ++j)
      {
        for(int i = 0; i < puzzle.Count; ++i)
          Console.Write(result[j, i] + " ");
        Console.WriteLine();
      }
    }
    else
      Console.WriteLine("解が見つかりませんでした");
  }

  /// <summary>
  /// コマンドライン引数に-fが入力されていた場合、ファイルを読み込む
  /// </summary>
  private static string ArgumentPath(string[] args)
  {
    var path = args.Where((v, i) => i > 0 && args[i - 1] == "-f").First();
    return path;
  }

  private static List<List<string>> CreatePuzzle(string filePath)
  {
    List<List<string>> ret = new List<List<string>>();
    using(StreamReader sr = new StreamReader(filePath))
    {
      string line;
      int count = 0;
      while(!sr.EndOfStream)
      {
        line = sr.ReadLine();
        if(!string.IsNullOrWhiteSpace(line))
        {
          var lineSplit = line.Split(' ');
          ret.Add(new List<string>());
          foreach(var str in lineSplit)
            ret[count].Add(str);
          ++count;
        }
      }
    }

    return ret;
  }
}
