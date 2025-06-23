using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ActivitySpec
{
    public class ActivitySpecification : BaseSpecification<Activity>
    {
        public ActivitySpecification() : base(p => p.IsVisible) { }
        

        //public ActivitySpecification(int id)
        //   : base(p => p.Id == id && p.IsVisible)
        //{
        //    AddIncludes();
        //}

        //public ActivitySpecification(GetAllPatientsQuery query)
        //: base(
        //      p => p.IsActive &&
        //       (string.IsNullOrEmpty(query.FullName) || p.FullName.Contains(query.FullName)) &&
        //       (string.IsNullOrEmpty(query.Phone) || p.Phone.Contains(query.Phone)) &&
        //       (string.IsNullOrEmpty(query.Status) || p.Status == Enum.Parse<PatientStatus>(query.Status)))
        //{
        //    AddIncludes();
        //    ApplyOrderByDescending(p => p.Id);
        //}


        //private void AddIncludes()
        //{
        //    Includes.Add(p => p.InsuranceCompany);
        //    Includes.Add(p => p.InsuranceCategory);
        //}
    }
}
