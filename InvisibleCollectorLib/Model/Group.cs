namespace InvisibleCollectorLib.Model
{
    public class Group : Model
    {
        internal const string IdName = "id";
        internal const string NameName = "name";
        
        public string Id
        {
            get => GetField<string>(IdName);

            set => this[IdName] = value;
        }
        
        public string Name
        {
            get => GetField<string>(NameName);

            set => this[NameName] = value;
        }

    }
}