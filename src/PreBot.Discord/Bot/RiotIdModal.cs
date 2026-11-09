using Discord;
using Discord.Interactions;

namespace PreBot.Bot;

/// <summary>Asks for the Riot ID in Discord before taking a seat (the host invites with it).</summary>
public sealed class RiotIdModal : IModal
{
    public string Title => "Katılmak için Riot ID'n";

    [InputLabel("Riot ID (oyundaki İsim#ETİKET)")]
    [ModalTextInput("riot-id", TextInputStyle.Short, placeholder: "Oyuncu#TR1", minLength: 3, maxLength: 40)]
    public string RiotId { get; set; } = "";
}
