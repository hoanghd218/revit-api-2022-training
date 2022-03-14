using Autodesk.Revit.DB;
namespace RevitAddins.CreateBeamFromExcel.Model
{
    public class BeamInfo
    {
        public string Mark { get; set; }

        public FamilySymbol FamilySymbol { get; set; }

        public int B { get; set; }

        public int H { get; set; }

        public XYZ StartPoint { get; set; }

        public XYZ EndPoint { get; set; }

        private string _profile;
        public string Profile
        {
            get
            {
                if (_profile == null)
                {
                    _profile = B.ToString() + "x" + H.ToString();
                }
                return _profile;
            }
            set
            {
                _profile = value;
            }
        }

        private Line _line;

        public Line Line
        {
            get
            {
                if (_line == null)
                {
                    _line = Line.CreateBound(this.StartPoint, this.EndPoint);
                }
                return _line;
            }
            set
            {
                _line = value;
            }
        }


    }
}
