using Microsoft.SolverFoundation.Solvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 数独を解く
/// </summary>
internal class Sudoku
{
  private int N;
  private int blockN;
  private List<Literal[]> ConditionsList;

  /// <summary>
  /// <para>与えられた数独のリストから数独を解く</para>
  /// <para>数独のリストは以下のように与えるものとする</para>
  /// <para>   List{List{string}} p = new List{List{string}}(){</para>
  /// <para>     new List{string}(){" "," ","1"," "},</para>
  /// <para>     new List{string}(){" ","3"," ","4"},</para>
  /// <para>     new List{string}(){"3"," ","4"," "},</para>
  /// <para>     new List{string}(){" ","2"," "," "},</para>
  /// <para>    };</para>
  /// <para>または</para>
  /// <para>   List{List{string}} p = new List{List{string}}(){</para>
  /// <para>     new List{string}(){"0","0","1","0"},</para>
  /// <para>     new List{string}(){"0","3","0","4"},</para>
  /// <para>     new List{string}(){"3","0","4","0"},</para>
  /// <para>     new List{string}(){"0","2","0","0"},</para>
  /// <para>    };</para>
  /// </summary>
  /// <param name="sudoku">数独のリスト</param>
  /// <param name="result">結果を格納する配列</param>
  /// <returns></returns>
  public bool Solve(List<List<string>> sudoku, out int[,] result)
  {
    /**************************************************
     * 引数チェック
     **************************************************/

    N = sudoku.Count;
    blockN = int.Parse(Math.Sqrt(N).ToString());
    if(!sudoku.All(l => l.Count == N))
      throw new ArgumentException("数独のリストが正しく入力されていません");

    /**************************************************
     * 各マスにN個の条件を付与
     **************************************************/

    List<List<List<string>>> masCond = new List<List<List<string>>>();

    //各列について
    for(int k = 0; k < N; ++k)
    {
      masCond.Add(new List<List<string>>());

      //各行について
      for(int j = 0; j < N; ++j)
      {
        masCond[k].Add(new List<string>());

        //各マスN個の条件
        for(int i = 0; i < N; ++i)
          masCond[k][j].Add(((k + 1) * 10).ToString() + ((j + 1) * 10).ToString() + (i + 1).ToString());
      }
    }

    /**************************************************
     * 各行に同じ数字が2度現れない条件を付与
     **************************************************/
    List<List<List<List<string>>>> rowCond = new List<List<List<List<string>>>>();

    //各行について
    for(int j = 0; j < N; ++j)
    {
      rowCond.Add(new List<List<List<string>>>());

      for(int i = 0; i < N; ++i)
      {
        rowCond[j].Add(new List<List<string>>());
        foreach(var c in Combinations(Enumerable.Range(0, N).ToList(), 2).Select((_v, _i) => new { _v, _i }))
        {
          rowCond[j][i].Add(new List<string>());
          foreach(var l in c._v.Select((_v, _i) => new { _v, _i }))
            rowCond[j][i][c._i]
              .Add("-" + ((j + 1) * 10).ToString() + ((l._v + 1) * 10).ToString() + (i + 1).ToString());
        }
      }
    }

    /**************************************************
     * 各列に同じ数字が2度現れない条件を付与
     **************************************************/
    List<List<List<List<string>>>> colCond = new List<List<List<List<string>>>>();

    //各行について
    for(int j = 0; j < N; ++j)
    {
      colCond.Add(new List<List<List<string>>>());
      for(int i = 0; i < N; ++i)
      {
        colCond[j].Add(new List<List<string>>());
        foreach(var c in Combinations(Enumerable.Range(0, N).ToList(), 2).Select((_v, _i) => new { _v, _i }))
        {
          colCond[j][i].Add(new List<string>());
          foreach(var l in c._v.Select((_v, _i) => new { _v, _i }))
            colCond[j][i][c._i]
              .Add("-" + ((l._v + 1) * 10).ToString() + ((j + 1) * 10).ToString() + (i + 1).ToString());
        }
      }
    }

    /**************************************************
     * 各ブロックに同じ数字が2度現れない条件を付与
     **************************************************/
    List<List<string>> blockCond = new List<List<string>>();
    List<string> buf = new List<string>();

    for(int yBlock = 0; yBlock < N; yBlock += blockN)
      for(int xBlock = 0; xBlock < N; xBlock += blockN)
      {
        //o0~onまで
        for(int n = 0; n < blockN * blockN; ++n)
        {
          //o0ブロックcheck
          for(int j = 0; j < blockN; ++j)
            for(int i = 0; i < blockN; ++i)
              buf.Add("-" + ((j + 1) * 10).ToString() + ((i + 1) * 10).ToString() + (n + 1).ToString());
          foreach(var c in Combinations(buf, 2))
            blockCond.Add(c);
          buf = new List<string>();
        }
      }

    /**************************************************
     * Solverに渡すリストの作成
     **************************************************/
    ConditionsList = new List<Literal[]>();

    foreach(var a in masCond)
      foreach(var b in a)
        ConditionsList.Add(ConditionConverter(b));
    foreach(var a in rowCond)
      foreach(var b in a)
        foreach(var c in b)
          ConditionsList.Add(ConditionConverter(c));
    foreach(var a in colCond)
      foreach(var b in a)
        foreach(var c in b)
          ConditionsList.Add(ConditionConverter(c));
    foreach(var a in blockCond)
      ConditionsList.Add(ConditionConverter(a));
    foreach(var a in sudoku.Select((v, i) => new { v, i }))
      foreach(var b in a.v.Select((v, i) => new { v, i }))
        if(!string.IsNullOrWhiteSpace(b.v) && b.v != "0")
          ConditionsList.Add(
            ConditionConverter(new List<string>() { ((a.i + 1) * 10).ToString() + ((b.i + 1) * 10).ToString() + b.v })
          );

    /**************************************************
     * 結果を返す
     **************************************************/

    result = new int[N, N];

    SatSolverParams param = new SatSolverParams();
    var count = ConditionsList.Count;
    foreach(var c in ConditionsList)
      count += c.Count();

    IEnumerable<SatSolution> solutions = null;

    while(true)
    {
      try
      {
        count *= 2;
        solutions = SatSolver.Solve(param, count, ConditionsList);
        break;
      }
      catch(IndexOutOfRangeException)
      {
        //Console.WriteLine("計算に必要な変数が足りなかったため再計算します");
        GC.Collect();
      }
      catch(OutOfMemoryException e)
      {
        throw new OutOfMemoryException("計算で使用するメモリ領域が足りなかったため計算できませんでした", e);
      }
    }

    var solutionsArray = solutions.ToArray();

    //solutionsArrayの解から配列を生成して返す
    if(solutionsArray.FirstOrDefault() != null)
    {
      //var resultString = solutionsArray.First().ToString().Replace("{", "").Replace("}", "").Split(',');
      var resultString = solutionsArray.Last().ToString().Replace("{", "").Replace("}", "").Split(',');

      foreach(var rs in resultString)
      {
        //rsの行/列末尾の0で区切って、パースしてresultに格納
        List<StringBuilder> sb = new List<StringBuilder>();
        int sb_index = 0;
        sb.Add(new StringBuilder());
        for(int i = 0; i < rs.Length; ++i)
        {
          if(rs[i] == '0' && i + 1 < rs.Length && rs[i + 1] != '0')
          {
            ++sb_index;
            sb.Add(new StringBuilder());
            continue;
          }
          sb[sb_index].Append(rs[i]);
        }

        result[int.Parse(sb[0].ToString()) - 1, int.Parse(sb[1].ToString()) - 1] = int.Parse(sb[2].ToString());
      }
      return true;
    }

    //問題が解けなかった場合
    else
      return false;
  }

  /// <summary>
  /// 条件節をSatSolver用にコンバートする
  /// </summary>
  /// <param name="list">条件節</param>
  /// <returns></returns>
  private Literal[] ConditionConverter(List<string> list)
  {
    Literal[] literals = new Literal[list.Count];
    for(int i = 0; i < literals.Length; ++i)
    {
      bool fSence = true;
      if(list[i][0] == '-')
      {
        fSence = false;
        list[i] = list[i].Remove(0, 1);
      }
      literals[i] = new Literal(int.Parse(list[i]), fSence);
    }
    return literals;
  }

  private static List<List<T>> Combinations<T>(IList<T> elements, int choose)
  {
    var ret = new List<List<T>>();

    // 再帰呼び出しの終端
    if(elements.Count < choose)
      return new List<List<T>>();
    else if(choose <= 0)
      return new List<List<T>>() { new List<T>() };

    // 本体
    for(var n = 1; n <= elements.Count; n++)
    {
      var subRet = Combinations(elements.Skip(n).ToList(), choose - 1);
      subRet.ForEach(e => e.Add(elements[n - 1]));
      ret.AddRange(subRet);
    }

    return ret;
  }
}
