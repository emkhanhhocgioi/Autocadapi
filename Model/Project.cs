using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public class Project 
    {

        public string ProjectID { get; set; }
        public string projectName { get; set; }
        public List<ProjectTower> towers { get; set; }
        public List<ProjectFloor> floors { get; set; }
        public List<ProjectUnit> units { get; set; }

        public Project(string id ,string pj , List<ProjectTower> pt, List<ProjectFloor> pf, List<ProjectUnit> pu)
        {   
            this.ProjectID = id;
            this.projectName = pj;
            this.towers = pt;
            this.floors = pf;
            this.units = pu;
        }


        

    }
    public class ProjectTower {
    public string towerName { get; set; }
    }
    public class ProjectFloor
    {

        public string floorName { get; set; }
    }
    public class ProjectUnit
    {
        public string unitName { get; set; }
    }
}
