using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    public class MigrationBaseModel : IModel<int>
    {

        public int Id { get; set; }

        public int TdpId { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }
    }
}
