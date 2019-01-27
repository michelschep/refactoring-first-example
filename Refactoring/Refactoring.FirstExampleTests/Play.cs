namespace Refactoring.FirstExampleTests
{
    public class Play
    {
        public string Type { get; }
        public string Name { get; }

        public Play(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}