using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnotherRpgModExpanded.Utils;

internal class MPDebug
{
    public static void Log(Mod mod, object message)
    {
        Log(mod, message.ToString());
    }

    public static void Log(Mod mod, string message)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            var packet = mod.GetPacket();
            packet.Write((byte)Message.Log);
            packet.Write(message);
            packet.Send();
        }
    }
}