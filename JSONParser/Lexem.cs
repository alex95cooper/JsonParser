namespace JSONParser
{
    internal class Lexem
    {
        public Lexem(int key, string value)
        {
            Key = key;
            Value = value;
        }

        public int Key { get; set; }
        public string Value { get; set; }
    }
}
