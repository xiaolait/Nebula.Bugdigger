using System;
namespace Nebula.Bugdigger
{
    public class Field
    {
        public string Name { get; set; }
        public FieldParam Param { get; set; } = new FieldParam();
        public string Remark { get; set; }
    }
}
