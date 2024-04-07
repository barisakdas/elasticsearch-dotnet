namespace Elasticsearch.Core.Dtos.BaseDtos;

public abstract record BaseDto
{
    public string Id { get; set; } = null!;

    /// <summary>Oluşturulma Tarihi.</summary>
    /// <summary>Creation Date.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Oluşturan kişinin organization veri tabanındaki id si.</summary>
    /// <summary>ID of the creator registered in the organization database.</summary>
    public int? CreatedBy { get; set; }

    /// <summary>Verinin güncellenme tarihi.</summary>
    /// <summary>Data updated date.</summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>Güncelleyen kişinin organization veri tabanındaki id si.</summary>
    /// <summary>ID of the updator registered in the organization database.</summary>
    public int? UpdatedBy { get; set; }

    /// <summary-tr>Veri silme işlemi geldikten sonra false olarak atanan, ama oluşturulduğunda true olarak atanan özellik</summary-tr>
    /// <summary-en>Property that is set to false after data deletion occurs, but is set to true when created</summary-en>
    public bool IsActive { get; set; } = true;
}