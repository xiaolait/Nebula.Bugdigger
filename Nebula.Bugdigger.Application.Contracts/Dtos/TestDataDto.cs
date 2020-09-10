using System;
namespace Nebula.Bugdigger
{
    public class TestDataDto
    {
        public BusTypeDto BusType { get; set; }
        public int NetNo { get; set; }
        public int SrcChannel1 { get; set; }
        public int SrcChannel2 { get; set; }
        public int DesChannel1 { get; set; }
        public int DesChannel2 { get; set; }
        public string Data { get; set; }
    }
}
