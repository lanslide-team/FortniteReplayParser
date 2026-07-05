using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortniteReplayParser.Data;

[Table("fortnite_replays_results")]
public class FortniteReplayResult
{
    [Key]
    [Column("id", TypeName = "char(36)")]
    public Guid Id { get; set; }

    // FK -> fortnite_replays.id
    [Column("fortnite_replays_id", TypeName = "char(36)")]
    public Guid? FortniteReplaysId { get; set; }

    [ForeignKey(nameof(FortniteReplaysId))]
    public FortniteReplay? Replay { get; set; }

    [Column("epic_id")]
    [MaxLength(64)]
    public string? EpicId { get; set; }

    [Column("player_name")]
    public string? PlayerName { get; set; }

    [Column("placement")]
    public int? Placement { get; set; }

    [Column("eliminations")]
    public uint? Eliminations { get; set; }

    [Column("team_index")]
    public int? TeamIndex { get; set; }    

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
