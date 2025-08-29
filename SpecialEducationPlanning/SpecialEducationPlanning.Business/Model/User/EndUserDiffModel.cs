namespace SpecialEducationPlanning
.Business.Model
{
    public class EndUserDiffModel
    {
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public EndUserDiffModel(string name, string oldValue, string newValue)
        {
            this.PropertyName = name;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
