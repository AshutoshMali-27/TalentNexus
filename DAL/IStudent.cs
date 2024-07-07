using BusinessLogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicObject;

namespace DAL
{
    public interface IStudent
    {
        ModelMessage AddStudent(Students objstudent);
        
    }
}
