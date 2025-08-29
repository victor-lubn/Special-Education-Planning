namespace SpecialEducationPlanning
.Business.IService.IFileStorage
{
    public interface IAzureOptionsConfiguration
    {
        public static string Section{ get; set; }
        public string AzureAccountName { get; set; }
        public string AzureKeyValue { get; set; }
    }
}
