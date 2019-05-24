﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AnotherRpgMod.Items
{
    class SuperAdditionalDamageNode : ItemNode
    {
        new protected string m_Name = "(Rare) Additional Damage";
        new protected string m_Desc = "Add";
        new public float rarityWeight = 0.075f;
        
        public override NodeCategory GetNodeCategory 
        {
            get
            {
                
                return NodeCategory.Flat;
            }
        }

        public override string GetName
        {
            get
            {
                return m_Name;
            }
        }

        public override string GetDesc { get {
                return "Add " + (FlatDamage * Utils.Mathf.Clamp(GetLevel,1,GetMaxLevel)) + " Damage";
            } }

        

        public int FlatDamage;

        public override void Passive(Item item)
        {
            item.GetGlobalItem<ItemUpdate>().DamageFlatBuffer += FlatDamage * GetLevel;
        }

        public override void SetPower(float value)
        {
            FlatDamage = Utils.Mathf.Clamp((int)Utils.Mathf.Pow(value*4,1.2f),8,999);
            m_MaxLevel = 1;
            m_RequiredPoints = Utils.Mathf.FloorInt(1 + power * 0.75f)*5;
        }

        public override void LoadValue(string saveValue)
        {
            power = float.Parse(saveValue);
            SetPower(power);
        }

        public override string GetSaveValue()
        {
            return power.ToString();
        }

    }
}