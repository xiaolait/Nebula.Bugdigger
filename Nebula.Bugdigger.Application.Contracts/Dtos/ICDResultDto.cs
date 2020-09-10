using System;
namespace Nebula.Bugdigger
{
    public class ICDResultDto
    {
        public string Expect { get; set; }
        public string Actual { get; set; }
        public bool IsPass { get; set; }
        public string Remark { get; set; }
    }
}
