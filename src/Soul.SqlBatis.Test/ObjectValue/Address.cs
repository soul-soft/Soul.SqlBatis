namespace Soul.SqlBatis.Entities
{
    [JsonValue]
    public record Address
    {
        public string P { get; set; }
        public string C { get; set; }
        public string A { get; set; }
    }
}
