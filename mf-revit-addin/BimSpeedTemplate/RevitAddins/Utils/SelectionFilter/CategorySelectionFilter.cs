using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace RevitApiUtils
{
   public class CategorySelectionFilter
   {
      public CategorySelectionFilter(Category category)
      {
         mCategoryIds = new HashSet<ElementId>();
         mCategoryIds.Add(category.Id);
      }

      public CategorySelectionFilter(IEnumerable<Category> categories)
      {
         this.mCategoryIds = new HashSet<ElementId>(from x in categories
                                                    select x.Id);
      }

      public bool AllowElement(Element elem)
      {
         bool result = elem.Category != null && mCategoryIds.Contains(elem.Category.Id);
         return result;
      }

      public bool AllowReference(Reference reference, XYZ position)
      {
         return false;
      }

      protected readonly HashSet<ElementId> mCategoryIds;
   }
}