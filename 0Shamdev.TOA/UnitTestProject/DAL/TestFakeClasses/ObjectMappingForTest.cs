using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Attribute;
using System.ComponentModel.DataAnnotations;

namespace UnitTestProject.DAL.TestFakeClasses
{
    internal class ObjectMappingForTest : DomainObject
    {

        [MinLength(10)]
        public string StrValue { get; set; }

        public int? IntValue { get; set; }
        [Summary("Строка")]
        public int? IntValue2 { get; set; }
        public SubObjectMappingForTest SubObject { get; set; }
    }
}
