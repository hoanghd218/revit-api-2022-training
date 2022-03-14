﻿using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RevitApiUtils
{
   public static class ParameterUtilities
   {

      public static void CreateGroupParameter(Document doc,
         string sharedParameter,
         string groupName,
         string paraName,
         CategorySet inputcatSet,
         BuiltInParameterGroup inputparameterGroup,
         bool SetAllowVaryBetweenGroups = true,
         bool visible = true,
         bool usermodify = true)
      {
         using (Transaction t = new Transaction(doc, "Add Parameter"))
         {
            t.Start();
            Application app = doc.Application;
            // get the shared parameter file
            string oldFile = app.SharedParametersFilename;
            app.SharedParametersFilename = sharedParameter;
            if (!File.Exists(sharedParameter))
            {
               t.RollBack();
               return;
            }
            DefinitionFile file = app.OpenSharedParameterFile();

            // if our group is not there, create it
            DefinitionGroup group = file.Groups.get_Item(groupName);
            if (group == null) group = file.Groups.Create(groupName);


            Definition def = group.Definitions.get_Item(paraName);
            if (null == def)
            {
               ExternalDefinitionCreationOptions opt = new ExternalDefinitionCreationOptions(paraName, ParameterType.Text);
               opt.Visible = visible;
               opt.UserModifiable = usermodify;
               def = group.Definitions.Create(opt);
            }


            // create a binding - instance or type:
            InstanceBinding bind = app.Create.NewInstanceBinding(inputcatSet);
            doc.ParameterBindings.Insert(def, bind, inputparameterGroup);

            SharedParameterElement sp = new FilteredElementCollector(doc)
                        .OfClass(typeof(SharedParameterElement))
                        .Cast<SharedParameterElement>()
                        .Where(x => x.Name == paraName)
                        .FirstOrDefault();

            if (sp != null)
            {
               InternalDefinition internalDefinition = sp.GetDefinition();
               internalDefinition.SetAllowVaryBetweenGroups(doc, SetAllowVaryBetweenGroups);
            }

            //return the shared parameter file
            app.SharedParametersFilename = oldFile;
            t.Commit();
         }

      }
      public static string ValueString(this Parameter para)
      {
         string valueString = string.Empty;
         if (para != null)
         {
            if (para.StorageType == StorageType.Integer)
            {
               valueString = para.AsInteger().ToString();
            }
            else if (para.StorageType == StorageType.Double)
            {
#if Version2017 || Version2018 || Version2019 || Version2020
               double internalUnit = UnitUtils.ConvertFromInternalUnits(para.AsDouble(), para.DisplayUnitType);
               valueString = internalUnit.ToString();
#else
               double internalUnit = UnitUtils.ConvertFromInternalUnits(para.AsDouble(), para.GetUnitTypeId());
               valueString = internalUnit.ToString();
#endif
            }
            else if (para.StorageType == StorageType.String || para.StorageType == StorageType.ElementId)
            {
               valueString = para.AsString();
               if (valueString == string.Empty || valueString == null)
               {
                  valueString = para.AsValueString();
               }
            }
         }

         if (valueString == null)
         {
            return valueString = string.Empty;
         }
         return valueString;
      }

      public static Parameter GetTypeParameter(this Element ele, string param)
      {
         ElementId typeId = ele.GetTypeId();
         return (ele.Document.GetElement(typeId) as ElementType)?.LookupParameter(param);
      }

      public static string GetParameterValueByNameAsString(this Element ele, string p)
      {
         var rs = "";
         var param = ele.LookupParameter(p);
         if (param != null && param.StorageType == StorageType.String)
         {
            rs = param.AsString();
         }
         return rs;
      }

      public static string GetParameterValueAsString(this Element ele, BuiltInParameter p)
      {
         var rs = "";
         var param = ele.get_Parameter(p);
         if (param != null && param.StorageType == StorageType.String)
         {
            rs = param.AsString();
         }

         return rs;
      }

      public static double GetParameterValueByNameAsDouble(this Element ele, string p)
      {
         var rs = 0.0;
         var param = ele.LookupParameter(p);
         if (param != null && param.StorageType == StorageType.Double)
         {
            rs = param.AsDouble();
         }
         return rs;
      }

      public static double GetParameterValueByNameAsDouble(this Element ele, BuiltInParameter pp)
      {
         var rs = 0.0;
         var param = ele.get_Parameter(pp);
         if (param != null && param.StorageType == StorageType.Double)
         {
            rs = param.AsDouble();
         }
         return rs;
      }

      public static int GetParameterValueByNameAsInteger(this Element ele, string p)
      {
         var rs = 0;
         var param = ele.LookupParameter(p);
         if (param != null && param.StorageType == StorageType.Integer)
         {
            rs = param.AsInteger();
         }
         return rs;
      }

      public static int GetParameterValueByNameAsInteger(this Element ele, BuiltInParameter p)
      {
         var rs = 0;
         var param = ele.get_Parameter(p);
         if (param != null && param.StorageType == StorageType.Integer)
         {
            rs = param.AsInteger();
         }
         return rs;
      }

      public static ElementId GetParameterValueByNameAsElementId(this Element ele, string p)
      {
         var rs = ElementId.InvalidElementId;
         var param = ele.LookupParameter(p);
         if (param != null && param.StorageType == StorageType.Double)
         {
            rs = param.AsElementId();
         }
         return rs;
      }

      public static ElementId GetParameterValueByNameAsElementId(this Element ele, BuiltInParameter p)
      {
         var rs = ElementId.InvalidElementId;
         var param = ele.get_Parameter(p);
         if (param != null && param.StorageType == StorageType.ElementId)
         {
            rs = param.AsElementId();
         }
         return rs;
      }

      public static bool SetParameterValueByName(this Element ele, string name, string value)
      {
         var param = ele.LookupParameter(name);
         if (param != null && param.StorageType == StorageType.String && !param.IsReadOnly)
         {
            if (string.IsNullOrEmpty(value) == false)
            {
               param.Set(value);
            }

            return true;
         }
         return false;
      }

      public static bool SetParameterValueByName(this Element ele, BuiltInParameter name, string value)
      {
         var param = ele.get_Parameter(name);
         if (param != null && param.StorageType == StorageType.String && !param.IsReadOnly)
         {
            if (string.IsNullOrEmpty(value) == false)
            {
               param.Set(value);
            }

            return true;
         }
         return false;
      }

      public static bool SetParameterValueByName(this Element ele, BuiltInParameter bip, double value)
      {
         var param = ele.get_Parameter(bip);
         if (param != null && param.StorageType == StorageType.Double && !param.IsReadOnly)
         {
            param.Set(value);

            return true;
         }
         return false;
      }

      public static bool SetParameterValueByName(this Element ele, BuiltInParameter bip, int value)
      {
         var param = ele.get_Parameter(bip);
         if (param != null && param.StorageType == StorageType.Integer && !param.IsReadOnly)
         {
            param.Set(value);

            return true;
         }
         return false;
      }

      public static bool SetParameterValueByName(this Element ele, string name, int value)
      {
         var param = ele.LookupParameter(name);
         if (param != null && param.StorageType == StorageType.Integer && param.IsReadOnly == false)
         {
            try
            {
               param.Set(value);
               return true;
            }
            catch (Exception ex)
            {
               Debug.Print(ex.Message);
            }
         }

         return false;
      }

      public static bool SetParameterValueByName(this Element ele, string name, double value)
      {
         var param = ele.LookupParameter(name);
         if (param != null && param.StorageType == StorageType.Double)
         {
            param.Set(value);
            return true;
         }

         return false;
      }

      public static bool HasParameter(this Element element, string parameterName)
      {
         if (element != null)
         {
            using (var enumerator = element.GetParameters(parameterName).GetEnumerator())
            {
               while (enumerator.MoveNext())
               {
                  object obj = enumerator.Current;
                  Parameter parameter = (Parameter)obj;
                  if (parameterName == parameter.Definition.Name)
                  {
                     return true;
                  }
               }
               return false;
            }
         }
         return false;
      }

      public static bool SetParameterValueByName(this Element ele, string name, ElementId value)
      {
         var param = ele.LookupParameter(name);
         if (param != null && param.StorageType == StorageType.ElementId)
         {
            param.Set(value);
            return true;
         }
         return false;
      }
   }
}