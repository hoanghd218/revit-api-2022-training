﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RevitApiUtils
{
   public static class ObjectUtils
   {
      public static IEnumerable<T> GetReverse<T>(IEnumerable<T> input)
      {
         return new Stack<T>(input);
      }
      public static string ToValueString(this object obj)
      {
         return obj != null ? obj.ToString() : "";
      }

      public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
      {
         var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
         var destProps = typeof(TU).GetProperties()
                 .Where(x => x.CanWrite)
                 .ToList();

         foreach (var sourceProp in sourceProps)
         {
            if (destProps.Any(x => x.Name == sourceProp.Name))
            {
               var p = destProps.First(x => x.Name == sourceProp.Name);
               if (p.CanWrite)
               { // check if the property can be set or no.
                  p.SetValue(dest, sourceProp.GetValue(source, null), null);
               }
            }
         }
      }

      public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
      {
         var sortableList = new List<T>(collection);
         sortableList.Sort(comparison);

         for (int i = 0; i < sortableList.Count; i++)
         {
            collection.Move(collection.IndexOf(sortableList[i]), i);
         }
      }
      public static void Sort<T>(this ObservableCollection<T> collection, IComparer<T> comparer)
      {
         var sortableList = new List<T>(collection);
         sortableList.Sort(comparer);

         for (int i = 0; i < sortableList.Count; i++)
         {
            collection.Move(collection.IndexOf(sortableList[i]), i);
         }
      }
      public static void RemoveAll<T>(this ObservableCollection<T> collection, Predicate<T> predicate)
      {
         var sortableList = new List<T>(collection);
         sortableList.RemoveAll(predicate);
         collection.Clear();
         for (int i = 0; i < sortableList.Count; i++)
         {
            collection.Add(sortableList[i]);
         }
      }
   }
}