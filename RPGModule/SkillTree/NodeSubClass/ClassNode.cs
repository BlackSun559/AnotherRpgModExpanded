using AnotherRpgModExpanded.RPGModule.Entities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AnotherRpgModExpanded.RPGModule;

internal class ClassNode : Node
{
    public ClassNode(ClassType _classType, NodeType _type, bool _unlocked = false, float _value = 1,
        int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1, bool _ascended = false) : base(_type,
        _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel, _ascended)
    {
        GetClassType = _classType;
    }

    public ClassType GetClassType { get; }

    public string GetClassName => Language.GetTextValue("Mods.AnotherRpgModExpanded.ClassName." + GetClassType);

    public void loadingUpgrade()
    {
        base.Upgrade();
    }

    public override void Upgrade()
    {
        base.Upgrade();
        UpdateClass();
    }

    public void Disable(RpgPlayer p)
    {
        if (p.GetSkillTree.ActiveClass == this)
            p.GetSkillTree.ActiveClass = null;
        enable = false;
    }

    public void Disable()
    {
        if (Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetSkillTree.ActiveClass == this)
            Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetSkillTree.ActiveClass = null;
        enable = false;
    }

    public void UpdateClass()
    {
        NodeParent _node;
        var Active = ClassType.Hobo;

        if (Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetSkillTree.ActiveClass != null)
            Active = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetSkillTree.ActiveClass.GetClassType;
        for (var i = 0;
             i < Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetSkillTree.nodeList.nodeList.Count;
             i++)
        {
            _node = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetSkillTree.nodeList.nodeList[i];

            if (_node.GetNodeType == NodeType.Class)
            {
                var classNode = (ClassNode)_node.GetNode;

                if (Active != classNode.GetClassType && classNode.GetActivate) classNode.Disable();
            }
        }
    }

    public override void ToggleEnable()
    {
        base.ToggleEnable();

        if (enable)
            Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetSkillTree.ActiveClass = this;
        else
            Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>().GetSkillTree.ActiveClass = null;

        var player = Main.player[Main.myPlayer].GetModPlayer<RpgPlayer>();


        UpdateClass();

        if (Main.netMode == NetmodeID.MultiplayerClient)
            player.SendClientChanges(player);
    }
}