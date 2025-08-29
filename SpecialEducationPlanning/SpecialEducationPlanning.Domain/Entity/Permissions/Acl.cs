namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Acl
    {
        #region Properties
        public string EntityType { get; set; }

        public int EntityId { get; set; }

        public int UserId { get; set; }
        #endregion
    }
}
