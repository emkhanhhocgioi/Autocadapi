using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.ViewModel
{
    internal class ParameterEditVM: ViewModelBase
    {
        private double _distanceW;
        private double _distance1;



        public ParameterEditVM(double distanceW, double distance1)
        {
            _distanceW = distanceW;
            _distance1 = distance1;
        }


    }
}
