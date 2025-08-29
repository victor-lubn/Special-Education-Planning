using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.DtoModel
{
    public class CreatioProjectDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The CRMProjectName field is required and should not be null or empty.")]
        [JsonProperty("CRMProjectName"), JsonPropertyName("CRMProjectName")]
        [StringLength(DataContext.DefaultPropertyLength)]
        public string CodeProject { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The CRMProjectId field is required and should not be null or empty.")]
        [JsonProperty("CRMProjectId"), JsonPropertyName("CRMProjectId")]
        [StringLength(DataContext.DefaultPropertyLength)]
        public string KeyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Aiep field is required and should not be null or empty.")]
        [JsonProperty("Aiep"), JsonPropertyName("Aiep")]
        [StringLength(DataContext.DefaultPropertyLength)]
        public string AiepCode { get; set; }

        [Required]
        [JsonPropertyName("builder")]
        public BuilderDto Builder { get; set; }

        [Required]
        [JsonPropertyName("housingSpecifications")]
        [MinLength(1, ErrorMessage = "HousingSpecifications array must contain at least one element.")]
        public ICollection<HousingSpecificationDto> HousingSpecifications { get; set; }

    }

    public class BuilderDto
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        public string AccountNumber { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string TradingName { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address1 { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string PostCode { get; set; }
    }

    public class HousingSpecificationDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The CRMHousingSpecificationId field is required and should not be null or empty.")]
        [MaxLength(500, ErrorMessage = "The CRMHousingSpecificationId must be a string with a maximum length of 500")]
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonProperty("CRMHousingSpecificationId"), JsonPropertyName("CRMHousingSpecificationId")]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The CRMHousingSpecificationName field is required and should not be null or empty.")]
        [MaxLength(500, ErrorMessage = "The CRMHousingSpecificationName must be a string with a maximum length of 500")]
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonProperty("CRMHousingSpecificationName"), JsonPropertyName("CRMHousingSpecificationName")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("housingTypes")]
        [MinLength(1, ErrorMessage = "HousingTypes array must contain at least one element.")]
        public ICollection<HousingTypeDto> HousingTypes { get; set; }
    }

    public class HousingTypeDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The CRMHousingTypeId field is required and should not be null or empty.")]
        [MaxLength(500, ErrorMessage = "The CRMHousingTypeId must be a string with a maximum length of 500")]
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonProperty("CRMHousingTypeId"), JsonPropertyName("CRMHousingTypeId")]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The CRMHousingTypeName field is required and should not be null or empty.")]
        [MaxLength(500, ErrorMessage = "The CRMHousingTypeName must be a string with a maximum length of 500")]
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonProperty("CRMHousingTypeName"), JsonPropertyName("CRMHousingTypeName")]
        public string Name { get; set; }
    }
}

