namespace InvisibleCollectorLib.Model
{
    internal interface IRoutableModel
    {
        /// <summary>
        ///     The model's id taht can be used on requests to retrieve models, update models, etc
        /// </summary>
        string RoutableId { get; }
    }
}