using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public enum ImageFiles
    {
        SliderImages = 0,
        ActivityImages = 1,
        ActivitySliderImages = 2,
        ExportFiles = 3,
        PhotoImages = 4,
        PhotoDetailImages = 5,
        EventSliderImages = 6,
        BeneFactorImages = 7,
        BeneFactorDetailsImages = 8,
        ProjectSliderImages = 9
    }

    public enum TaskPriority
    {
        HighPriority = 1,
        MediumPriority = 2,
        LowPriority = 3,
    }

    public enum TaskStatus
    {
        Completed = 1,
        InProgress = 2,
        Finished = 3
    }

    public enum StudentStatus
    {
        Present = 1, // موجود
        Withdrawn = 2, // منسحب 
        Deleted = 3 // محذوف
    }
}
