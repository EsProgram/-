using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationAndEncryption
{
  internal static class Algorithm
  {
    /// <summary>
    /// 中央値を返す
    /// </summary>
    public static T PivotSelect<T>(List<T> list) where T : struct,IComparable
    {
      const int DIV_NUM = 5;
      int count_of_list = (int)Math.Ceiling(list.Count / (double)DIV_NUM);

      List<List<T>> split_list = new List<List<T>>();

      //リスト分割
      for(int i = 0; i < count_of_list; ++i)
        if(i == count_of_list - 1)
          split_list.Add(list.Slice(i * DIV_NUM, list.Count).ToList());
        else
          split_list.Add(list.Slice(i * DIV_NUM, (i + 1) * DIV_NUM).ToList());

      //それぞれのリストの中央値を求める
      List<T> medians = new List<T>();
      foreach(var elm in split_list)
      {
        elm.Sort();
        medians.Add(elm[(int)Math.Floor(elm.Count / 2.0)]);
      }

      //中央値の集合からさらに中央値を求める
      return Generic(medians, (int)Math.Ceiling(medians.Count / 2.0));
    }

    /// <summary>
    /// k番目に小さい要素の値を返す
    /// 要素数が少ない場合は要素数の中央の値を返す
    /// </summary>
    private static T Generic<T>(List<T> list, int k) where T : struct,IComparable
    {
      //リストの要素数<kなら例外
      if(list.Count < k)
        throw new ArgumentException("argument 'k' is out of range.");

      //要素数が少ない場合は要素の中央の値を返す
      if(list.Count < 5)
      {
        list.Sort();
        return list[(int)Math.Floor(list.Count / 2.0)];
      }

      var median = PivotSelect(list);

      const int LESS = 0;
      const int EQUAL = 1;
      const int LARGE = 2;
      List<T>[] split_median = new List<T>[3];
      for(int i = 0; i < split_median.Length; ++i)
        split_median[i] = new List<T>();

      foreach(var elm in list)
      {
        if(elm.CompareTo(median) < 0)
          split_median[LESS].Add(elm);
        if(elm.CompareTo(median) == 0)
          split_median[EQUAL].Add(elm);
        if(elm.CompareTo(median) > 0)
          split_median[LARGE].Add(elm);
      }

      if(split_median[LESS].Count >= k)
        return Generic(split_median[LESS], k);
      if(split_median[LESS].Count < k && split_median[EQUAL].Count >= k - split_median[LESS].Count)
        return median;
      if(split_median[LESS].Count + split_median[EQUAL].Count < k)
        return Generic(split_median[LARGE], k - split_median[LESS].Count - split_median[LARGE].Count);

      throw new ApplicationException("algorithm error in Generic method.");
    }
  }
}
