using System;

namespace Tripod
{
    public class FakeEntityWithSortableProperties : EntityWithId<int>
    {
        public DateTime DateTime { get; set; }
        public DateTime? NullableDateTime { get; set; }
        public bool Bool { get; set; }
        public bool? NullableBool { get; set; }
        public int Int { get; set; }
        public int? NullableInt { get; set; }
        public long Long { get; set; }
        public long? NullableLong { get; set; }
        public short Short { get; set; }
        public short? NullableShort { get; set; }
        public double Double { get; set; }
        public double? NullableDouble { get; set; }
        public float Float { get; set; }
        public float? NullableFloat { get; set; }
        public decimal Decimal { get; set; }
        public decimal? NullableDecimal { get; set; }
        public byte Byte { get; set; }
    }
}