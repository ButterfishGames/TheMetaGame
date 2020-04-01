using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : EnemyBehaviour
{
    public Dialogue firstDialogue, otherDialogue;
    public bool interacted;

    public enum NPCType
    {
        standard,
        restoreHP,
        restoreMP,
        learnSpell,
        learnSkill,
        boostHP,
        boostMP,
        boostStrength,
        boostMagic
    };
    public NPCType npcType;

    public int amt;
    public Spell spell;
    public Skill skill;

    public void Start()
    {
        interacted = false;
    }

    public void Interact()
    {
        StartCoroutine(InteractRtn());
    }

    private IEnumerator InteractRtn()
    {
        if (interacted)
        {
            DialogueManager.singleton.StartDialogue(otherDialogue);
        }
        else
        {
            DialogueManager.singleton.StartDialogue(firstDialogue);
        }

        yield return new WaitUntil(() => !DialogueManager.singleton.GetDisplaying());

        bool found = false;

        switch (npcType)
        {
            case NPCType.restoreHP:
                GameController.singleton.Damage(-GameController.singleton.maxHP);
                SaveManager.singleton.UpdatePlayerData();
                break;

            case NPCType.restoreMP:
                GameController.singleton.Cast(-GameController.singleton.maxMP);
                SaveManager.singleton.UpdatePlayerData();
                break;

            case NPCType.boostHP:
                if (!interacted)
                {
                    GameController.singleton.maxHP += amt;
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostMP:
                if (!interacted)
                {
                    GameController.singleton.maxMP += amt;
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostStrength:
                if (!interacted)
                {
                    GameController.singleton.SetStrength(GameController.singleton.GetStrength() + amt);
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostMagic:
                if (!interacted)
                {
                    GameController.singleton.SetMagic(GameController.singleton.GetMagic() + amt);
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.learnSpell:
                if (!interacted)
                {
                    for (int i = 0; i < GameController.singleton.spellList.Length && !found; i++)
                    {
                        if (GameController.singleton.spellList[i].spell.spellName.Equals(spell.spellName))
                        {
                            GameController.singleton.spellList[i].unlocked = true;
                            SaveManager.singleton.UpdatePlayerData();
                            found = true;
                        }
                    }
                }
                break;
        }

        if (!interacted)
        {
            interacted = true;
            SaveManager.singleton.UpdateSceneData();
        }
    }
}
