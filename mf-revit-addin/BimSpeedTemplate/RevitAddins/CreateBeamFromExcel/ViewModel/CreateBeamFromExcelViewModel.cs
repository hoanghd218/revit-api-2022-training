using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using RevitAddins.RebarOpeningForSlab.Filter;
using RevitAddins.CreateBeamFromExcel.Model;
using RevitApiUtils;

using Excel = Microsoft.Office.Interop.Excel;

namespace RevitAddins.CreateBeamFromExcel.ViewModel
{
    public class CreateBeamFromExcelViewModel : ViewModelBase
    {
        /// <summary>
        /// Case choose "actived excel file"
        /// </summary>
        private bool _selectedActiveFile;
        public bool SelectedActiveFile
        {
            get { return _selectedActiveFile; }
            set
            {
                _selectedActiveFile = value;
                OnPropertyChanged(nameof(SelectedActiveFile));
            }
        }

        /// <summary>
        /// Case choose "choose excel file"
        /// </summary>
        private bool _selectedChooseFile;
        public bool SelectedChooseFile
        {
            get { return _selectedChooseFile; }
            set
            {
                _selectedChooseFile = value;
                OnPropertyChanged(nameof(SelectedChooseFile));
            }
        }

        /// <summary>
        /// Full path of excel file
        /// </summary>
        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }
        /// <summary>
        /// List Beam from excel file
        /// </summary>
        public List<BeamInfo> ListBeamInfo { get; set; } = new();

        /// <summary>
        /// List Beam type from excel file (mark and profile)
        /// Mark: string
        /// Profie: b = int[0]
        ///         h = int[1]
        /// </summary>
        public Dictionary<string, int[]> ListBeamType { get; set; } = new();

        /// <summary>
        /// All framingfamily in model
        /// </summary>
        private List<Family> _allFramingFamily;
        public List<Family> AllFramingFamily
        {
            get
            {
                if (_allFramingFamily == null)
                {
                    _allFramingFamily = new List<Family>();
                }
                return _allFramingFamily;
            }
            set
            {
                _allFramingFamily = value;
                OnPropertyChanged(nameof(AllFramingFamily));
                this.SelectedFamily = _allFramingFamily.FirstOrDefault();
            }
        }

        /// <summary>
        /// Selected family
        /// </summary>
        private Family _selectedFamily;
        public Family SelectedFamily
        {
            get { return _selectedFamily; }
            set
            {
                _selectedFamily = value;
                OnPropertyChanged(nameof(SelectedFamily));
            }
        }

        /// <summary>
        /// All level in model
        /// </summary>
        private List<Level> _allLevel;
        public List<Level> AllLevel
        {
            get
            {
                if (_allLevel == null)
                {
                    _allLevel = new List<Level>();
                }
                return _allLevel;
            }
            set
            {
                _allLevel = value;
                OnPropertyChanged(nameof(AllLevel));
                this.SelectedLevel = _allLevel.FirstOrDefault();
            }
        }

        /// <summary>
        /// Selected Level;
        /// </summary>
        private Level _selectedLevel;
        public Level SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                _selectedLevel = value;
                OnPropertyChanged(nameof(SelectedLevel));
            }
        }

        /// <summary>
        /// All symbol of selected family
        /// </summary>
        public List<FamilySymbol> AllSymbol { get; set; } = new();

        public XYZ BasePointCad { get; set; }


        /// <summary>
        /// Command
        /// </summary>
        public ICommand CreateCommand { get; set; }
        public ICommand BrowseCommand { get; set; }



        /// <summary>
        /// Constructor
        /// </summary>
        public CreateBeamFromExcelViewModel()
        {
            SelectedActiveFile = true;
            FilteredElementCollector colFramingFamily = new FilteredElementCollector(AC.Document);
            List<Family> lissAllFramingFamily = colFramingFamily.OfClass(typeof(Family)).Cast<Family>().Where(x => x.FamilyCategoryId.IntegerValue == -2001320).ToList();
            this.AllFramingFamily = lissAllFramingFamily;

            FilteredElementCollector colLevel = new FilteredElementCollector(AC.Document);
            List<Level> listAllLevel = colLevel.OfClass(typeof(Level)).WhereElementIsNotElementType().Cast<Level>().ToList();
            this.AllLevel = listAllLevel;

            CreateCommand = new RelayCommand(Create);
            BrowseCommand = new RelayCommand<object>(p => true, a => Browse());
        }



        /// <summary>
        /// Open file dialog to choose file
        /// </summary>
        private void Browse()
        {
            using (OpenFileDialog folder = new OpenFileDialog() { RestoreDirectory = false, Filter = "|*.xlsx" })
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    this.Path = folder.FileName;
                }
            }
        }

        /// <summary>
        /// Method Create beam to Revit
        /// </summary>
        private void Create(object ob)
        {
            if (SelectedActiveFile)
            {
                if (GetDataFromActiveFile() == false) return;
            }
            else if (SelectedChooseFile)
            {
                if (GetDataFromChooseFile() == false) return;
            }

            using (TransactionGroup trGr = new TransactionGroup(AC.Document, "Create symbol and create beam"))
            {
                trGr.Start();
                if (CheckAndCreateFamilySymbol() == false) return;

                Window window = ob as Window;
                window.Hide();

                try
                {
                    XYZ basePoinRevit = AC.UiDoc.Selection.PickPoint("Pick base point");
                    basePoinRevit = new XYZ(basePoinRevit.X, basePoinRevit.Y, 0);
                    //Create beam
                    foreach (var beamInfo in this.ListBeamInfo)
                    {
                        //reference to get profile and symbol
                        beamInfo.B = this.ListBeamType.Where(x => x.Key == beamInfo.Mark).FirstOrDefault().Value[0];
                        beamInfo.H = this.ListBeamType.Where(x => x.Key == beamInfo.Mark).FirstOrDefault().Value[1];
                        beamInfo.FamilySymbol = this.AllSymbol.Where(x => x.Name == beamInfo.Profile).FirstOrDefault();
                        XYZ translateElevation = new XYZ(0, 0, this.SelectedLevel.ProjectElevation);
                        beamInfo.StartPoint = beamInfo.StartPoint - this.BasePointCad + basePoinRevit + translateElevation;
                        beamInfo.EndPoint = beamInfo.EndPoint - this.BasePointCad + basePoinRevit + translateElevation;

                        using (var tr = new Transaction(AC.Document, "Create Beam"))
                        {
                            tr.Start();
                            AC.Document.Create.NewFamilyInstance(beamInfo.Line, beamInfo.FamilySymbol, this.SelectedLevel, StructuralType.Beam);
                            tr.Commit();
                        }
                    }
                    System.Windows.Forms.MessageBox.Show("Done");
                    window.Close();
                }
                catch
                {
                    trGr.RollBack();
                    window.ShowDialog();
                    return;
                }
                trGr.Assimilate();
            }
        }

        /// <summary>
        /// Check And Create Framing Family Type, return all symbol to this.AllSymbol
        /// </summary>
        private bool CheckAndCreateFamilySymbol()
        {
            //Check selected family exist parameter b and h
            var symbolCheck = AC.Document.GetElement(this.SelectedFamily.GetFamilySymbolIds().ToList().First()) as FamilySymbol;
            symbolCheck.GetOrderedParameters();
            List<string> lstParaName = new List<string>();
            foreach (Parameter item in symbolCheck.GetOrderedParameters())
            {
                lstParaName.Add(item.Definition.Name.ToLower());
            }
            if (!(lstParaName.Contains("b") && lstParaName.Contains("h")))
            {
                System.Windows.MessageBox.Show("Choose family constains b and h parameter");
                return false;
            }


            //Check exist and create new symnol
            this.AllSymbol.Clear();
            var elementIds = SelectedFamily.GetFamilySymbolIds().ToList();
            foreach (var item in elementIds)
            {
                var symbol = AC.Document.GetElement(item) as FamilySymbol;
                this.AllSymbol.Add(symbol);
            }
            foreach (var item in this.ListBeamType)
            {
                string typeName = item.Value[0].ToString() + "x" + item.Value[1].ToString();
                if (this.AllSymbol.All(x => !x.Name.Equals(typeName)))
                {
                    var oldSymbol = this.AllSymbol.FirstOrDefault();
                    using (var tr = new Transaction(AC.Document, "Create new familysymbol"))
                    {
                        tr.Start();
                        var symbolToCareate = oldSymbol.Duplicate(typeName) as FamilySymbol;
                        var parameters = symbolToCareate.GetOrderedParameters();
                        foreach (var para in parameters)
                        {
                            if (!para.IsReadOnly)
                            {
                                if (para.Definition.Name.ToLower().Equals("b"))
                                {
                                    para.Set(item.Value[0].MmToFoot());
                                }
                                else if (para.Definition.Name.ToLower().Equals("h"))
                                {
                                    para.Set(item.Value[1].MmToFoot());
                                }
                            }
                        }
                        tr.Commit();
                        this.AllSymbol.Add(symbolToCareate);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Method get data beam type from sheet1 excel file
        /// </summary>
        private bool GetDataFromActiveFile()
        {
            bool isOpenExcel = false;
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains("EXCEL"))
                {
                    isOpenExcel = true;
                    break;
                }
            }
            if (isOpenExcel == false)
            {
                System.Windows.MessageBox.Show("Need open excel!");
                return false;
            }


            Excel.Application xlApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
            Excel.Workbook book = xlApp.ActiveWorkbook;
            if (book == null)
            {
                System.Windows.MessageBox.Show("Need open excel file!");
                return false;
            }
            Excel.Sheets sheets = book.Sheets;
            if (sheets.Count < 2)
            {
                System.Windows.MessageBox.Show("File not enoungh data");
                return false;
            }
            Excel.Worksheet sheet1 = (Excel.Worksheet)sheets[1];
            Excel.Worksheet sheet2 = (Excel.Worksheet)sheets[2];
            Excel.Range range1 = sheet1.UsedRange;
            Excel.Range range2 = sheet2.UsedRange;

            try
            {
                //Sheet1 list beam type
                ListBeamType.Clear();
                for (int i = 2; i <= range1.Rows.Count; i++)
                {
                    string mark = string.Empty;
                    int[] profile = new int[2];

                    mark = range1.Cells[i, 1].Value2.ToString();
                    profile[0] = Convert.ToInt32(range1.Cells[i, 2].Value2.ToString());
                    profile[1] = Convert.ToInt32(range1.Cells[i, 3].Value2.ToString());

                    ListBeamType.Add(mark, profile);
                }

                //Sheet2 (Beam and Cordinate)
                ListBeamInfo.Clear();
                for (int i = 2; i <= range2.Rows.Count; i++)
                {
                    BeamInfo beamInfo = new BeamInfo();
                    string starPoint = string.Empty;
                    string endPoint = string.Empty;

                    beamInfo.Mark = range2.Cells[i, 1].Value2.ToString();
                    starPoint = range2.Cells[i, 2].Value2.ToString();
                    endPoint = range2.Cells[i, 3].Value2.ToString();

                    double startX = Convert.ToDouble(starPoint.Split(';')[0]);
                    double startY = Convert.ToDouble(starPoint.Split(';')[1]);
                    double endX = Convert.ToDouble(endPoint.Split(';')[0]);
                    double endY = Convert.ToDouble(endPoint.Split(';')[1]);

                    double a = 100;
                    beamInfo.StartPoint = new XYZ(startX / 304.79999999999995, startY / 304.79999999999995, 0);
                    beamInfo.EndPoint = new XYZ(endX / 304.79999999999995, endY / 304.79999999999995, 0);

                    ListBeamInfo.Add(beamInfo);
                }
                //BasePoint in Cad
                string basePoint = range2.Cells[1, 5].Value2.ToString();
                double basePointX = Convert.ToDouble(basePoint.Split(';')[0]);
                double basePointY = Convert.ToDouble(basePoint.Split(';')[1]);
                this.BasePointCad = new XYZ(basePointX / 304.79999999999995, basePointY / 304.79999999999995, 0);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Error! Check data in excel file");
                return false;
            }
            return true;

        }

        /// <summary>
        /// Method get data beam type from sheet1 excel file
        /// </summary>
        private bool GetDataFromChooseFile()
        {
            Excel.Application xlApp = new Excel.Application();
            //Chech exist path
            if (!File.Exists(this.Path))
            {
                System.Windows.MessageBox.Show("File does not exists!");
                return false;
            }
            Excel.Workbook book = xlApp.Workbooks.Open(this.Path);
            Excel.Sheets sheets = book.Sheets;
            //Check excel file
            if (sheets.Count < 2)
            {
                System.Windows.MessageBox.Show("File not enoungh data");
                return false;
            }
            Excel.Worksheet sheet1 = (Excel.Worksheet)sheets[1];
            Excel.Worksheet sheet2 = (Excel.Worksheet)sheets[2];
            Excel.Range range1 = sheet1.UsedRange;
            Excel.Range range2 = sheet2.UsedRange;


            try
            {
                ListBeamType.Clear();
                //Sheet1 list beam type
                for (int i = 2; i <= range1.Rows.Count; i++)
                {
                    string mark = string.Empty;
                    int[] profile = new int[2];

                    mark = range1.Cells[i, 1].Value2.ToString();
                    profile[0] = Convert.ToInt32(range1.Cells[i, 2].Value2.ToString());
                    profile[1] = Convert.ToInt32(range1.Cells[i, 3].Value2.ToString());

                    ListBeamType.Add(mark, profile);
                }


                //Sheet2(Beam and Cordinate)
                ListBeamInfo.Clear();
                for (int i = 2; i <= range2.Rows.Count; i++)
                {
                    BeamInfo beamInfo = new BeamInfo();
                    string starPoint = string.Empty;
                    string endPoint = string.Empty;

                    beamInfo.Mark = range2.Cells[i, 1].Value2.ToString();
                    starPoint = range2.Cells[i, 2].Value2.ToString();
                    endPoint = range2.Cells[i, 3].Value2.ToString();

                    double startX = Convert.ToDouble(starPoint.Split(';')[0]);
                    double startY = Convert.ToDouble(starPoint.Split(';')[1]);
                    double endX = Convert.ToDouble(endPoint.Split(';')[0]);
                    double endY = Convert.ToDouble(endPoint.Split(';')[1]);

                    double a = 100;
                    beamInfo.StartPoint = new XYZ(startX / 304.79999999999995, startY / 304.79999999999995, 0);
                    beamInfo.EndPoint = new XYZ(endX / 304.79999999999995, endY / 304.79999999999995, 0);

                    ListBeamInfo.Add(beamInfo);
                }
                //BasePoint in Cad
                string basePoint = range2.Cells[1, 5].Value2.ToString();
                double basePointX = Convert.ToDouble(basePoint.Split(';')[0]);
                double basePointY = Convert.ToDouble(basePoint.Split(';')[1]);
                this.BasePointCad = new XYZ(basePointX / 304.79999999999995, basePointY / 304.79999999999995, 0);
                xlApp.Quit();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Error! Check data in excel file");
                xlApp.Quit();
                return false;
            }
            return true;
        }

    }
}
