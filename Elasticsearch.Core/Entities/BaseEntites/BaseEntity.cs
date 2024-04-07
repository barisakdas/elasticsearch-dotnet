namespace Elasticsearch.Core.Entities.BaseEntites;

public class BaseEntity
{
    public string Id { get; set; } = null!;

    /// <summary>Oluşturulma Tarihi.</summary>
    /// <summary>Creation Date.</summary>
    [MaxLength(20)]
    [JsonPropertyName("createddate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>Oluşturan kişinin kullacını veri tabanındaki id si.</summary>
    /// <summary>ID of the creator registered in the organization database.</summary>
    [MaxLength(10)]
    [JsonPropertyName("createdby")]
    public int? CreatedBy { get; set; }

    /// <summary>Verinin güncellenme tarihi.</summary>
    /// <summary>Data updated date.</summary>
    [MaxLength(20)]
    [JsonPropertyName("updateddate")]
    public DateTime? UpdatedDate { get; set; }

    /// <summary>Güncelleyen kişinin kullacını veri tabanındaki id si.</summary>
    /// <summary>ID of the updator registered in the organization database.</summary>
    [MaxLength(10)]
    [JsonPropertyName("updatedby")]
    public int? UpdatedBy { get; set; }

    /// <summary>Veri silme işlemi geldikten sonra false olarak atanan, ama oluşturulduğunda true olarak atanan özellik</summary>
    /// <summary>Property that is set to false after data deletion occurs, but is set to true when created</summary>
    [JsonPropertyName("isactive")]
    public bool IsActive { get; set; } = true;
}