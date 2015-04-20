using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationAndEncryption
{
  internal static class ListExtension
  {
    public static IEnumerable<T> Slice<T>(this IEnumerable<T> list, int start, int end) where T : struct
    {
      if(start >= end)
        throw new ArgumentException("index range error.");
      return list.Skip(start).Take(end - start);
    }
  }
}
