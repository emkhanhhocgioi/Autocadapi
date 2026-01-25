using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Model;
using test.Service;

namespace test.ViewModel
{
    internal class ProjectSelectViewModel : ViewModelBase
    {   
        public Action RequestClose;
       
        public ObservableCollection<Project> ProjectCollection { get; set; }
        public RelayCommand SelectProjectCommand => new RelayCommand(() =>);

        public List<Project> Projects { get; set; }




        public ProjectSelectViewModel()
        {
            ProjectCollection = GetTestProjects();
           

        }

        private Project selectedProject;

        public Project SelectedProject
        {
            get { return selectedProject; }
            set
            {
                selectedProject = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Project> GetTestProjects()
        {
            ObservableCollection<Project> projects = new ObservableCollection<Project>();

            // Dự án 1: Vinhomes Central Park
            var vcp_towers = new List<ProjectTower>
            {
                new ProjectTower { towerName = "Landmark 81" },
                new ProjectTower { towerName = "Park 1" },
                new ProjectTower { towerName = "Park 2" },
                new ProjectTower { towerName = "Park 3" }
            };
            var vcp_floors = new List<ProjectFloor>
            {
                new ProjectFloor { floorName = "Floor 1" },
                new ProjectFloor { floorName = "Floor 2" },
                new ProjectFloor { floorName = "Floor 3" },
                new ProjectFloor { floorName = "Floor 5" },
                new ProjectFloor { floorName = "Floor 10" }
            };
            var vcp_units = new List<ProjectUnit>
            {
                new ProjectUnit { unitName = "Unit 101" },
                new ProjectUnit { unitName = "Unit 102" },
                new ProjectUnit { unitName = "Unit 201" },
                new ProjectUnit { unitName = "Unit 202" },
                new ProjectUnit { unitName = "Unit 301" },
                new ProjectUnit { unitName = "Unit 501" }
            };
            projects.Add(new Project("VCP1","Vinhomes Central Park", vcp_towers, vcp_floors, vcp_units));

            // Dự án 2: Masteri Thao Dien
            var mtd_towers = new List<ProjectTower>
            {
                new ProjectTower { towerName = "T1" },
                new ProjectTower { towerName = "T2" },
                new ProjectTower { towerName = "T3" },
                new ProjectTower { towerName = "T4" },
                new ProjectTower { towerName = "T5" }
            };
            var mtd_floors = new List<ProjectFloor>
            {
                new ProjectFloor { floorName = "Floor 5" },
                new ProjectFloor { floorName = "Floor 10" },
                new ProjectFloor { floorName = "Floor 15" },
                new ProjectFloor { floorName = "Floor 20" }
            };
            var mtd_units = new List<ProjectUnit>
            {
                new ProjectUnit { unitName = "Unit A501" },
                new ProjectUnit { unitName = "Unit A502" },
                new ProjectUnit { unitName = "Unit B1001" },
                new ProjectUnit { unitName = "Unit B1002" },
                new ProjectUnit { unitName = "Unit C1501" }
            };
            projects.Add(new Project("MTD1","Masteri Thao Dien", mtd_towers, mtd_floors, mtd_units));

            // Dự án 3: The Sun Avenue
            var tsa_towers = new List<ProjectTower>
            {
                new ProjectTower { towerName = "Tower A" },
                new ProjectTower { towerName = "Tower B" },
                new ProjectTower { towerName = "Tower C" }
            };
            var tsa_floors = new List<ProjectFloor>
            {
                new ProjectFloor { floorName = "Ground Floor" },
                new ProjectFloor { floorName = "Floor 7" },
                new ProjectFloor { floorName = "Floor 12" },
                new ProjectFloor { floorName = "Floor 18" },
                new ProjectFloor { floorName = "Floor 20" }
            };
            var tsa_units = new List<ProjectUnit>
            {
                new ProjectUnit { unitName = "Unit 701A" },
                new ProjectUnit { unitName = "Unit 702A" },
                new ProjectUnit { unitName = "Unit 1201B" },
                new ProjectUnit { unitName = "Unit 1801C" },
                new ProjectUnit { unitName = "Unit 2001A" }
            };
            projects.Add(new Project("TSA1","The Sun Avenue", tsa_towers, tsa_floors, tsa_units));

            // Dự án 4: Gateway Thao Dien
            var gtd_towers = new List<ProjectTower>
            {
                new ProjectTower { towerName = "Aspen" },
                new ProjectTower { towerName = "Madison" },
                new ProjectTower { towerName = "Riva Park" }
            };
            var gtd_floors = new List<ProjectFloor>
            {
                new ProjectFloor { floorName = "Floor 2" },
                new ProjectFloor { floorName = "Floor 6" },
                new ProjectFloor { floorName = "Floor 8" },
                new ProjectFloor { floorName = "Floor 16" }
            };
            var gtd_units = new List<ProjectUnit>
            {
                new ProjectUnit { unitName = "Unit 201" },
                new ProjectUnit { unitName = "Unit 601" },
                new ProjectUnit { unitName = "Unit 801" },
                new ProjectUnit { unitName = "Unit 802" },
                new ProjectUnit { unitName = "Unit 1601" },
                new ProjectUnit { unitName = "Unit 1602" }
            };
            projects.Add(new Project("GTD1" ,"Gateway Thao Dien", gtd_towers, gtd_floors, gtd_units));

            // Dự án 5: Saigon Pearl
            var sp_towers = new List<ProjectTower>
            {
                new ProjectTower { towerName = "Ruby 1" },
                new ProjectTower { towerName = "Ruby 2" },
                new ProjectTower { towerName = "Sapphire 1" },
                new ProjectTower { towerName = "Sapphire 2" },
                new ProjectTower { towerName = "Topaz" },
                new ProjectTower { towerName = "Emerald" }
            };
            var sp_floors = new List<ProjectFloor>
            {
                new ProjectFloor { floorName = "Floor 3" },
                new ProjectFloor { floorName = "Floor 6" },
                new ProjectFloor { floorName = "Floor 9" },
                new ProjectFloor { floorName = "Floor 12" },
                new ProjectFloor { floorName = "Floor 18" }
            };
            var sp_units = new List<ProjectUnit>
            {
                new ProjectUnit { unitName = "Unit 301" },
                new ProjectUnit { unitName = "Unit 302" },
                new ProjectUnit { unitName = "Unit 601" },
                new ProjectUnit { unitName = "Unit 901" },
                new ProjectUnit { unitName = "Unit 1201" },
                new ProjectUnit { unitName = "Unit 1801" },
                new ProjectUnit { unitName = "Unit 1802" }
            };
            projects.Add(new Project("SGP1","Saigon Pearl", sp_towers, sp_floors, sp_units));

            return projects;
        }

        public void SetActiveProject()
        {
            ProjectService.ActiveProjectID = SelectedProject.ProjectID;
        }

        public void Dispose()
        {
            selectedProject = null;
            ProjectCollection = null;
        }

        public void RequestCloseInvoke()
        {
            RequestClose?.Invoke();
        }


    }
}
